using System;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 미니게임 성공·실패 판정 한 가지를 표현한다. MiniGameBase.Watch 로 등록하면
    /// Arm 이 호출되고, 조건이 충족되면 Met 이벤트가 1회 발화된다.
    /// </summary>
    public interface IMiniGameCondition
    {
        MiniGameResult Result { get; }

        event Action<IMiniGameCondition> Met;

        void Arm(UnityEngine.MonoBehaviour host);

        void Disarm();
    }
}
