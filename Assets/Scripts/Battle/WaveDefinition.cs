using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // F-17: 방 하나의 웨이브 한 판. RoomDefinition 이 여러 개를 참조한다.
    [CreateAssetMenu(fileName = "WaveDefinition", menuName = "BoolPyeon/Battle/Wave")]
    public class WaveDefinition : ScriptableObject
    {
        public List<MonsterDefinition> monsters = new List<MonsterDefinition>();
    }
}
