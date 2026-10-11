namespace BoolPyeon.Battle
{
    // T7: 상태이상 한 줄 설명
    public static class StatusEffectText
    {
        public static string Describe(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Stun: return "커맨드를 쓸 수 없음";
                case StatusEffectType.NoEnergyReserve: return "다음 에너지 수집 페이즈 건너뜀";
                case StatusEffectType.DamageTakenUp: return "받는 피해 증가";
                case StatusEffectType.Weaken: return "주는 피해 감소";
                case StatusEffectType.Rush: return "다음 에너지 수집 제한시간 ×0.7";
                case StatusEffectType.CostUp: return "커맨드 코스트 +1";
                case StatusEffectType.Burn: return "턴 시작 시 지속 피해";
                default: return type.ToString();
            }
        }
    }
}
