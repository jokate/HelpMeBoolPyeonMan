using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // F-17: 방 하나 = 웨이브 연속(잡몹→잡몹→중간보스 등). BattleController.room 에 연결한다.
    [CreateAssetMenu(fileName = "RoomDefinition", menuName = "BoolPyeon/Battle/Room")]
    public class RoomDefinition : ScriptableObject
    {
        public string displayName;
        public RoomType roomType;
        public List<WaveDefinition> waves = new List<WaveDefinition>();
    }
}
