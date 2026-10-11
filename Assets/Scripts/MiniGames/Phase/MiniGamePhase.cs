using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.MiniGames.Phase
{
    /// <summary>
    /// 정해진 시간(phaseDuration) 동안 미니게임을 연속으로 돌리며
    /// 성공할수록 빨라지고 에너지가 쌓이는 드라이버.
    /// </summary>
    public class MiniGamePhase : MonoBehaviour
    {
        public MiniGameRunner runner;
        public List<MiniGameDefinition> pool = new List<MiniGameDefinition>();
        public MiniGamePhaseConfig config = MiniGamePhaseConfig.Default;

        private MiniGamePhaseState state;
        private float remainingTime;
        private float activeDuration;
        public float CurrentDuration => activeDuration;
        // F-30: BattleController 가 phase.Run() 전에 스쿼드 풀·에너지 규칙으로 바꿔 끼운다.
        // 둘 다 null 이면 기존 pool/config 그대로 동작한다(레거시).
        private List<MiniGameDefinition> squadPool;
        private IEnergyRule squadRule;
        // T8: Rush 등으로 페이즈 시간·판별 제한시간을 줄인다. 1이면 영향 없음.
        private float timeScale = 1f;

        // F-30: 스쿼드 출제 풀과 에너지 규칙을 주입한다. rule 은 null 을 줄 수 있다(레거시 계산 유지).
        public void SetSquad(List<MiniGameDefinition> squadPoolOverride, IEnergyRule rule)
        {
            squadPool = squadPoolOverride;
            squadRule = rule;
        }

        // T8: BattleController 가 phase.Run() 직전에 State.EnergyCollectTimeScale 을 넣는다.
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Clamp(scale, 0.1f, 1f);
        }

        public float RemainingTime => remainingTime;
        public MiniGamePhaseState State => state;
        public bool IsRunning { get; private set; }

        /// <summary>페이즈가 시작될 때(Run 진입, 상태 초기화 직후) 발화.</summary>
        public event Action PhaseStarted;

        /// <summary>미니게임 한 판이 끝나고 state.Record 가 반영된 직후 발화.</summary>
        public event Action<MiniGameResult> MiniGameResolved;

        public void Run(Action<MiniGamePhaseResult> onComplete)
        {
            StartCoroutine(RunRoutine(config.phaseDuration, onComplete));
        }

        // T4: 스쿼드 멤버 한 명을 짧은 시간(config.MemberPhaseDurationOrDefault) 동안만 돌린다.
        // memberId 는 로그용이다(MiniGames 어셈블리는 Battle 어셈블리를 참조하지 않아 CharacterDefinition 을 못 받는다).
        public void RunForMember(string memberId, List<MiniGameDefinition> memberPool, IEnergyRule rule, Action<MiniGamePhaseResult> onComplete)
        {
            SetSquad(memberPool, rule);
            UnityEngine.Debug.Log($"[Squad] member={memberId ?? "?"} pool={(memberPool != null ? memberPool.Count : 0)}");
            StartCoroutine(RunRoutine(config.MemberPhaseDurationOrDefault, onComplete));
        }

        private IEnumerator RunRoutine(float duration, Action<MiniGamePhaseResult> onComplete)
        {
            List<MiniGameDefinition> activePool = (squadPool != null && squadPool.Count > 0) ? squadPool : pool;
            if (runner == null || activePool == null || activePool.Count == 0)
            {
                UnityEngine.Debug.LogWarning("MiniGamePhase: runner 또는 pool 이 비어 있습니다.");
                yield break;
            }

            state = new MiniGamePhaseState(config, squadRule);
            activeDuration = duration * timeScale;
            remainingTime = activeDuration;
            IsRunning = true;
            if (timeScale < 1f)
            {
                UnityEngine.Debug.Log($"MiniGamePhase: timeScale={timeScale:F2} duration={remainingTime:F1}");
            }
            PhaseStarted?.Invoke();
            if (squadRule != null)
            {
                var ids = new List<string>();
                foreach (var d in activePool) ids.Add(d != null ? d.id : "?");
                UnityEngine.Debug.Log($"MiniGamePhase: squad pool = [{string.Join(",", ids)}]");
            }

            while (remainingTime > 0f)
            {
                MiniGameDefinition def = state.PickNext(activePool, n => UnityEngine.Random.Range(0, n));
                if (def == null)
                {
                    break;
                }

                MiniGameContext ctx = state.BuildContext(def, remainingTime);
                ctx.TimeLimit *= timeScale;

                bool done = false;
                MiniGameResult last = MiniGameResult.Failed;
                runner.Run(def, ctx, r =>
                {
                    done = true;
                    last = r;
                });

                float startTime = remainingTime;
                while (!done)
                {
                    yield return null;
                    remainingTime -= Time.deltaTime;
                }
                remainingTime = Mathf.Min(remainingTime, startTime);

                int energyBefore = state.Energy;
                state.Record(last, def);
                int gain = state.Energy - energyBefore;
                if (squadRule != null)
                {
                    // F-30: [owner/trait] -> result +gain | Combo Energy
                    UnityEngine.Debug.Log($"MiniGamePhase: {def.id} [{squadRule.OwnerLabel(def)}] -> {last} +{gain} | Combo={state.Combo} Energy={state.Energy} Speed={state.Speed:F2}");
                }
                else
                {
                    UnityEngine.Debug.Log($"MiniGamePhase: {def.id} -> {last} +{gain} | Combo={state.Combo} Energy={state.Energy} Speed={state.Speed:F2}");
                }
                MiniGameResolved?.Invoke(last);

                if (remainingTime <= 0f)
                {
                    break;
                }
            }

            IsRunning = false;
            remainingTime = 0f;
            timeScale = 1f;
            onComplete?.Invoke(state.ToResult());
        }
    }
}
