using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Dialogue;
using BoolPyeon.Save;

namespace BoolPyeon.Meta.Tests
{
    public class AffinityTests
    {
        private string dir;
        private readonly List<UnityEngine.Object> created = new List<UnityEngine.Object>();

        [SetUp]
        public void SetUp()
        {
            dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            try { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
            catch { /* best effort */ }
            foreach (var o in created) UnityEngine.Object.DestroyImmediate(o);
            created.Clear();
        }

        private AffinityDefinition MakeAffinity(string agentId, params AffinityStage[] stages)
        {
            var def = ScriptableObject.CreateInstance<AffinityDefinition>();
            def.agentId = agentId;
            def.stages = new List<AffinityStage>(stages);
            created.Add(def);
            return def;
        }

        private ModuleDefinition MakeModule(string name)
        {
            var m = ScriptableObject.CreateInstance<ModuleDefinition>();
            m.name = name;
            m.unlockedByDefault = false;
            created.Add(m);
            return m;
        }

        [Test]
        public void FirstTalk_AddsOnePoint()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var def = MakeAffinity("Agent_Story");

            Assert.IsTrue(service.CanTalk(def));
            Assert.IsTrue(service.TryTalk(def, out _));
            Assert.AreEqual(1, service.GetPoints(def));
        }

        [Test]
        public void SecondTalk_SameRun_Fails()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var def = MakeAffinity("Agent_Story");

            Assert.IsTrue(service.TryTalk(def, out _));
            Assert.IsFalse(service.CanTalk(def));
            Assert.IsFalse(service.TryTalk(def, out _));
            Assert.AreEqual(1, service.GetPoints(def)); // 아무것도 바뀌지 않는다
        }

        [Test]
        public void NotifyRunStarted_AllowsTalkAgain()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var def = MakeAffinity("Agent_Story");

            Assert.IsTrue(service.TryTalk(def, out _));
            Assert.IsFalse(service.CanTalk(def));

            AffinityService.NotifyRunStarted(save);

            Assert.IsTrue(service.CanTalk(def));
            Assert.IsTrue(service.TryTalk(def, out _));
            Assert.AreEqual(2, service.GetPoints(def));
        }

        [Test]
        public void ReachStage_UnlocksStoryAndGrantsRewardOnce()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var module = MakeModule("Module_Test");
            var stage1 = new AffinityStage { requiredPoints = 1, rewardKind = AffinityRewardKind.Crystals, amount = 10 };
            var stage2 = new AffinityStage { requiredPoints = 2, rewardKind = AffinityRewardKind.ModuleSlot, amount = 1 };
            var stage3 = new AffinityStage { requiredPoints = 4, rewardKind = AffinityRewardKind.ModuleUnlock, module = module };
            var def = MakeAffinity("Agent_Story", stage1, stage2, stage3);

            int startCrystals = save.Data.crystals;
            int startSlots = save.Data.moduleSlots;

            Assert.IsTrue(service.TryTalk(def, out var reached1));
            Assert.AreEqual(stage1, reached1);
            Assert.AreEqual(startCrystals + 10, save.Data.crystals);

            AffinityService.NotifyRunStarted(save);
            Assert.IsTrue(service.TryTalk(def, out var reached2));
            Assert.AreEqual(stage2, reached2);
            Assert.AreEqual(startSlots + 1, save.Data.moduleSlots);

            // 단계 재도달 없이 재대화해도 이미 받은 보상은 다시 지급되지 않는다(같은 단계 중복 지급 확인용으로
            // crystals 값이 그대로인지 아래 재로드 테스트에서 다시 검증한다).
            Assert.IsFalse(service.IsModuleUnlocked(module));

            AffinityService.NotifyRunStarted(save);
            Assert.IsTrue(service.TryTalk(def, out _)); // points=3, 아직 stage3(4) 미도달
            Assert.IsFalse(service.IsModuleUnlocked(module));

            AffinityService.NotifyRunStarted(save);
            Assert.IsTrue(service.TryTalk(def, out var reached4));
            Assert.AreEqual(stage3, reached4);
            Assert.IsTrue(service.IsModuleUnlocked(module));

            var stories = service.GetUnlockedStories(def);
            Assert.AreEqual(0, stories.Count); // storyDialogue 를 안 채웠으니 목록은 비어있다(널 걸러짐)
        }

        [Test]
        public void ReloadedSaveService_KeepsPointsAndDoesNotRegrantReward()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var stage1 = new AffinityStage { requiredPoints = 1, rewardKind = AffinityRewardKind.Crystals, amount = 10 };
            var def = MakeAffinity("Agent_Story", stage1);

            Assert.IsTrue(service.TryTalk(def, out _));
            int crystalsAfterFirst = save.Data.crystals;

            var reopened = new SaveService(dir);
            var reopenedService = new AffinityService(reopened);
            Assert.AreEqual(1, reopenedService.GetPoints(def));
            Assert.AreEqual(crystalsAfterFirst, reopened.Data.crystals);

            // 다시 저장을 write 해도(같은 단계 재도달 없이) 보상이 중복 지급되지 않는다.
            AffinityService.NotifyRunStarted(reopened);
            Assert.IsTrue(reopenedService.TryTalk(def, out var reached));
            Assert.IsNull(reached); // 이미 claimed 된 stage1 은 다시 reached 로 나오지 않는다
            Assert.AreEqual(crystalsAfterFirst, reopened.Data.crystals);
        }

        [Test]
        public void MaxStageReached_FurtherTalkStillAddsPointsButNoMoreRewards()
        {
            var save = new SaveService(dir);
            var service = new AffinityService(save);
            var stage1 = new AffinityStage { requiredPoints = 1, rewardKind = AffinityRewardKind.Crystals, amount = 5 };
            var def = MakeAffinity("Agent_Story", stage1);

            Assert.IsTrue(service.TryTalk(def, out var reached1));
            Assert.IsNotNull(reached1);
            int crystalsAfter = save.Data.crystals;

            AffinityService.NotifyRunStarted(save);
            Assert.IsTrue(service.TryTalk(def, out var reached2));
            Assert.IsNull(reached2); // 더 이상 새 단계가 없다
            Assert.AreEqual(crystalsAfter, save.Data.crystals); // 보상 중복 없음
            Assert.AreEqual(2, service.GetPoints(def));
        }
    }
}
