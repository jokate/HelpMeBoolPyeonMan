using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Battle;
using BoolPyeon.Save;

namespace BoolPyeon.Meta.Tests
{
    // F-30: 후방 지원 스쿼드 편성 저장/제한(SquadService).
    // 스쿼드별 출제 풀·에너지 규칙(SquadPoolBuilder/SquadEnergyRule)은 BattleFlow 폴더(기본 어셈블리)에
    // 있어 이 EditMode 테스트 어셈블리(BoolPyeon.Meta.Tests)에서 참조할 수 없다 — RunCommand 로 확인했다.
    public class SquadTests
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

        private CharacterDefinition MakeCharacter(string id, InconvenienceType type, CharacterTrait trait)
        {
            var c = ScriptableObject.CreateInstance<CharacterDefinition>();
            c.id = id;
            c.displayName = id;
            c.type = type;
            c.trait = trait;
            created.Add(c);
            return c;
        }

        [Test]
        public void Toggle_AddsAndRemoves_UpTo3_LeaderProtected()
        {
            var save = new SaveService(dir);
            var service = new SquadService(save);
            var hero = MakeCharacter("hero", InconvenienceType.Home, CharacterTrait.Volume);
            var cook = MakeCharacter("cook", InconvenienceType.Food, CharacterTrait.Volume);
            var fashion = MakeCharacter("fashion", InconvenienceType.Wear, CharacterTrait.HighScore);
            var extra = MakeCharacter("extra", InconvenienceType.Device, CharacterTrait.Volume);

            var defs = new[] { hero, cook, fashion, extra };
            CollectionAssert.AreEqual(new[] { "hero" }, ConvertToIds(service.GetSquad(defs)));

            Assert.IsTrue(service.Toggle("cook"));
            Assert.IsTrue(service.Toggle("fashion"));
            CollectionAssert.AreEqual(new[] { "hero", "cook", "fashion" }, ConvertToIds(service.GetSquad(defs)));

            // 4번째는 거부
            Assert.IsFalse(service.Toggle("extra"));
            CollectionAssert.AreEqual(new[] { "hero", "cook", "fashion" }, ConvertToIds(service.GetSquad(defs)));

            // 리더는 해제 불가
            Assert.IsFalse(service.Toggle("hero"));
            Assert.IsTrue(service.IsSelected("hero"));

            Assert.IsTrue(service.Toggle("cook")); // 해제
            CollectionAssert.AreEqual(new[] { "hero", "fashion" }, ConvertToIds(service.GetSquad(defs)));
        }

        [Test]
        public void SquadIds_PersistAcrossReopen()
        {
            var save = new SaveService(dir);
            var service = new SquadService(save);
            var hero = MakeCharacter("hero", InconvenienceType.Home, CharacterTrait.Volume);
            var cook = MakeCharacter("cook", InconvenienceType.Food, CharacterTrait.Volume);
            Assert.IsTrue(service.Toggle("cook"));

            var reopened = new SaveService(dir);
            var reopenedService = new SquadService(reopened);
            var defs = new[] { hero, cook };
            CollectionAssert.AreEqual(new[] { "hero", "cook" }, ConvertToIds(reopenedService.GetSquad(defs)));
        }

        [Test]
        public void Toggle_MinimumOneMember_CannotRemoveLastNonLeader()
        {
            var save = new SaveService(dir);
            var service = new SquadService(save);
            var hero = MakeCharacter("hero", InconvenienceType.Home, CharacterTrait.Volume);
            var cook = MakeCharacter("cook", InconvenienceType.Food, CharacterTrait.Volume);

            // hero 는 처음부터 있고 리더라 해제 불가. cook 을 추가/제거 왕복은 허용.
            Assert.IsTrue(service.Toggle("cook"));
            Assert.IsTrue(service.Toggle("cook"));
            var defs = new[] { hero, cook };
            CollectionAssert.AreEqual(new[] { "hero" }, ConvertToIds(service.GetSquad(defs)));
        }

        private static List<string> ConvertToIds(List<CharacterDefinition> members)
        {
            var ids = new List<string>();
            foreach (var m in members) ids.Add(m.id);
            return ids;
        }
    }
}
