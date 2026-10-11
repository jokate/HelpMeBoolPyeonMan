using NUnit.Framework;

namespace BoolPyeon.Battle.Tests
{
    public class SquadTests
    {
        [Test]
        public void TryAdd_RejectsFourthMember()
        {
            var s = new Squad();
            for (int i = 0; i < Squad.MaxMembers; i++)
                Assert.IsTrue(s.TryAdd(null, new Combatant("C" + i, 10, 0, 0)));
            Assert.IsFalse(s.TryAdd(null, new Combatant("X", 10, 0, 0)));
            Assert.AreEqual(3, s.Members.Count);
        }

        [Test]
        public void Leader_IsFirstAlive_AndDefeatedWhenAllDead()
        {
            var s = new Squad();
            var a = new Combatant("A", 1, 0, 0);
            var b = new Combatant("B", 1, 0, 0);
            s.TryAdd(null, a);
            s.TryAdd(null, b);
            Assert.AreSame(a, s.Leader.Combatant);
            a.TakeDamage(5);
            Assert.AreSame(b, s.Leader.Combatant);
            Assert.IsFalse(s.IsDefeated);
            b.TakeDamage(5);
            Assert.IsNull(s.Leader);
            Assert.IsTrue(s.IsDefeated);
        }

        [Test]
        public void Energy_IsPerMember()
        {
            var s = new Squad();
            var a = new Combatant("A", 10, 0, 0);
            var b = new Combatant("B", 10, 0, 0);
            s.TryAdd(null, a);
            s.TryAdd(null, b);

            s.Members[0].Energy.Add(4);

            Assert.AreEqual(4, s.Members[0].Energy.Current);
            Assert.AreEqual(0, s.Members[1].Energy.Current);
            Assert.AreEqual(EnergyPool.DefaultCap, s.Members[0].Energy.Cap);
            Assert.AreEqual(4, s.TotalEnergy);
        }
    }
}
