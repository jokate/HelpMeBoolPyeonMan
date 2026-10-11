using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    [CreateAssetMenu(fileName = "MonsterDefinition", menuName = "BoolPyeon/Battle/Monster")]
    public class MonsterDefinition : ScriptableObject
    {
        public string displayName;
        [Min(1)] public int maxHp = 20;
        [Min(0)] public int attack = 3;
        [Min(0)] public int defense;
        public InconvenienceType type;
        [Tooltip("표시용 역할. 로직에는 관여하지 않는다.")]
        public MonsterRole role = MonsterRole.Attacker;

        [Header("View")]
        [Tooltip("T6: 기존 스프라이트에 곱해질 색. 기본값(흰색)이면 원래 색 그대로.")]
        public Color tint = Color.white;
        [Tooltip("T6: 비어 있으면(null) 씬 enemyViews 의 기존 스프라이트를 유지한다.")]
        public Sprite spriteOverride;
        [Tooltip("공격할 때 잠깐 바꿔 보여줄 스프라이트(비우면 전진 동작만)")]
        public Sprite attackSprite;
        [Tooltip("맞았을 때 잠깐 바꿔 보여줄 스프라이트(비우면 흔들림만)")]
        public Sprite hitSprite;

        [Header("AI (비어 있으면 아래 Legacy 순서·주기 방식)")]
        public List<MonsterActionDefinition> actions = new List<MonsterActionDefinition>();
        [Tooltip("0: 항상 최고점 행동, 1: 점수 비율대로 무작위")]
        [Range(0f, 1f)] public float randomness = 1f;

        [Header("Legacy")]
        public List<MonsterActionKind> actionPattern = new List<MonsterActionKind> { MonsterActionKind.Attack };
        [Tooltip("0 이면 특수 공격 없음")]
        [Min(0)] public int specialInterval;
        [Min(0f)] public float specialDamageMultiplier = 2f;

        // MiniGameDefinition.id (asmdef 경계 때문에 문자열로 참조)
        // T6: 일반 공격 회피 미니게임 제거. 데이터 손실 방지를 위해 필드는 남기고 숨긴다.
        [HideInInspector]
        [System.Obsolete("T6: 일반 공격 회피 제거, 데이터 보존용")]
        public List<string> evasionMiniGameIds = new List<string>();
        public List<string> specialMiniGameIds = new List<string>();

        // T8.2: Legacy 전용. actions 가 있는 몬스터는 각 MonsterActionDefinition.statuses 로 패널티를 정하며
        // 이 필드는 무시된다(BattleState.ResolveEnemyIntent 참고). 새 몬스터는 actions[].statuses 를 쓴다.
        [Header("Legacy 전용: actions 가 비어 있을 때만 사용")]
        public StatusEffectType penalty = StatusEffectType.DamageTakenUp;
        [Min(0)] public int penaltyTurns = 1;
        public float penaltyMagnitude = StatusEffect.DefaultDamageTakenUpMagnitude;

        // T11: 보스 전투 기믹. isBoss 가 켜진 몬스터만 BattleState.UpdateBossPhases 의 대상이 된다.
        [Header("Boss")]
        public bool isBoss;
        [Tooltip("0 이하이면 2페이즈 기능이 꺼진다. 현재 HP 비율이 이 값 이하가 되면 한 번만 전환한다.")]
        [Range(0f, 1f)] public float phase2Threshold;
        [Tooltip("2페이즈 전환 후 actions 목록에 더해지는 행동 후보")]
        public List<MonsterActionDefinition> phase2Actions = new List<MonsterActionDefinition>();
    }
}
