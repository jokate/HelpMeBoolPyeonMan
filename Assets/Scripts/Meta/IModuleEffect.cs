using BoolPyeon.Battle;

namespace BoolPyeon.Meta
{
    // F-28: 전투 이벤트 훅에 붙는 특수 모듈 효과 하나. MonoBehaviour 가 아닌 순수 C# —
    // BattleController 가 웨이브 State 를 만들 때 Attach, 끝나면 Detach 한다.
    public interface IModuleEffect
    {
        void Attach(BattleEvents events, BattleState state, CharacterDefinition character);
        void Detach();
    }
}
