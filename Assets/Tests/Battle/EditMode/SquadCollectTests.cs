using NUnit.Framework;

namespace BoolPyeon.Battle.Tests
{
    // T4: 멤버별 미니게임 수집 결과가 ActiveMember 의 EnergyPool 에만 들어가는지 검증한다.
    // SquadPoolBuilder/SquadEnergyRule/MiniGamePhaseConfig 는 BattleFlow·MiniGames(Phase) 폴더(기본
    // 어셈블리)에 있어 이 EditMode 테스트 어셈블리(asmdef)에서 참조할 수 없다(Assets/Tests/Meta/EditMode/
    // SquadTests.cs 13행의 선례와 동일한 제약). 해당 수치·풀 선택은 MCP Unity_RunCommand 로 확인했다.
    public class SquadCollectTests
    {
        [Test]
        public void CollectEnergy_OnlyAffectsActiveMember()
        {
            var s = new Squad();
            var a = new Combatant("A", 10, 0, 0);
            var b = new Combatant("B", 10, 0, 0);
            s.TryAdd(null, a);
            s.TryAdd(null, b);
            var state = new BattleState(s, null);

            state.CollectEnergy(2);
            Assert.AreEqual(2, s.Members[0].Energy.Current);
            Assert.AreEqual(0, s.Members[1].Energy.Current);

            state.AdvanceMember();
            state.CollectEnergy(3);
            Assert.AreEqual(2, s.Members[0].Energy.Current);
            Assert.AreEqual(3, s.Members[1].Energy.Current);
        }
    }
}
