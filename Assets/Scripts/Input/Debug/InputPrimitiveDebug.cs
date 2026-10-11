using BoolPyeon.Input.Interaction;
using UnityEngine;

namespace BoolPyeon.Input.Debug
{
    /// <summary>
    /// 입력 프리미티브 4종(연타·긋기·문지르기·돌리기)의 값을 로그와 화면에
    /// 표시하는 디버그 드라이버. R 키로 모두 초기화한다.
    /// </summary>
    public class InputPrimitiveDebug : MonoBehaviour
    {
        public MashInteraction mash;
        public SwipeInteraction swipe;
        public RubInteraction rub;
        public RotateInteraction rotate;
        public UnityEngine.UI.Text label;

        private int lastMashCount;
        private bool lastSwipeTracing;
        private float lastSwipeMaxDev;
        private int lastRubChanges;
        private bool lastRotating;
        private int lastTurns;

        private void OnEnable()
        {
            swipe?.Reset();
            if (swipe != null)
            {
                swipe.Finished += OnSwipeFinished;
            }
        }

        private void OnDisable()
        {
            if (swipe != null)
            {
                swipe.Finished -= OnSwipeFinished;
            }
        }

        private void Update()
        {
            mash?.Tick();
            swipe?.Tick();
            rub?.Tick();
            rotate?.Tick();

            if (mash != null && mash.Count != lastMashCount)
            {
                UnityEngine.Debug.Log($"[InputDebug] Mash count={mash.Count} rate={mash.Rate:F2}");
                lastMashCount = mash.Count;
            }

            if (swipe != null && (swipe.IsTracing != lastSwipeTracing || !Mathf.Approximately(swipe.MaxDeviation, lastSwipeMaxDev)))
            {
                UnityEngine.Debug.Log($"[InputDebug] Swipe tracing={swipe.IsTracing} progress={swipe.Progress:F2} maxDev={swipe.MaxDeviation:F2}");
                lastSwipeTracing = swipe.IsTracing;
                lastSwipeMaxDev = swipe.MaxDeviation;
            }

            if (rub != null && rub.DirectionChanges != lastRubChanges)
            {
                UnityEngine.Debug.Log($"[InputDebug] Rub directionChanges={rub.DirectionChanges} distance={rub.Distance:F2}");
                lastRubChanges = rub.DirectionChanges;
            }

            if (rotate != null)
            {
                int turns = Mathf.FloorToInt(rotate.Turns);
                if (rotate.IsRotating != lastRotating || turns != lastTurns)
                {
                    UnityEngine.Debug.Log($"[InputDebug] Rotate angle={rotate.AccumulatedAngle:F1} turns={rotate.Turns:F2}");
                    lastRotating = rotate.IsRotating;
                    lastTurns = turns;
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                mash?.Reset();
                swipe?.Reset();
                rub?.Reset();
                rotate?.Reset();
                UnityEngine.Debug.Log("[InputDebug] Reset all");
            }
        }

        private void OnSwipeFinished(bool completed, float maxDev)
        {
            UnityEngine.Debug.Log($"[InputDebug] Swipe finished completed={completed} maxDev={maxDev:F2}");
        }

        private void OnGUI()
        {
            string text = "Mash: count=" + (mash != null ? mash.Count : 0) + " rate=" + (mash != null ? mash.Rate.ToString("F2") : "0") + "\n"
                + "Swipe: tracing=" + (swipe != null && swipe.IsTracing) + " progress=" + (swipe != null ? swipe.Progress.ToString("F2") : "0") + " maxDev=" + (swipe != null ? swipe.MaxDeviation.ToString("F2") : "0") + " completed=" + (swipe != null && swipe.Completed) + "\n"
                + "Rub: changes=" + (rub != null ? rub.DirectionChanges : 0) + " distance=" + (rub != null ? rub.Distance.ToString("F2") : "0") + "\n"
                + "Rotate: angle=" + (rotate != null ? rotate.AccumulatedAngle.ToString("F1") : "0") + " turns=" + (rotate != null ? rotate.Turns.ToString("F2") : "0") + "\n"
                + "(R = Reset)";

            GUI.Label(new Rect(10, 10, 500, 160), text);

            if (label != null)
            {
                label.text = text;
            }
        }
    }
}
