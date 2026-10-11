using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class EnergyPoolTests
    {
        [Test]
        public void Add_ClampsToCap()
        {
            var e = new EnergyPool();
            e.Add(7);
            e.Add(7);
            Assert.AreEqual(10, e.Current);
        }

        [Test]
        public void TrySpend_FailsWithoutChangeWhenInsufficient()
        {
            var e = new EnergyPool();
            e.Add(2);
            Assert.IsFalse(e.TrySpend(3));
            Assert.AreEqual(2, e.Current);
            Assert.IsTrue(e.TrySpend(2));
            Assert.AreEqual(0, e.Current);
        }

        // T2: 멤버마다 독립 풀이고, 상한은 Squad 가 넘기는 memberEnergyCap 을 따른다.
        [Test]
        public void Squad_MembersEach_HaveOwnCap()
        {
            var s = new Squad(EnergyPool.DefaultCap + 3);
            var a = new Combatant("A", 10, 0, 0);
            s.TryAdd(null, a);

            Assert.AreEqual(EnergyPool.DefaultCap + 3, s.Members[0].Energy.Cap);
        }

        // T2: RunState.CreateSquad 는 StartEnergyBonus 를 편성 전원에게 채운다.
        [Test]
        public void RunState_CreateSquad_FillsStartEnergyForEveryMember()
        {
            var a = ScriptableObject.CreateInstance<CharacterDefinition>();
            a.displayName = "A"; a.maxHp = 10; a.attack = 1; a.defense = 0;
            a.skills = new System.Collections.Generic.List<SkillDefinition>();
            var b = ScriptableObject.CreateInstance<CharacterDefinition>();
            b.displayName = "B"; b.maxHp = 10; b.attack = 1; b.defense = 0;
            b.skills = new System.Collections.Generic.List<SkillDefinition>();

            var run = new RunState(new System.Collections.Generic.List<CharacterDefinition> { a, b });
            run.ApplyPermanent(PermanentUpgradeKind.StartEnergy, 3);

            var squad = run.CreateSquad();

            Assert.AreEqual(3, squad.Members[0].Energy.Current);
            Assert.AreEqual(3, squad.Members[1].Energy.Current);

            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }
    }
}
