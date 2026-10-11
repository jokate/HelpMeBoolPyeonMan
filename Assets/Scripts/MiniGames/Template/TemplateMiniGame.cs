using BoolPyeon.Input;
using UnityEngine;

namespace BoolPyeon.MiniGames.Template
{
    /// <summary>
    /// 새 미니게임의 뼈대. Assets/SampleScript/&lt;Name&gt;MiniGame/&lt;Name&gt;MiniGame.cs 로 복사해
    /// 클래스 이름을 바꾸고 채운다 (Docs/MINIGAME_GUIDE.md 참고).
    ///
    /// 계약 (MiniGameBase):
    /// - Start 를 선언하지 않는다. 단독 실행이면 베이스가 한 프레임 뒤 Begin(Default) 을 부른다.
    /// - 시작 처리는 OnBegin(ctx) 에서. ctx.Difficulty 로 개수·허용 오차, ctx.Speed 로 움직임 속도.
    /// - 입력은 IsRunning 일 때만 받는다. 결과는 Resolve(Success/Failed) 한 번.
    /// - 제한 시간은 Runner 가 관리한다 (단독 실행에서는 시간초과 없음).
    /// </summary>
    public class TemplateMiniGame : MiniGameBase
    {
        [Tooltip("결과 연출 Animator (bool isSuccess / isFailed). 비워 두면 연출 없이 결과만 보고한다.")]
        public Animator resultAnimator;

        [Tooltip("움직임 기본 속도(월드 단위/초). 실제 속도 = baseSpeed × ctx.Speed")]
        public float baseSpeed = 1.5f;

        private float speed;

        protected override Animator ResultAnimator => resultAnimator;

        protected override void OnBegin(MiniGameContext ctx)
        {
            // 난이도: 개수·허용 오차는 Difficulty 로 (정의의 min~maxDifficulty 범위).
            int difficulty = Mathf.Max(1, ctx.Difficulty);
            // 속도: 움직임은 deltaTime × Speed.
            speed = baseSpeed * Mathf.Max(0.1f, ctx.Speed);

            // TODO: difficulty 에 따라 대상 생성·배치.
            // 조건 방식 예 (코루틴·트리거로 판정할 때):
            //   Watch(new PredicateCondition(MiniGameResult.Failed, () => failed));  // 실패를 먼저 등록
            //   Watch(goalTrigger);  // public TriggerTagCondition 필드(씬의 트리거 콜라이더, ClearObject 도달 → 성공)
            _ = difficulty;
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            // TODO: 움직임은 Time.deltaTime * speed 로.
            _ = speed;

            if (!PointerInput.Down)
            {
                return;
            }
            RaycastHit2D hit = PointerInput.RaycastWorld2D();
            if (hit.collider == null)
            {
                return;
            }
            // TODO: 판정. 예: 목표(태그 ClearObject)를 누르면 성공.
            if (hit.collider.CompareTag("ClearObject"))
            {
                Resolve(MiniGameResult.Success);
            }
        }

        // 결과 연출을 더하려면 오버라이드한다 (움직임 정지 등). base 호출이 Animator bool 을 켠다.
        protected override void OnResolved(MiniGameResult result)
        {
            base.OnResolved(result);
        }
    }
}
