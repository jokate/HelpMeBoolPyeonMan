using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    [CreateAssetMenu(fileName = "CharacterDefinition", menuName = "BoolPyeon/Battle/Character")]
    public class CharacterDefinition : ScriptableObject
    {
        public const int MaxSkills = 6;

        // F-30: 스쿼드 편성·저장에 쓰는 고정 id (SaveData.squadIds 에 저장). 비우면 안 된다.
        public string id;
        public string displayName;
        [Min(1)] public int maxHp = 30;
        [Min(0)] public int attack = 3;
        [Min(0)] public int defense;
        public InconvenienceType type;
        public CharacterTrait trait;

        [Header("View (전투 화면)")]
        [Tooltip("서 있는 스프라이트. 비우면 씬에 놓인 스프라이트를 그대로 쓴다.")]
        public Sprite standSprite;
        public Sprite[] attackFrames;
        public Sprite[] defendFrames;
        [Tooltip("피격 1장")]
        public Sprite hitSprite;
        [Tooltip("스킬 3프레임")]
        public Sprite[] skillFrames;

        public SkillDefinition attackSkill;
        public SkillDefinition guardSkill;
        public List<SkillDefinition> skills = new List<SkillDefinition>();

        void OnValidate()
        {
            if (skills != null && skills.Count > MaxSkills)
                skills.RemoveRange(MaxSkills, skills.Count - MaxSkills);
        }
    }
}
