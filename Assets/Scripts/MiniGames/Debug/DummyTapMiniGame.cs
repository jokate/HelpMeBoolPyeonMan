using BoolPyeon.Input;

namespace BoolPyeon.MiniGames.Debug
{
    /// <summary>
    /// 프레임워크 배선 확인용 더미 미니게임. 화면을 탭하면 바로 Success 처리한다.
    /// </summary>
    public class DummyTapMiniGame : MiniGameBase
    {
        protected override void OnBegin(MiniGameContext ctx)
        {
        }

        private void Update()
        {
            if (IsRunning && PointerInput.Down)
            {
                Resolve(MiniGameResult.Success);
            }
        }
    }
}
