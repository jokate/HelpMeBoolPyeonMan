using BoolPyeon.Battle;

namespace BoolPyeon.Meta
{
    // F-28: 반복 턴 — turn % interval == 0 인 턴에 추가 커맨드 페이즈 1회를 부여한다.
    // 에너지를 다시 모으지 않는다(남은 에너지로만 행동). interval 은 ModuleDefinition.param(기본 3).
    public class RepeatTurnEffect : IModuleEffect
    {
        private readonly int interval;
        private BattleEvents events;
        private BattleState state;

        public RepeatTurnEffect(int interval)
        {
            this.interval = interval > 0 ? interval : 3;
        }

        public void Attach(BattleEvents events, BattleState state, CharacterDefinition character)
        {
            this.events = events;
            this.state = state;
            if (events != null) events.OnTurnStart += OnTurnStart;
        }

        public void Detach()
        {
            if (events != null) events.OnTurnStart -= OnTurnStart;
            events = null;
            state = null;
        }

        private void OnTurnStart(int turn)
        {
            if (state == null) return;
            if (turn % interval == 0) state.GrantExtraCommandPhase();
        }
    }
}
