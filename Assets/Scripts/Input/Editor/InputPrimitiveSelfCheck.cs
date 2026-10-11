using BoolPyeon.Input.Interaction;
using UnityEditor;
using UnityEngine;

namespace BoolPyeon.Input.Editor
{
    /// <summary>
    /// 연타·긋기·문지르기·돌리기 4종 인터랙션에 합성 PointerSample을 넣어
    /// 설계 규칙대로 값이 나오는지 확인하는 에디터 전용 셀프체크.
    /// </summary>
    public static class InputPrimitiveSelfCheck
    {
        [MenuItem("Tools/BoolPyeon/F32 Input SelfCheck")]
        public static void RunAllMenu()
        {
            RunAll();
        }

        public static bool RunAll()
        {
            bool mash = CheckMash();
            bool swipe = CheckSwipe();
            bool rub = CheckRub();
            bool rotate = CheckRotate();
            return mash && swipe && rub && rotate;
        }

        private static PointerSample Sample(bool down, bool held, Vector2 world, float time, bool up = false, GameObject hit = null)
        {
            return new PointerSample { down = down, held = held, up = up, world = world, time = time, hit = hit, overUI = false };
        }

        private static bool CheckMash()
        {
            GameObject go = new GameObject("SelfCheck_MashTarget");
            go.AddComponent<BoxCollider2D>();
            var mash = new MashInteraction { target = go.transform, window = 1f };

            float t = 0f;
            for (int i = 0; i < 5; i++)
            {
                t = i * 0.1f;
                mash.Step(Sample(true, true, Vector2.zero, t, hit: go));
                mash.Step(Sample(false, false, Vector2.zero, t + 0.01f));
            }

            bool pass = mash.Count == 5 && mash.Rate > 0f;

            mash.Step(Sample(false, false, Vector2.zero, t + 1.5f));
            pass &= mash.Count == 0 && mash.TotalCount == 5;

            Object.DestroyImmediate(go);

            LogResult("Mash", pass, $"Count={mash.Count} Rate={mash.Rate} TotalCount={mash.TotalCount}");
            return pass;
        }

        private static bool CheckSwipe()
        {
            GameObject startGo = new GameObject("SelfCheck_SwipeStart") { transform = { position = new Vector3(0f, 0f, 0f) } };
            GameObject endGo = new GameObject("SelfCheck_SwipeEnd") { transform = { position = new Vector3(4f, 0f, 0f) } };

            var onPath = new SwipeInteraction { path = new[] { startGo.transform, endGo.transform }, startRadius = 0.5f, finishRadius = 0.5f };
            onPath.Step(Sample(true, true, new Vector2(0f, 0f), 0f));
            onPath.Step(Sample(false, true, new Vector2(1f, 0f), 0.1f));
            onPath.Step(Sample(false, true, new Vector2(2f, 0f), 0.2f));
            onPath.Step(Sample(false, true, new Vector2(3f, 0f), 0.3f));
            onPath.Step(Sample(false, false, new Vector2(4f, 0f), 0.4f, up: true));
            bool onPathPass = onPath.Completed && onPath.MaxDeviation < 0.1f;

            var offPath = new SwipeInteraction { path = new[] { startGo.transform, endGo.transform }, startRadius = 0.5f, finishRadius = 0.5f };
            offPath.Step(Sample(true, true, new Vector2(0f, 0f), 0f));
            offPath.Step(Sample(false, true, new Vector2(2f, 1f), 0.1f));
            bool offPathPass = offPath.MaxDeviation >= 0.9f;

            Object.DestroyImmediate(startGo);
            Object.DestroyImmediate(endGo);

            bool pass = onPathPass && offPathPass;
            LogResult("Swipe", pass, $"Completed={onPath.Completed} MaxDeviation(on)={onPath.MaxDeviation} MaxDeviation(off)={offPath.MaxDeviation}");
            return pass;
        }

        private static bool CheckRub()
        {
            GameObject go = new GameObject("SelfCheck_RubTarget");
            go.AddComponent<BoxCollider2D>();
            var rub = new RubInteraction { target = go.transform, axis = RubAxis.Horizontal, minStroke = 0.3f };

            rub.Step(Sample(true, true, new Vector2(0f, 0f), 0f, hit: go));
            rub.Step(Sample(false, true, new Vector2(0.5f, 0f), 0.1f, hit: go));
            rub.Step(Sample(false, true, new Vector2(-0.5f, 0f), 0.2f, hit: go));
            rub.Step(Sample(false, true, new Vector2(0.5f, 0f), 0.3f, hit: go));
            rub.Step(Sample(false, true, new Vector2(-0.5f, 0f), 0.4f, hit: go));
            rub.Step(Sample(false, true, new Vector2(0.5f, 0f), 0.5f, hit: go));
            rub.Step(Sample(false, false, new Vector2(0.5f, 0f), 0.6f, up: true, hit: go));

            bool pass = rub.DirectionChanges >= 3 && rub.Distance > 0f;

            Object.DestroyImmediate(go);

            LogResult("Rub", pass, $"DirectionChanges={rub.DirectionChanges} Distance={rub.Distance}");
            return pass;
        }

        private static bool CheckRotate()
        {
            GameObject centerGo = new GameObject("SelfCheck_RotateCenter") { transform = { position = Vector3.zero } };
            var rotate = new RotateInteraction { center = centerGo.transform, grabRadius = 0f };
            const float radius = 2f;

            rotate.Step(Sample(true, true, new Vector2(radius, 0f), 0f));
            for (int i = 1; i <= 36; i++)
            {
                float rad = i * 10f * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                rotate.Step(Sample(false, true, pos, i * 0.05f));
            }
            rotate.Step(Sample(false, false, new Vector2(radius, 0f), 2f, up: true));

            float accumulatedCcw = rotate.AccumulatedAngle;
            bool ccwMagnitude = Mathf.Abs(Mathf.Abs(accumulatedCcw) - 360f) <= 5f && Mathf.Abs(rotate.Turns - 1f) <= 0.05f;
            bool ccwSign = accumulatedCcw > 0f;

            rotate.Reset();
            rotate.Step(Sample(true, true, new Vector2(radius, 0f), 0f));
            for (int i = 1; i <= 36; i++)
            {
                float rad = -i * 10f * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                rotate.Step(Sample(false, true, pos, i * 0.05f));
            }
            rotate.Step(Sample(false, false, new Vector2(radius, 0f), 2f, up: true));
            float accumulatedCw = rotate.AccumulatedAngle;
            bool cwSign = accumulatedCw < 0f;

            Object.DestroyImmediate(centerGo);

            bool pass = ccwMagnitude && ccwSign && cwSign;
            LogResult("Rotate", pass, $"AccumulatedAngle(ccw)={accumulatedCcw} AccumulatedAngle(cw)={accumulatedCw}");
            return pass;
        }

        private static void LogResult(string name, bool pass, string details)
        {
            string message = $"[F32Check] {name} {(pass ? "PASS" : "FAIL")} {details}";
            if (pass)
            {
                UnityEngine.Debug.Log(message);
            }
            else
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}
