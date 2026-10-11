using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 모든 미니게임 매니저가 상속하는 베이스. 호스트(MiniGameRunner)가 있으면
    /// 호스트가 Begin 을 호출하고, 단독 실행(씬을 바로 Play)이면 한 프레임 뒤
    /// 스스로 기본값으로 Begin 한다.
    /// 파생 클래스는 Start 를 선언하지 말고 OnBegin 에서 시작 처리를 한다.
    /// </summary>
    public abstract class MiniGameBase : MonoBehaviour
    {
        public event Action<MiniGameResult> Finished;

        public bool IsHosted { get; set; }

        protected MiniGameContext Context { get; private set; }

        /// <summary>Begin 이후 결과를 Report 하기 전까지 true. 입력 처리는 이 값으로 막는다.</summary>
        protected bool IsRunning { get; private set; }

        private bool reported;
        private bool begun;
        private readonly List<IMiniGameCondition> conditions = new List<IMiniGameCondition>();

        /// <summary>결과 연출에 쓸 Animator. 파생 클래스가 오버라이드한다.</summary>
        protected virtual Animator ResultAnimator => null;

        public void Begin(MiniGameContext ctx)
        {
            if (begun)
            {
                return;
            }
            begun = true;
            Context = ctx;
            IsRunning = true;
            OnBegin(ctx);
        }

        protected abstract void OnBegin(MiniGameContext ctx);

        /// <summary>
        /// 판정 조건을 등록한다. 등록 순서가 우선순위다(먼저 등록한 조건이 먼저 확인돼야 하면
        /// 먼저 Watch 한다. 예: Fail 조건을 Success 조건보다 먼저 Watch).
        /// </summary>
        protected void Watch(IMiniGameCondition condition)
        {
            conditions.Add(condition);
            condition.Met += OnConditionMet;
            condition.Arm(this);
        }

        private void OnConditionMet(IMiniGameCondition condition)
        {
            Resolve(condition.Result);
        }

        /// <summary>조건 충족 시 호출: 모든 조건을 해제하고 결과 연출 후 Report 한다.</summary>
        protected void Resolve(MiniGameResult result)
        {
            if (!IsRunning)
            {
                return;
            }
            foreach (IMiniGameCondition condition in conditions)
            {
                condition.Met -= OnConditionMet;
                condition.Disarm();
            }
            OnResolved(result);
            Report(result);
        }

        /// <summary>결과가 정해졌을 때의 연출. 기본은 ResultAnimator 로 isSuccess/isFailed 를 켠다.</summary>
        protected virtual void OnResolved(MiniGameResult result)
        {
            PlayResultAnim(ResultAnimator, result);
        }

        protected void Report(MiniGameResult result)
        {
            if (reported)
            {
                return;
            }
            reported = true;
            IsRunning = false;
            Finished?.Invoke(result);
        }

        /// <summary>
        /// 결과 연출: Success → isSuccess, Failed → isFailed bool 을 켠다.
        /// Animator 가 없거나 파라미터가 없으면 무시한다.
        /// </summary>
        protected static void PlayResultAnim(Animator animator, MiniGameResult result)
        {
            if (animator == null)
            {
                return;
            }
            string param = result == MiniGameResult.Success ? "isSuccess" : "isFailed";
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                if (p.name == param && p.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(param, true);
                    return;
                }
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            // Runner 가 있으면 Runner 가 지시문 표시 후 Begin(ctx) 를 부른다.
            // (IsHosted 는 지시문 대기 뒤에 켜지므로 그것만 보면 기본값으로 먼저 시작해 ctx 가 무시된다.)
            if (!IsHosted && MiniGameRunner.Instance == null)
            {
                Begin(MiniGameContext.Default);
            }
        }
    }
}
