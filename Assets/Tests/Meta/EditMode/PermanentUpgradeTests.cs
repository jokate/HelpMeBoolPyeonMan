using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Battle;
using BoolPyeon.Save;

namespace BoolPyeon.Meta.Tests
{
    public class PermanentUpgradeTests
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

        private static PermanentUpgradeDefinition MakeDef(string name, PermanentUpgradeKind kind, int[] costs, int[] values)
        {
            var def = ScriptableObject.CreateInstance<PermanentUpgradeDefinition>();
            def.name = name;
            def.kind = kind;
            def.costs = costs;
            def.values = values;
            return def;
        }

        [Test]
        public void Buy_DeductsCrystals_PersistsAcrossReopen()
        {
            var save = new SaveService(dir);
            save.AddCrystals(20);
            var maxHp = MakeDef("PU_MaxHp", PermanentUpgradeKind.MaxHp, new[] { 5, 10, 20 }, new[] { 5, 5, 5 });

            var service = new PermanentUpgradeService(save);
            Assert.IsTrue(service.TryBuy(maxHp));
            Assert.AreEqual(1, service.GetLevel(maxHp));
            Assert.AreEqual(15, save.Data.crystals);

            var reopened = new SaveService(dir);
            var reopenedService = new PermanentUpgradeService(reopened);
            Assert.AreEqual(1, reopenedService.GetLevel(maxHp));
            Assert.AreEqual(15, reopened.Data.crystals);
        }

        [Test]
        public void CannotBuy_WhenShortOfCrystals_OrMaxed()
        {
            var save = new SaveService(dir);
            save.AddCrystals(3);
            var maxHp = MakeDef("PU_MaxHp", PermanentUpgradeKind.MaxHp, new[] { 5, 10 }, new[] { 5, 5 });
            var service = new PermanentUpgradeService(save);

            Assert.IsFalse(service.CanBuy(maxHp));
            Assert.IsFalse(service.TryBuy(maxHp));
            Assert.AreEqual(0, service.GetLevel(maxHp));
            Assert.AreEqual(3, save.Data.crystals);

            save.AddCrystals(100);
            Assert.IsTrue(service.TryBuy(maxHp));
            Assert.IsTrue(service.TryBuy(maxHp));
            Assert.IsFalse(service.CanBuy(maxHp));
            Assert.IsFalse(service.TryBuy(maxHp));
            Assert.AreEqual(2, service.GetLevel(maxHp));
        }

        [Test]
        public void ApplyTo_AddsBonusesToNewRunState()
        {
            var save = new SaveService(dir);
            save.AddCrystals(1000);
            var maxHp = MakeDef("PU_MaxHp", PermanentUpgradeKind.MaxHp, new[] { 5, 10 }, new[] { 10, 5 });
            var attack = MakeDef("PU_Attack", PermanentUpgradeKind.Attack, new[] { 8 }, new[] { 1 });

            var service = new PermanentUpgradeService(save);
            service.TryBuy(maxHp);
            service.TryBuy(maxHp); // level 2 -> total +15
            service.TryBuy(attack); // level 1 -> total +1

            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.displayName = "Tester";
            character.maxHp = 30;
            character.attack = 3;
            character.defense = 1;
            character.skills = new List<SkillDefinition>();

            var state = new RunState(character);
            PermanentUpgradeService.ApplyTo(state, new[] { maxHp, attack }, save);

            var squad = state.CreateSquad();
            Assert.AreEqual(45, squad.Members[0].Combatant.MaxHp);
            Assert.AreEqual(4, squad.Members[0].Combatant.Attack);
        }
    }
}
