using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Battle;
using BoolPyeon.Save;

namespace BoolPyeon.Save.Tests
{
    public class SaveServiceTests
    {
        private string dir;

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
        }

        [Test]
        public void Meta_RoundTrip()
        {
            var svc = new SaveService(dir);
            svc.AddCrystals(42);
            svc.SetUpgradeLevel("HP_UP", 2);
            svc.Data.settings.bgmVolume = 0.5f;
            svc.Data.settings.sfxVolume = 0.25f;
            svc.Save(svc.Data);

            var loaded = new SaveService(dir).Load();
            Assert.AreEqual(42, loaded.crystals);
            Assert.AreEqual(2, loaded.permanentUpgrades.Find(e => e.id == "HP_UP").level);
            Assert.AreEqual(0.5f, loaded.settings.bgmVolume, 0.001f);
            Assert.AreEqual(0.25f, loaded.settings.sfxVolume, 0.001f);
        }

        [Test]
        public void Run_RoundTrip()
        {
            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.displayName = "Tester";
            character.maxHp = 30;
            character.attack = 3;
            character.defense = 1;
            character.skills = new List<SkillDefinition>();

            var skill = ScriptableObject.CreateInstance<SkillDefinition>();
            skill.displayName = "Slash";
            skill.effects = new List<SkillEffect>();

            var attackUp = ScriptableObject.CreateInstance<UpgradeDefinition>();
            attackUp.name = "ATK_UP";
            attackUp.kind = UpgradeKind.Attack;
            attackUp.value = 5;

            var hpUp = ScriptableObject.CreateInstance<UpgradeDefinition>();
            hpUp.name = "HP_UP";
            hpUp.kind = UpgradeKind.Hp;
            hpUp.value = 10;

            var learnUp = ScriptableObject.CreateInstance<UpgradeDefinition>();
            learnUp.name = "LEARN_SLASH";
            learnUp.kind = UpgradeKind.LearnSkill;
            learnUp.skill = skill;

            var pool = new List<UpgradeDefinition> { attackUp, hpUp, learnUp };

            var state = new RunState(character);
            state.Map = StageMaze.Generate(seed: 12345, combatRoomCount: 1, eliteRoomCount: 1);

            foreach (var def in pool) state.Apply(def);

            // 두 칸 진행(첫 칸은 클리어까지)
            var firstDir = state.Map.GetMovableDirections()[0];
            Assert.IsTrue(state.Map.TryMove(firstDir));
            if (!state.Map.Current.cleared) state.Map.CompleteCurrent();
            var secondDir = state.Map.GetMovableDirections()[0];
            Assert.IsTrue(state.Map.TryMove(secondDir));

            var squad = state.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(7);
            state.CaptureHp(squad);

            var svc = new SaveService(dir);
            svc.SaveRun(state, combatRoomCount: 1, eliteRoomCount: 1);

            var loadedSvc = new SaveService(dir);
            var restored = RunSaveMapper.Restore(loadedSvc.Data.run, character, pool);

            Assert.AreEqual(state.Map.Seed, restored.Map.Seed);
            Assert.AreEqual(state.Map.CurrentPos, restored.Map.CurrentPos);
            for (int x = 0; x < state.Map.Width; x++)
                for (int y = 0; y < state.Map.Height; y++)
                    Assert.AreEqual(state.Map.GetRoom(x, y).cleared, restored.Map.GetRoom(x, y).cleared, $"({x},{y}) cleared");
            Assert.AreEqual(state.CurrentHp, restored.CurrentHp);
            Assert.AreEqual(state.BonusAttack, restored.BonusAttack);
            Assert.AreEqual(state.BonusMaxHp, restored.BonusMaxHp);
            Assert.AreEqual(state.Skills.Count, restored.Skills.Count);
        }

        // T2: 옛(currentHps 없는) 세이브를 불러오면 currentHp(단일)만으로 복원된다(멤버0만).
        [Test]
        public void LegacySnapshot_WithoutHpArray_RestoresLeaderOnly()
        {
            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.displayName = "Tester";
            character.maxHp = 30;
            character.skills = new List<SkillDefinition>();

            var snap = new RunSnapshot
            {
                seed = 1,
                combatRoomCount = 1,
                eliteRoomCount = 1,
                currentHp = 12,
                currentHps = new int[0],
            };

            var restored = RunSaveMapper.Restore(snap, character, new List<UpgradeDefinition>());

            Assert.AreEqual(12, restored.CurrentHp);
            var squad = restored.CreateSquad();
            Assert.AreEqual(12, squad.Members[0].Combatant.Hp);
        }

