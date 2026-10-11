using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    [CreateAssetMenu(fileName = "SkillDefinition", menuName = "BoolPyeon/Battle/Skill")]
    public class SkillDefinition : ScriptableObject
    {
        public string displayName;
        public Sprite icon;
        [Min(0)] public int energyCost = 1;
        public SkillTargetType targetType = SkillTargetType.SingleEnemy;
        public List<SkillEffect> effects = new List<SkillEffect>();
        [Min(0)] public int upgradeLevel;
    }
}
