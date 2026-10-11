using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.MiniGames;
using BoolPyeon.MiniGames.Phase;

namespace BoolPyeon.UI
{
    /// <summary>
    /// MiniGamePhase 진행 상태(남은 시간·에너지·콤보)와 성공/실패 피드백,
    /// 종료 결과 요약을 화면에 보여주는 Presenter. Update 에서 phase 를 폴링하고
    /// PhaseStarted / MiniGameResolved 이벤트를 구독한다.
    /// </summary>
    public class MiniGamePhaseHud : MonoBehaviour
    {
        public MiniGamePhase phase;

        public Image phaseTimeBar;
        public Image energyGauge;
        public Text energyText;
        public Text comboText;
        public Text feedbackText;
        public GameObject resultPanel;
        public Text resultText;
        public GameObject startHintText;

        // 와리오식 연출 리소스(T5). hintBanner는 startHintText 오브젝트 자체의 Image(글자는 자식 Label)라서
        // 표시/숨김 로직은 startHintText.SetActive를 그대로 따라가므로 여기서 따로 제어하지 않는다.
        // 스프라이트를 교체하는 용도로만 인스펙터에서 연결한다.
        public Image hintBanner;
        public RectTransform timeBarFuseIcon;
        public GameObject successStamp;
        public GameObject failStamp;
        public Text energyPopupText;

        [SerializeField] private float lowTimeThreshold = 3f;
        [SerializeField] private float blinkSpeed = 6f;
        [SerializeField] private float feedbackPopSeconds = 0.5f;
        [SerializeField] private float resultHoldSeconds = 2f;
        [SerializeField] private float energyPopupSeconds = 0.6f;

        private Coroutine feedbackRoutine;
        private Coroutine energyPopupRoutine;
        private int lastEnergy;

