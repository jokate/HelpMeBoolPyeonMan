using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class SkillResolverTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        SkillDefinition Skill(int cost, SkillTargetType target, params SkillEffect[] effects)
        {
            var s = ScriptableObject.CreateInstance<SkillDefinition>();
            s.energyCost = cost;
            s.targetType = target;
            s.effects = new List<SkillEffect>(effects);
            created.Add(s);
            return s;
        }

        static SkillEffect Damage(int amount) => new SkillEffect { kind = SkillEffectKind.Damage, amount = amount };

        [Test]
        public void Attack_SpendsEnergyAndDamagesFirstAliveEnemy()
        {
            var user = new Combatant("P", 10, 2, 0);
            var dead = new Combatant("E0", 1, 0, 0);
            dead.TakeDamage(5);
            var enemy = new Combatant("E1", 20, 0, 0);
            var energy = new EnergyPool();
            energy.Add(3);
            var events = new BattleEvents();
            int dealt = 0;
            events.OnDamageDealt += (_, d) => dealt = d;

            Assert.IsTrue(SkillResolver.TryUse(Skill(1, SkillTargetType.SingleEnemy, Damage(3)),
                user, energy, new[] { dead, enemy }, events));
            Assert.AreEqual(2, energy.Current);
            Assert.AreEqual(15, enemy.Hp);
            Assert.AreEqual(5, dealt);
        }

        [Test]
        public void TryUse_FailsWhenInsufficientEnergy()
        {
            var user = new Combatant("P", 10, 0, 0);
            var enemy = new Combatant("E", 10, 0, 0);
            var energy = new EnergyPool();
            energy.Add(1);
            Assert.IsFalse(SkillResolver.TryUse(Skill(2, SkillTargetType.SingleEnemy, Damage(3)),
                user, energy, new[] { enemy }));
            Assert.AreEqual(1, energy.Current);
            Assert.AreEqual(10, enemy.Hp);
        }

        [Test]
        public void TryUse_FailsWhenStunned()
        {
            var user = new Combatant("P", 10, 0, 0);
            user.AddStatus(StatusEffectType.Stun, 1, 0f);
            var energy = new EnergyPool();
            energy.Add(5);
            Assert.IsFalse(SkillResolver.TryUse(Skill(1, SkillTargetType.SingleEnemy, Damage(3)),
                user, energy, new[] { new Combatant("E", 10, 0, 0) }));
            Assert.AreEqual(5, energy.Current);
        }

        [Test]
        public void AllEnemies_HitsEveryAliveEnemy()
        {
            var user = new Combatant("P", 10, 0, 0);
            var a = new Combatant("A", 10, 0, 0);
            var b = new Combatant("B", 10, 0, 0);
            var energy = new EnergyPool();
            energy.Add(3);
            Assert.IsTrue(SkillResolver.TryUse(Skill(2, SkillTargetType.AllEnemies, Damage(4)),
                user, energy, new[] { a, b }));
            Assert.AreEqual(6, a.Hp);
            Assert.AreEqual(6, b.Hp);
        }

        [Test]
        public void Guard_SelfSetsGuarding()
        {
            var user = new Combatant("P", 10, 0, 0);
            var energy = new EnergyPool();
            energy.Add(1);
            var guard = Skill(1, SkillTargetType.Self, new SkillEffect { kind = SkillEffectKind.Guard });
            Assert.IsTrue(SkillResolver.TryUse(guard, user, energy, new List<Combatant>()));
            Assert.IsTrue(user.IsGuarding);
        }

        [Test]
        public void ExplicitTarget_IsUsedForSingleEnemy()
        {
            var user = new Combatant("P", 10, 0, 0);
            var a = new Combatant("A", 10, 0, 0);
            var b = new Combatant("B", 10, 0, 0);
            var energy = new EnergyPool();
            energy.Add(1);
            Assert.IsTrue(SkillResolver.TryUse(Skill(1, SkillTargetType.SingleEnemy, Damage(3)),
                user, energy, new[] { a, b }, b));
            Assert.AreEqual(10, a.Hp);
            Assert.AreEqual(7, b.Hp);
        }

        [Test]
        public void ApplyStatus_AddsStatusToTarget()
        {
            var user = new Combatant("P", 10, 0, 0);
            var enemy = new Combatant("E", 10, 0, 0);
            var energy = new EnergyPool();
            energy.Add(1);
            var skill = Skill(1, SkillTargetType.SingleEnemy, new SkillEffect
            {
                kind = SkillEffectKind.ApplyStatus, status = StatusEffectType.DamageTakenUp,
                statusTurns = 2, statusMagnitude = 0.5f
            });
            Assert.IsTrue(SkillResolver.TryUse(skill, user, energy, new[] { enemy }));
            Assert.IsTrue(enemy.HasStatus(StatusEffectType.DamageTakenUp));
        }
    }
}
