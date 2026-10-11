using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // 몬스터 행동 하나의 데이터. EnemyAI 가 점수를 매겨 확률로 고른다.
    [Serializable]
    public class MonsterActionDefinition
    {
        public string id;
        public MonsterActionKind kind = MonsterActionKind.Attack;
        [Min(0f)] public float weight = 1f;
        [Min(0f)] public float damageMultiplier = 1f;
        [Tooltip("사용 후 이 턴 수 동안 다시 고르지 않는다")]
        [Min(0)] public int cooldownTurns;
        [Tooltip("Special: 방어 미니게임 실패 시 다음 턴에 부여")]
        public List<StatusEffectSpec> statuses = new List<StatusEffectSpec>();
        [Tooltip("Special/Magic: 방어·회피 미니게임 후보 (MiniGameDefinition.id). 비면 몬스터 설정을 쓴다")]
        public List<string> miniGameIds = new List<string>();
        [Tooltip("Guard/Magic/Buff/Heal: false 면 자신, true 면 Guard/Buff 는 아군 전체, Heal 은 체력비율이 가장 낮은 아군")]
        public bool targetsAllies;
        [Tooltip("Guard/Buff: 효과 지속 턴 수(0 이면 기본 1턴)")]
        [Min(0)] public int effectTurns = 1;
        [Tooltip("Guard/Buff: 효과 크기(0 이면 기본값). Guard 기본 0.5, Buff 기본 0.3")]
        public float effectMagnitude;
        public List<AIConsideration> considerations = new List<AIConsideration>();
    }
}
