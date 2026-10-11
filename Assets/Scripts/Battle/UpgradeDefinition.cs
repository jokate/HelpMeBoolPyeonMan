using UnityEngine;

namespace BoolPyeon.Battle
{
    // F-19: 방 클리어 강화 선택지 하나. RunState 가 이 정의를 읽어 적용한다(원본 SO 는 바뀌지 않는다).
    [CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "BoolPyeon/Battle/Upgrade")]
    public class UpgradeDefinition : ScriptableObject
    {
        public string displayName;
        [TextArea] public string description;
        // 강화 선택 카드 아이콘(없으면 카드에서 숨긴다)
        public Sprite icon;
        public UpgradeKind kind;
        public int value;
        // LearnSkill: 새로 배울 스킬(원본)
        public SkillDefinition skill;
        // SkillPower: 강화할 스킬(원본). null 이면 보유 스킬 0번
        public SkillDefinition targetSkill;
    }
}