        // T2.1: 2인 편성 세이브를 편성 전원 리스트로 복원하면 멤버별 HP가 그대로 돌아온다.
        [Test]
        public void MultiMemberSnapshot_RestoresAllMembersHp()
        {
            var leader = ScriptableObject.CreateInstance<CharacterDefinition>();
            leader.displayName = "Leader";
            leader.maxHp = 20;
            leader.skills = new List<SkillDefinition>();

            var second = ScriptableObject.CreateInstance<CharacterDefinition>();
            second.displayName = "Second";
            second.maxHp = 15;
            second.skills = new List<SkillDefinition>();

            var squadDefs = new List<CharacterDefinition> { leader, second };
            var state = new RunState(squadDefs);
            state.Map = StageMaze.Generate(seed: 1, combatRoomCount: 1, eliteRoomCount: 1);

            var squad = state.CreateSquad();
            squad.Members[0].Combatant.TakeDamage(squad.Members[0].Combatant.MaxHp - 5); // -> hp 5
            squad.Members[1].Combatant.TakeDamage(squad.Members[1].Combatant.MaxHp - 7); // -> hp 7
            state.CaptureHp(squad);

            var snap = RunSaveMapper.Capture(state, combatRoomCount: 1, eliteRoomCount: 1);
            var restored = RunSaveMapper.Restore(snap, squadDefs, new List<UpgradeDefinition>());

            Assert.AreEqual(2, restored.Members.Count);
            var restoredSquad = restored.CreateSquad();
            Assert.AreEqual(2, restoredSquad.Members.Count);
            Assert.AreEqual(5, restoredSquad.Members[0].Combatant.Hp);
            Assert.AreEqual(7, restoredSquad.Members[1].Combatant.Hp);
        }

        // T9: 3인 편성, 사망 멤버(HP 0) 포함 세이브 round-trip — Capture/Restore 로 멤버별 HP(사망 포함)가 그대로 돌아온다.
        [Test]
        public void MultiMemberSnapshot_ThreeMembers_WithDead_RoundTrip()
        {
            var a = ScriptableObject.CreateInstance<CharacterDefinition>();
            a.displayName = "A"; a.maxHp = 20; a.skills = new List<SkillDefinition>();
            var b = ScriptableObject.CreateInstance<CharacterDefinition>();
            b.displayName = "B"; b.maxHp = 20; b.skills = new List<SkillDefinition>();
            var c = ScriptableObject.CreateInstance<CharacterDefinition>();
            c.displayName = "C"; c.maxHp = 20; c.skills = new List<SkillDefinition>();

            var squadDefs = new List<CharacterDefinition> { a, b, c };
            var state = new RunState(squadDefs);
            state.Map = StageMaze.Generate(seed: 2, combatRoomCount: 1, eliteRoomCount: 1);
            state.RestoreHps(new List<int?> { 12, 0, null });

            var snap = RunSaveMapper.Capture(state, combatRoomCount: 1, eliteRoomCount: 1);
            var restored = RunSaveMapper.Restore(snap, squadDefs, new List<UpgradeDefinition>());

            Assert.AreEqual(3, restored.CurrentHps.Count);
            Assert.AreEqual(12, restored.CurrentHps[0]);
            Assert.AreEqual(0, restored.CurrentHps[1]);
            Assert.IsNull(restored.CurrentHps[2]);
        }

        // T2.1: 옛(currentHps 비고 currentHp 단일) 세이브를 2인 리스트로 복원하면 멤버0만 채우고 멤버1은 가득 찬다.
        [Test]
        public void LegacySnapshot_RestoredWithMultiMemberList_FillsFirstOnly()
        {
            var leader = ScriptableObject.CreateInstance<CharacterDefinition>();
            leader.displayName = "Leader";
            leader.maxHp = 30;
            leader.skills = new List<SkillDefinition>();

            var second = ScriptableObject.CreateInstance<CharacterDefinition>();
            second.displayName = "Second";
            second.maxHp = 18;
            second.skills = new List<SkillDefinition>();

            var squadDefs = new List<CharacterDefinition> { leader, second };

            var snap = new RunSnapshot
            {
                seed = 1,
                combatRoomCount = 1,
                eliteRoomCount = 1,
                currentHp = 5,
                currentHps = new int[0],
            };

            var restored = RunSaveMapper.Restore(snap, squadDefs, new List<UpgradeDefinition>());
            var squad = restored.CreateSquad();

            Assert.AreEqual(5, squad.Members[0].Combatant.Hp);
            Assert.AreEqual(18, squad.Members[1].Combatant.Hp);
        }

