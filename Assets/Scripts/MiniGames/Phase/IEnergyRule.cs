namespace BoolPyeon.MiniGames.Phase
{
    // F-30: 스쿼드 편성에 따라 성공 시 얻는 에너지·로그 표시 이름을 바꿔 끼우는 규칙.
    // null 이면 MiniGamePhaseState 가 기존(레거시) 계산을 그대로 쓴다.
    public interface IEnergyRule
    {
        // 성공 1회당 얻는 에너지. combo 는 이번 성공을 반영한 뒤의 콤보 수(1부터).
        int OnSuccess(MiniGameDefinition def, int combo);

        // 로그에 찍을 담당자 표시 이름 (예: "cook" 또는 "hero").
        string OwnerLabel(MiniGameDefinition def);
    }
}
