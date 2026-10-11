using System.Collections;
using UnityEngine;
using BoolPyeon.MiniGames.Phase;
using BoolPyeon.UI;

namespace BoolPyeon.MiniGames.Debug
{
    /// <summary>
    /// Title 씬 버튼 또는 시작 입력(Space/클릭)에서 호출: MiniGamePhase 를 한 번 돌리고
    /// 결과를 콘솔에 남기며 HUD 결과 패널을 띄운다.
    /// </summary>
    public class MiniGamePhaseDebug : MonoBehaviour
    {
        public MiniGamePhase phase;
        public MiniGamePhaseHud hud;
        public bool autoRunOnStart = false;
        public KeyCode startKey = KeyCode.Space;

        private void Start()
        {
            if (autoRunOnStart)
            {
                StartCoroutine(AutoRunNextFrame());
            }
        }

        private IEnumerator AutoRunNextFrame()
        {
            yield return null;
            RunPhaseDebug();
        }

        private void Update()
        {
            if (phase == null || phase.IsRunning)
            {
                return;
            }
            if (UnityEngine.Input.GetKeyDown(startKey) || UnityEngine.Input.GetMouseButtonDown(0))
            {
                StartCoroutine(StartNextFrame());
            }
        }

        private IEnumerator StartNextFrame()
        {
            // 시작 입력으로 사용한 클릭이 다음 프레임 첫 미니게임 입력으로 새지 않도록 한 프레임 미룬다.
            yield return null;
            RunPhaseDebug();
        }

        public void RunPhaseDebug()
        {
            if (phase == null)
            {
                UnityEngine.Debug.LogWarning("MiniGamePhaseDebug: phase 가 비어 있습니다.");
                return;
            }
            phase.Run(OnResult);
        }

        private void OnResult(MiniGamePhaseResult result)
        {
            UnityEngine.Debug.Log($"Phase result: S={result.Successes} F={result.Failures} E={result.Energy} MaxCombo={result.MaxCombo}");
            hud?.ShowResult(result);
        }
    }
}
