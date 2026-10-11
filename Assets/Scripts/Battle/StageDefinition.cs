using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // T1: 스테이지 하나(방 목록·보스·몬스터 배율)를 묶는다. BattleController.stages 에서
    // RunState.StageNumber 에 맞는 항목을 골라 미로를 만든다. 맞는 항목이 없으면 BattleController 의
    // 기존 combatRooms/eliteRooms/bossRoom 필드로 대신한다.
    [CreateAssetMenu(fileName = "Stage_", menuName = "BoolPyeon/Battle/Stage")]
    public class StageDefinition : ScriptableObject
    {
        public int stageNumber = 1;
        public string displayName;
        public List<RoomDefinition> combatRooms = new List<RoomDefinition>();
        public List<RoomDefinition> eliteRooms = new List<RoomDefinition>();
        public RoomDefinition bossRoom;
        public float monsterHpMultiplier = 1f;
        public float monsterAttackMultiplier = 1f;
    }
}
