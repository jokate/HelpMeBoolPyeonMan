using System;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 누르고 있기형 입력. 매니저의 Update 에서 Tick 을 부르고 IsHolding 을 읽는다.
    /// </summary>
    [Serializable]
    public class HoldInteraction
    {
        public bool IsHolding { get; private set; }

        public void Tick()
        {
            if (PointerInput.Down)
            {
                IsHolding = true;
            }
            else if (PointerInput.Up || !PointerInput.Held)
            {
                // Held 확인은 포커스 손실 등으로 Up 을 놓친 경우 대비
                IsHolding = false;
            }
        }

        public void Reset()
        {
            IsHolding = false;
        }
    }
}
