using System;
using UnityEngine;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 태그가 붙은 콜라이더가 트리거로 들어오면 Met 을 발화하는 조건. "통과 지점" 판정에
    /// 재사용한다(예: PizzaChecker). Collider2D 는 트리거로 설정돼 있어야 한다.
    /// </summary>
    public class TriggerTagCondition : MonoBehaviour, IMiniGameCondition
    {
        [SerializeField] private string targetTag = "ClearObject";
        [SerializeField] private MiniGameResult result = MiniGameResult.Success;

        public MiniGameResult Result => result;

        public event Action<IMiniGameCondition> Met;

        private bool armed;
        private bool fired;

        public void Arm(MonoBehaviour host)
        {
            armed = true;
        }

        public void Disarm()
        {
            armed = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!armed || fired)
            {
                return;
            }
            if (collision.CompareTag(targetTag))
            {
                fired = true;
                Met?.Invoke(this);
            }
        }
    }
}