        private void OnEnable()
        {
            if (phase != null)
            {
                phase.PhaseStarted += HandlePhaseStarted;
                phase.MiniGameResolved += HandleMiniGameResolved;
            }
            HandlePhaseStarted();
            if (startHintText != null)
            {
                startHintText.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (phase != null)
            {
                phase.PhaseStarted -= HandlePhaseStarted;
                phase.MiniGameResolved -= HandleMiniGameResolved;
            }
        }

        private void Update()
        {
            if (phase == null || !phase.IsRunning)
            {
                return;
            }

            MiniGamePhaseConfig config = phase.config;
            MiniGamePhaseState state = phase.State;

            if (phaseTimeBar != null)
            {
                float duration = Mathf.Max(0.0001f, phase.CurrentDuration);
                float ratio = Mathf.Clamp01(phase.RemainingTime / duration);
                phaseTimeBar.fillAmount = ratio;
                bool lowTime = phase.RemainingTime <= lowTimeThreshold;
                float blink = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
                if (lowTime)
                {
                    phaseTimeBar.color = Color.Lerp(Color.white, Color.red, blink);
                }
                else
                {
                    phaseTimeBar.color = Color.white;
                }

                if (timeBarFuseIcon != null)
                {
                    Vector2 anchor = timeBarFuseIcon.anchorMin;
                    anchor.x = ratio;
                    timeBarFuseIcon.anchorMin = anchor;
                    Vector2 anchorMax = timeBarFuseIcon.anchorMax;
                    anchorMax.x = ratio;
                    timeBarFuseIcon.anchorMax = anchorMax;
                    float fuseScale = lowTime ? Mathf.Lerp(1f, 1.2f, blink) : 1f;
                    timeBarFuseIcon.localScale = Vector3.one * fuseScale;
                }
            }

            if (state == null)
            {
                return;
            }

            int energyCap = Mathf.Max(1, config.energyCap);
            if (energyGauge != null)
            {
                energyGauge.fillAmount = Mathf.Clamp01((float)state.Energy / energyCap);
            }
            if (energyText != null)
            {
                energyText.text = $"{state.Energy}/{energyCap}";
            }

            if (comboText != null)
            {
                bool show = state.Combo >= 2;
                comboText.gameObject.SetActive(show);
                if (show)
                {
                    comboText.text = $"x{state.Combo}";
                }
            }

            if (energyPopupText != null && state.Energy > lastEnergy)
            {
                int gained = state.Energy - lastEnergy;
                if (energyPopupRoutine != null)
                {
                    StopCoroutine(energyPopupRoutine);
                }
                energyPopupRoutine = StartCoroutine(ShowEnergyPopup(gained));
            }
            lastEnergy = state.Energy;
        }

        private IEnumerator ShowEnergyPopup(int gained)
        {
            energyPopupText.text = $"+{gained}";
            energyPopupText.gameObject.SetActive(true);
            Transform t = energyPopupText.transform;
            Vector3 basePos = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < energyPopupSeconds)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / energyPopupSeconds);
                t.localPosition = basePos + new Vector3(0f, 40f * progress, 0f);
                yield return null;
            }
            t.localPosition = basePos;
            energyPopupText.gameObject.SetActive(false);
            energyPopupRoutine = null;
        }

        // 순차 표시 규칙: 새 페이즈 시작 시 result/combo/feedback/startHint를 모두 숨긴다(동시 표시 없음, 로직 변경 없음).
        private void HandlePhaseStarted()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
            if (comboText != null)
            {
                comboText.gameObject.SetActive(false);
            }
            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(false);
            }
            if (startHintText != null)
            {
                startHintText.SetActive(false);
            }
            if (energyPopupText != null)
            {
                energyPopupText.gameObject.SetActive(false);
            }
            if (successStamp != null)
            {
                successStamp.SetActive(false);
            }
            if (failStamp != null)
            {
                failStamp.SetActive(false);
            }
            lastEnergy = 0;
        }

        // 순차 표시 규칙: feedback(ShowFeedback) → result(ShowResult) → 자동 숨김 후 startHint 재표시(HideResultAfterDelay) 순으로,
        // 세 요소는 동시에 표시되지 않는다. 로직은 바꾸지 않고 이 규칙만 문서화한다.
        private void HandleMiniGameResolved(MiniGameResult result)
        {
            if (feedbackText == null)
            {
                return;
            }
            if (feedbackRoutine != null)
            {
                StopCoroutine(feedbackRoutine);
            }
            feedbackRoutine = StartCoroutine(ShowFeedback(result));
        }

        private IEnumerator ShowFeedback(MiniGameResult result)
        {
            bool success = result == MiniGameResult.Success;
            feedbackText.text = success ? "SUCCESS!" : "FAIL";
            feedbackText.color = success ? Color.green : Color.red;
            feedbackText.gameObject.SetActive(true);

            GameObject stamp = success ? successStamp : failStamp;
            if (stamp != null)
            {
                stamp.SetActive(true);
            }

            Transform t = feedbackText.transform;
            Transform stampT = stamp != null ? stamp.transform : null;
            float elapsed = 0f;
            Vector3 baseScale = Vector3.one;
            while (elapsed < feedbackPopSeconds)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / feedbackPopSeconds);
                float scale = success
                    ? Mathf.Lerp(1f, 1f, progress) * (1f + 0.2f * Mathf.Sin(progress * Mathf.PI))
                    : 1f;
                Vector3 shake = !success ? new Vector3(Mathf.Sin(elapsed * 40f) * 4f, 0f, 0f) : Vector3.zero;
                t.localScale = baseScale * scale;
                t.localPosition = shake;
                if (stampT != null)
                {
                    stampT.localScale = baseScale * scale;
                    stampT.localPosition = shake;
                }
                yield return null;
            }
            t.localScale = baseScale;
            t.localPosition = Vector3.zero;
            feedbackText.gameObject.SetActive(false);
            if (stampT != null)
            {
                stampT.localScale = baseScale;
                stampT.localPosition = Vector3.zero;
            }
            if (stamp != null)
            {
                stamp.SetActive(false);
            }
            feedbackRoutine = null;
        }

        /// <summary>페이즈 종료 시 호출: 결과 요약을 잠깐 보여준다.</summary>
        public void ShowResult(MiniGamePhaseResult result)
        {
            if (resultPanel == null)
            {
                return;
            }
            if (resultText != null)
            {
                resultText.text = $"성공 {result.Successes} / 실패 {result.Failures}\n에너지 {result.Energy} / 최대콤보 {result.MaxCombo}";
            }
            resultPanel.SetActive(true);
            StartCoroutine(HideResultAfterDelay());
        }

        private IEnumerator HideResultAfterDelay()
        {
            yield return new WaitForSeconds(resultHoldSeconds);
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
            if (startHintText != null)
            {
                startHintText.SetActive(true);
            }
        }
    }
}
