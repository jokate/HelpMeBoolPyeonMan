using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Battle;
using BoolPyeon.Save;

namespace BoolPyeon.Meta.Tests
{
    public class ModuleTests
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

        private ModuleDefinition MakeModule(string name, ModuleKind kind, int param = 3)
        {
            var def = ScriptableObject.CreateInstance<ModuleDefinition>();
            def.name = name;
            def.kind = kind;
            def.param = param;
            created.Add(def);
            return def;
        }

        private CharacterDefinition MakeCharacter()
        {
            var attack = ScriptableObject.CreateInstance<SkillDefinition>();
            attack.energyCost = 0;
            attack.targetType = SkillTargetType.SingleEnemy;
            attack.effects = new List<SkillEffect> { new SkillEffect { kind = SkillEffectKind.Damage, amount = 0 } };
            created.Add(attack);

            var character = ScriptableObject.CreateInstance<CharacterDefinition>();
            character.displayName = "Tester";
            character.maxHp = 30;
            character.attack = 5;
            character.defense = 0;
            character.attackSkill = attack;
            character.skills = new List<SkillDefinition>();
            created.Add(character);
            return character;
        }

        private static BattleState NewBattle(CharacterDefinition character, int enemyHp = 20)
        {
            var monster = ScriptableObject.CreateInstance<MonsterDefinition>();
            monster.displayName = "M";
            monster.maxHp = enemyHp;
            monster.attack = 1;
            monster.actionPattern = new List<MonsterActionKind> { MonsterActionKind.Attack };

            var squad = new Squad();
            squad.TryAdd(character);
            var state = new BattleState(squad, new List<MonsterDefinition> { monster });
            return state;
        }

        // --- ModuleService: 장착/해제/슬롯/hasRun/저장 재로드 ---

        [Test]
        public void Equip_Then_Unequip_UpdatesEquippedList()
        {
            var save = new SaveService(dir);
            var service = new ModuleService(save);
            var module = MakeModule("Module_FirstStrike", ModuleKind.FirstStrike);

            Assert.IsFalse(service.IsEquipped(module));
            Assert.IsTrue(service.TryEquip(module));
            Assert.IsTrue(service.IsEquipped(module));
            Assert.IsTrue(service.TryUnequip(module));
            Assert.IsFalse(service.IsEquipped(module));
        }

        [Test]
        public void TryEquip_FailsWhenSlotsFull()
        {
            var save = new SaveService(dir);
            var service = new ModuleService(save);
            var a = MakeModule("Module_A", ModuleKind.FirstStrike);
            var b = MakeModule("Module_B", ModuleKind.RepeatTurn);

            Assert.IsTrue(service.TryEquip(a)); // 슬롯 1개 채움
            Assert.IsFalse(service.CanEquip(b));
            Assert.IsFalse(service.TryEquip(b));
            Assert.IsFalse(service.IsEquipped(b));
        }

        [Test]
        public void EquipUnequip_FailWhenHasRun()
        {
            var save = new SaveService(dir);
            save.Data.hasRun = true;
            var service = new ModuleService(save);
            var module = MakeModule("Module_FirstStrike", ModuleKind.FirstStrike);

            Assert.IsFalse(service.TryEquip(module));
            save.Data.hasRun = false;
            Assert.IsTrue(service.TryEquip(module));
            save.Data.hasRun = true;
            Assert.IsFalse(service.TryUnequip(module));
            Assert.IsTrue(service.IsEquipped(module));
        }

        [Test]
        public void EquippedModule_PersistsAcrossReopen()
        {
            var save = new SaveService(dir);
            var service = new ModuleService(save);
            var module = MakeModule("Module_FirstStrike", ModuleKind.FirstStrike);
            Assert.IsTrue(service.TryEquip(module));

            var reopened = new SaveService(dir);
            var reopenedService = new ModuleService(reopened);
            Assert.IsTrue(reopenedService.IsEquipped(module));
        }

        // --- 선제 타격: 장착 시 발동, 미장착 시 미발동 ---

        [Test]
        public void FirstStrike_Equipped_DamagesFirstEnemy_OnBattleStart_EnergyUnchanged()
        {
            var save = new SaveService(dir);
            var service = new ModuleService(save);
            var module = MakeModule("Module_FirstStrike", ModuleKind.FirstStrike);
            service.TryEquip(module);

            var character = MakeCharacter();
            var state = NewBattle(character, 20);
            var effects = ModuleService.AttachEquipped(new[] { module }, save, state.Events, state, character);
            Assert.AreEqual(1, effects.Count);

            state.StartBattle();

            Assert.AreEqual(15, state.Enemies[0].Hp); // 20 - attack(5)
            Assert.AreEqual(0, state.Energy.Current); // 에너지 소모 없음

            foreach (var e in effects) e.Detach();
        }

        [Test]
        public void FirstStrike_NotEquipped_DoesNothing_OnBattleStart()
        {
            var save = new SaveService(dir);
            var module = MakeModule("Module_FirstStrike", ModuleKind.FirstStrike);
            // 장착하지 않음

            var character = MakeCharacter();
            var state = NewBattle(character, 20);
            var effects = ModuleService.AttachEquipped(new[] { module }, save, state.Events, state, character);
            Assert.AreEqual(0, effects.Count); // 미장착이면 Attach 자체가 안 된다

            state.StartBattle();

            Assert.AreEqual(20, state.Enemies[0].Hp);
        }

        // --- 반복 턴: 장착 시 3턴마다 추가 페이즈, 미장착 시 항상 false ---

        [Test]
        public void RepeatTurn_Equipped_GrantsExtraPhaseEveryThirdTurn()
        {
            var save = new SaveService(dir);
            var service = new ModuleService(save);
            var module = MakeModule("Module_RepeatTurn", ModuleKind.RepeatTurn, 3);
            service.TryEquip(module);

            var character = MakeCharacter();
            var state = NewBattle(character, 20);
            var effects = ModuleService.AttachEquipped(new[] { module }, save, state.Events, state, character);

            state.StartBattle(); // turn 1
            Assert.IsFalse(state.ConsumeExtraCommandPhase());

            AdvanceTurn(state); // turn 2
            Assert.IsFalse(state.ConsumeExtraCommandPhase());

            AdvanceTurn(state); // turn 3
            Assert.IsTrue(state.ConsumeExtraCommandPhase());
            Assert.IsFalse(state.ConsumeExtraCommandPhase()); // 한 번 소비하면 다시 false

            foreach (var e in effects) e.Detach();
        }

        [Test]
        public void RepeatTurn_NotEquipped_NeverGrantsExtraPhase()
        {
            var save = new SaveService(dir);
            var module = MakeModule("Module_RepeatTurn", ModuleKind.RepeatTurn, 3);
            // 장착하지 않음

            var character = MakeCharacter();
            var state = NewBattle(character, 20);
            ModuleService.AttachEquipped(new[] { module }, save, state.Events, state, character);

            state.StartBattle();
            Assert.IsFalse(state.ConsumeExtraCommandPhase());
            AdvanceTurn(state);
            AdvanceTurn(state);
            Assert.IsFalse(state.ConsumeExtraCommandPhase());
        }

        private static void AdvanceTurn(BattleState state)
        {
            state.BeginCommandPhase();
            state.EndPlayerPhase();
            for (int i = 0; i < state.Intents.Count; i++) state.ResolveEnemyIntent(i, true);
            state.EndTurn();
            state.BeginTurn();
        }
    }
}
