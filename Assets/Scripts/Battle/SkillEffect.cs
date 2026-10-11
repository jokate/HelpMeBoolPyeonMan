using System;

namespace BoolPyeon.Battle
{
    [Serializable]
    public class SkillEffect
    {
        public SkillEffectKind kind = SkillEffectKind.Damage;
        public int amount;
        public StatusEffectType status;
        public int statusTurns = 1;
        public float statusMagnitude = StatusEffect.DefaultDamageTakenUpMagnitude;
    }
}
