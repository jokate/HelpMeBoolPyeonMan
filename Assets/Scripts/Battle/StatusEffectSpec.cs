using System;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // 데이터로 적는 상태이상 한 개 (몬스터 행동이 방어 실패 시 부여)
    [Serializable]
    public class StatusEffectSpec
    {
        public StatusEffectType type = StatusEffectType.DamageTakenUp;
        [Min(0)] public int turns = 1;
        public float magnitude = StatusEffect.DefaultDamageTakenUpMagnitude;
    }
}
