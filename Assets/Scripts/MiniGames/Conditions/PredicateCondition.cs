using System;
using System.Collections;
using UnityEngine;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 순수 C# 조건. host 의 코루틴으로 WaitUntil 폴링하다가 predicate 가 true 가 되면
    /// Met 을 1회 발화한다. 기존 게임들의 "플래그 대기" 판정을 대체한다.
    /// </summary>
    public class PredicateCondition : IMiniGameCondition
    {
        public MiniGameResult Result { get; }

        public event Action<IMiniGameCondition> Met;

        private readonly Func<bool> predicate;
        private MonoBehaviour host;
        private Coroutine routine;
        private bool fired;

        public PredicateCondition(MiniGameResult result, Func<bool> predicate)
        {
            Result = result;
            this.predicate = predicate;
        }

        public void Arm(MonoBehaviour host)
        {
            this.host = host;
            routine = host.StartCoroutine(Wait());
        }

        public void Disarm()
        {
            if (host != null && routine != null)
            {
                host.StopCoroutine(routine);
            }
            routine = null;
        }

        private IEnumerator Wait()
        {
            yield return new WaitUntil(predicate);
            if (fired)
            {
                yield break;
            }
            fired = true;
            Met?.Invoke(this);
        }
    }
}
