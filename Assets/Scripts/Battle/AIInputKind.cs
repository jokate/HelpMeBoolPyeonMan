namespace BoolPyeon.Battle
{
    // 적 AI 판단 요소의 입력. 모두 0~1 로 정규화된다.
    public enum AIInputKind
    {
        SelfHpRatio,       // 내 HP / 최대 HP
        TargetHpRatio,     // 대상(주인공) HP / 최대 HP
        TargetHasStatus,   // 대상이 status 상태이상에 걸려 있으면 1, 아니면 0
        TargetGuarding,    // 대상이 방어 중이면 1
        PlayerEnergyRatio, // 에너지 / 최대 에너지
        TurnsSinceUsed     // 이 행동을 마지막으로 쓴 뒤 지난 턴 / normalizeTurns (안 썼으면 1)
    }
}