        [Test]
        public void MissingOrCorrupt_ReturnsDefault()
        {
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "save.json");
            File.WriteAllText(path, "{ not valid json ");

            var svc = new SaveService(dir);
            SaveData data = null;
            Assert.DoesNotThrow(() => data = svc.Load());
            Assert.AreEqual(0, data.crystals);
            Assert.IsFalse(data.hasRun);
            Assert.IsTrue(File.Exists(svc.BackupPath));
        }

        [Test]
        public void ClearRun_KeepsMeta()
        {
            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.maxHp = 30;
            character.skills = new List<SkillDefinition>();

            var svc = new SaveService(dir);
            svc.AddCrystals(9);
            var state = new RunState(character);
            state.Map = StageMaze.Generate(1, 1, 1);
            svc.SaveRun(state, 1, 1);

            svc.ClearRun();

            Assert.AreEqual(9, svc.Data.crystals);
            Assert.IsFalse(svc.Data.hasRun);
        }

        [Test]
        public void NewInstance_SameDir_Loads()
        {
            var svc = new SaveService(dir);
            svc.AddCrystals(3);

            var reopened = new SaveService(dir);
            Assert.AreEqual(3, reopened.Data.crystals);
        }

        // T2: 1스테이지 클리어 → 다음 런은 StageNumber=2 로 시작한다.
        [Test]
        public void Stage1Clear_NextRunStartsAt2()
        {
            var svc = new SaveService(dir);
            svc.MarkStageCleared(1);

            Assert.AreEqual(1, svc.Data.highestClearedStage);
            int startStage = RunState.ComputeStartStage(svc.Data.highestClearedStage, 3);
            Assert.AreEqual(2, startStage);
        }

        // T2: 사망(ClearRun)해도 highestClearedStage 는 그대로 유지된다.
        [Test]
        public void Death_KeepsProgress()
        {
            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.maxHp = 30;
            character.skills = new List<SkillDefinition>();

            var svc = new SaveService(dir);
            svc.MarkStageCleared(1);
            var state = new RunState(character);
            state.Map = StageMaze.Generate(1, 1, 1);
            svc.SaveRun(state, 1, 1);

            svc.ClearRun();

            Assert.AreEqual(1, svc.Data.highestClearedStage);
            Assert.IsFalse(svc.Data.hasRun);
        }

        // T2: 저장·재시작(새 SaveService 인스턴스) 후에도 진행과 stageNumber 가 유지된다.
        [Test]
        public void Progress_PersistsAfterReload()
        {
            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.maxHp = 30;
            character.skills = new List<SkillDefinition>();
            var pool = new List<UpgradeDefinition>();

            var svc = new SaveService(dir);
            svc.MarkStageCleared(1);

            var reloaded = new SaveService(dir);
            Assert.AreEqual(1, reloaded.Data.highestClearedStage);

            var state = new RunState(character);
            state.Map = StageMaze.Generate(2, 1, 1);
            state.StageNumber = 2;
            reloaded.SaveRun(state, 1, 1);

            var reloaded2 = new SaveService(dir);
            Assert.AreEqual(2, reloaded2.Data.run.stageNumber);
            var restored = RunSaveMapper.Restore(reloaded2.Data.run, character, pool);
            Assert.AreEqual(2, restored.StageNumber);
        }

        // T2: 구버전(v1) 저장을 읽으면 version=2 로 올라가고 highestClearedStage 는 0 으로 보정된다.
        [Test]
        public void Migrate_V1_SetsVersion2()
        {
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "save.json");
            File.WriteAllText(path, "{\"version\":1,\"crystals\":5}");

            var svc = new SaveService(dir);
            var data = svc.Load();

            Assert.AreEqual(SaveService.CurrentVersion, data.version);
            Assert.AreEqual(0, data.highestClearedStage);
        }
    }
}
