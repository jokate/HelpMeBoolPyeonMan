using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class RoomProgressTests
    {
        readonly List<Object> created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in created) Object.DestroyImmediate(o);
            created.Clear();
        }

        MonsterDefinition Monster(int hp = 8, int attack = 3)
        {
            var m = ScriptableObject.CreateInstance<MonsterDefinition>();
            m.displayName = "M";
            m.maxHp = hp;
            m.attack = attack;
            created.Add(m);
            return m;
        }

        WaveDefinition Wave(params MonsterDefinition[] monsters)
        {
            var w = ScriptableObject.CreateInstance<WaveDefinition>();
            w.monsters = new List<MonsterDefinition>(monsters);
            created.Add(w);
            return w;
        }

        RoomDefinition Room(params WaveDefinition[] waves)
        {
            var r = ScriptableObject.CreateInstance<RoomDefinition>();
            r.displayName = "Room";
            r.roomType = RoomType.Combat;
            r.waves = new List<WaveDefinition>(waves);
            created.Add(r);
            return r;
        }

        [Test]
        public void ThreeWaves_DefeatAll_ClearsRoomInOrder()
        {
            var room = Room(Wave(Monster()), Wave(Monster()), Wave(Monster()));
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 30, 3, 0));
            var progress = new RoomProgress(room, squad, 1);

            var s0 = progress.StartNextWave();
            Assert.AreEqual(0, progress.WaveIndex);
            s0.StartBattle();
            s0.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();
            Assert.IsFalse(progress.IsCleared);

            var s1 = progress.StartNextWave();
            Assert.AreEqual(1, progress.WaveIndex);
            s1.StartBattle();
            s1.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();
            Assert.IsFalse(progress.IsCleared);

            var s2 = progress.StartNextWave();
            Assert.AreEqual(2, progress.WaveIndex);
            s2.StartBattle();
            s2.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();

            Assert.IsTrue(progress.IsCleared);
            Assert.IsFalse(progress.IsFailed);
        }

        [Test]
        public void Squad_KeepsHpAcrossWaves()
        {
            var room = Room(Wave(Monster(attack: 4)), Wave(Monster(attack: 0)));
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 30, 3, 0));
            var progress = new RoomProgress(room, squad, 1);

            var s0 = progress.StartNextWave();
            s0.StartBattle();
            s0.BeginCommandPhase();
            s0.EndPlayerPhase();
            for (int i = 0; i < s0.Intents.Count; i++) s0.ResolveEnemyIntent(i);
            int hpAfterHit = s0.Player.Hp;
            Assert.Less(hpAfterHit, 30);
            s0.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();

            var s1 = progress.StartNextWave();
            Assert.AreEqual(hpAfterHit, s1.Player.Hp);
        }

        // T9: 멤버 여러 명(3명)의 HP 가 웨이브를 건너 각자 독립적으로 유지되는지 확인한다.
        [Test]
        public void Squad_KeepsPerMemberHpAcrossWaves()
        {
            var room = Room(Wave(Monster()), Wave(Monster()));
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("A", 30, 3, 0));
            squad.TryAdd(null, new Combatant("B", 30, 3, 0));
            squad.TryAdd(null, new Combatant("C", 30, 3, 0));
            var progress = new RoomProgress(room, squad, 1);

            var s0 = progress.StartNextWave();
            s0.StartBattle();
            squad.Members[0].Combatant.TakeDamage(10);
            squad.Members[1].Combatant.TakeDamage(20);
            s0.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();

            var s1 = progress.StartNextWave();
            Assert.AreEqual(20, s1.Members[0].Combatant.Hp);
            Assert.AreEqual(10, s1.Members[1].Combatant.Hp);
            Assert.AreEqual(30, s1.Members[2].Combatant.Hp);
        }

        [Test]
        public void Defeat_MarksFailed_AndStopsBeforeLastWave()
        {
            var room = Room(Wave(Monster()), Wave(Monster(attack: 999)), Wave(Monster()));
            var squad = new Squad();
            squad.TryAdd(null, new Combatant("P", 5, 3, 0));
            var progress = new RoomProgress(room, squad, 1);

            var s0 = progress.StartNextWave();
            s0.StartBattle();
            s0.DebugDefeatAllEnemies();
            progress.CompleteCurrentWave();

            var s1 = progress.StartNextWave();
            Assert.AreEqual(1, progress.WaveIndex);
            s1.StartBattle();
            s1.BeginCommandPhase();
            s1.EndPlayerPhase();
            for (int i = 0; i < s1.Intents.Count; i++) s1.ResolveEnemyIntent(i);
            Assert.IsTrue(s1.Player.IsDead);
            progress.CompleteCurrentWave();

            Assert.IsTrue(progress.IsFailed);
            Assert.IsFalse(progress.IsCleared);
            Assert.IsFalse(progress.HasNextWave);
            var s2 = progress.StartNextWave();
            Assert.IsNull(s2);
        }
    }
}
