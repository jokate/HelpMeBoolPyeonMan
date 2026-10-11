using NUnit.Framework;

namespace BoolPyeon.Battle.Tests
{
    public class CombatantTests
    {
        [Test]
        public void TakeDamage_SubtractsDefense()
        {
            var c = new Combatant("A", 20, 0, 2);
            Assert.AreEqual(3, c.TakeDamage(5));
            Assert.AreEqual(17, c.Hp);
        }

        [Test]
        public void TakeDamage_MinimumOne()
        {
            var c = new Combatant("A", 20, 0, 10);
            Assert.AreEqual(1, c.TakeDamage(3));
        }

        [Test]
        public void TakeDamage_GuardHalvesRoundedUp()
        {
            var c = new Combatant("A", 20, 0, 0);
            c.SetGuard(true);
            Assert.AreEqual(3, c.TakeDamage(5));
        }

        [Test]
        public void TakeDamage_DamageTakenUpThenGuard()
        {
            var c = new Combatant("A", 20, 0, 0);
            c.AddStatus(StatusEffectType.DamageTakenUp, 2, 0.5f);
            Assert.AreEqual(6, c.CalculateDamage(4));
            c.SetGuard(true);
            Assert.AreEqual(3, c.CalculateDamage(4));
        }

        [Test]
        public void TakeDamage_HpFloorsAtZero()
        {
            var c = new Combatant("A", 5, 0, 0);
            Assert.AreEqual(5, c.TakeDamage(100));
            Assert.AreEqual(0, c.Hp);
            Assert.IsTrue(c.IsDead);
            Assert.AreEqual(0, c.TakeDamage(3));
        }

        [Test]
        public void TakeDamage_RaisesDamagedEvent()
        {
            var c = new Combatant("A", 10, 0, 0);
            int got = -1;
            c.Damaged += (_, d) => got = d;
            c.TakeDamage(4);
            Assert.AreEqual(4, got);
        }

        [Test]
        public void Heal_CapsAtMaxAndIgnoresDead()
        {
            var c = new Combatant("A", 10, 0, 0);
            c.TakeDamage(3);
            Assert.AreEqual(3, c.Heal(10));
            Assert.AreEqual(10, c.Hp);
            c.TakeDamage(100);
            Assert.AreEqual(0, c.Heal(5));
            Assert.AreEqual(0, c.Hp);
        }

        [Test]
        public void AddStatus_SameTypeKeepsMaxTurnsAndOverwritesMagnitude()
        {
            var c = new Combatant("A", 10, 0, 0);
            c.AddStatus(StatusEffectType.DamageTakenUp, 3, 0.5f);
            c.AddStatus(StatusEffectType.DamageTakenUp, 1, 1f);
            Assert.AreEqual(1, c.Statuses.Count);
            Assert.AreEqual(3, c.Statuses[0].RemainingTurns);
            Assert.AreEqual(1f, c.Statuses[0].Magnitude);
        }

        [Test]
        public void EndTurn_TicksStatusesAndClearsGuard()
        {
            var c = new Combatant("A", 10, 0, 0);
            c.AddStatus(StatusEffectType.Stun, 1, 0f);
            c.AddStatus(StatusEffectType.NoEnergyReserve, 2, 0f);
            c.SetGuard(true);
            c.EndTurn();
            Assert.IsFalse(c.HasStatus(StatusEffectType.Stun));
            Assert.IsTrue(c.HasStatus(StatusEffectType.NoEnergyReserve));
            Assert.IsFalse(c.IsGuarding);
            c.EndTurn();
            Assert.AreEqual(0, c.Statuses.Count);
        }
    }
}
