using System;

namespace BoolPyeon.Battle
{
    [Serializable]
    public class StatusEffect
    {
        public const float DefaultDamageTakenUpMagnitude = 0.5f;

        public StatusEffectType Type;
        public int RemainingTurns;
        public float Magnitude;

        public StatusEffect(StatusEffectType type, int turns, float magnitude)
        {
            Type = type;
            RemainingTurns = turns;
            Magnitude = magnitude;
        }

        public bool IsExpired => RemainingTurns <= 0;

        public void Tick()
        {
            if (RemainingTurns > 0)
                RemainingTurns--;
        }
    }
}
