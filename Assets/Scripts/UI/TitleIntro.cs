using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BoolPyeon.UI
{
    /// <summary>
    /// Title 씬 진입 연출(로고 팝인, TOUCH TO START 깜빡임, 배경 줌·패닝)과
    /// 전환 연출(점멸 → 화이트 페이드)을 담당한다.
    /// </summary>
    public class TitleIntro : MonoBehaviour
    {
        public RectTransform logo;
        public RectTransform touchToStart;
        public RectTransform keyVisual;
        public RectTransform character;
        public Image whiteFade;
        public float inputDelay = 0.9f;
        public AudioSource sfxSource;
        public AudioClip tapSfx;

        public bool IsReady { get; private set; }

        private CanvasGroup _logoGroup;
        private CanvasGroup _touchGroup;
        private CanvasGroup _keyVisualGroup;

        private Vector2 _keyVisualStartPos;
        private Vector3 _keyVisualStartScale = Vector3.one;
        private Vector2 _characterStartPos;

        private Coroutine _introRoutine;
        private Coroutine _blinkRoutine;
        private Coroutine _backgroundRoutine;
        private Coroutine _characterRoutine;

        private bool _introSkipped;

        private CanvasGroup GetOrAddCanvasGroup(RectTransform target)
        {
            if (target == null)
            {
                return null;
            }

            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = target.gameObject.AddComponent<CanvasGroup>();
            }

            return group;
        }

        private void Awake()
        {
            _logoGroup = GetOrAddCanvasGroup(logo);
            _touchGroup = GetOrAddCanvasGroup(touchToStart);
            _keyVisualGroup = GetOrAddCanvasGroup(keyVisual);

            if (logo != null)
            {
                logo.localScale = Vector3.zero;
            }

            if (_logoGroup != null)
            {
                _logoGroup.alpha = 0f;
            }

            if (_touchGroup != null)
            {
                _touchGroup.alpha = 0f;
            }

            if (_keyVisualGroup != null)
            {
                _keyVisualGroup.alpha = 0f;
            }

            if (keyVisual != null)
            {
                _keyVisualStartPos = keyVisual.anchoredPosition;
                _keyVisualStartScale = keyVisual.localScale;
            }

            if (character != null)
            {
                _characterStartPos = character.anchoredPosition;
            }
        }

        private void Start()
        {
            _introRoutine = StartCoroutine(IntroRoutine());
            _backgroundRoutine = StartCoroutine(BackgroundIdle());
            _characterRoutine = StartCoroutine(CharacterIdle());
        }

        private IEnumerator IntroRoutine()
        {
            float t = 0f;

            // KeyVisual alpha 0->1 (0~0.4s)
            while (t < 0.4f)
            {
                if (_introSkipped)
                {
                    yield break;
                }

                t += Time.unscaledDeltaTime;
                if (_keyVisualGroup != null)
                {
                    _keyVisualGroup.alpha = Mathf.Clamp01(t / 0.4f);
                }

                yield return null;
            }

            if (_keyVisualGroup != null)
            {
                _keyVisualGroup.alpha = 1f;
            }

            // wait until 0.3s total elapsed before starting logo pop-in
            while (t < 0.3f)
            {
                if (_introSkipped)
                {
                    yield break;
                }

                t += Time.unscaledDeltaTime;
                yield return null;
            }

            // Logo scale 0 -> 1.1 (0.3s) -> 1.0 (0.15s), alpha 0 -> 1
            float phase1 = 0f;
            while (phase1 < 0.3f)
            {
                if (_introSkipped)
                {
                    yield break;
                }

                phase1 += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(phase1 / 0.3f);
                if (logo != null)
                {
                    logo.localScale = Vector3.one * Mathf.Lerp(0f, 1.1f, p);
                }

                if (_logoGroup != null)
                {
                    _logoGroup.alpha = p;
                }

                yield return null;
            }

            float phase2 = 0f;
            while (phase2 < 0.15f)
            {
                if (_introSkipped)
                {
                    yield break;
                }

                phase2 += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(phase2 / 0.15f);
                if (logo != null)
                {
                    logo.localScale = Vector3.one * Mathf.Lerp(1.1f, 1.0f, p);
                }

                yield return null;
            }

            if (logo != null)
            {
                logo.localScale = Vector3.one;
            }

            if (_logoGroup != null)
            {
                _logoGroup.alpha = 1f;
            }

            // wait until inputDelay(0.9s) total elapsed
            float totalElapsed = 0.4f > 0.3f + 0.3f + 0.15f ? 0.4f : 0.3f + 0.3f + 0.15f;
            while (totalElapsed < inputDelay)
            {
                if (_introSkipped)
                {
                    yield break;
                }

                totalElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            ShowTouchToStart();
        }

        private void ShowTouchToStart()
        {
            if (_touchGroup != null)
            {
                _touchGroup.alpha = 1f;
            }

            IsReady = true;
            _blinkRoutine = StartCoroutine(BlinkRoutine());
        }

        private IEnumerator BlinkRoutine()
        {
            float t = 0f;
            const float period = 1.2f;

            while (true)
            {
                t += Time.unscaledDeltaTime;
                float sinValue = Mathf.Sin((t / period) * Mathf.PI * 2f);
                float alpha = Mathf.Lerp(0.25f, 1f, (sinValue + 1f) * 0.5f);
                if (_touchGroup != null)
                {
                    _touchGroup.alpha = alpha;
                }

                yield return null;
            }
        }

        private IEnumerator BackgroundIdle()
        {
            float t = 0f;
            const float period = 8f;

            while (true)
            {
                t += Time.unscaledDeltaTime;
                float sinValue = Mathf.Sin((t / period) * Mathf.PI * 2f);

                if (keyVisual != null)
                {
                    float scale = Mathf.Lerp(1.0f, 1.03f, (sinValue + 1f) * 0.5f);
                    keyVisual.localScale = _keyVisualStartScale * scale;

                    Vector2 pos = _keyVisualStartPos;
                    pos.x += sinValue * 20f;
                    keyVisual.anchoredPosition = pos;
                }

                yield return null;
            }
        }

        private IEnumerator CharacterIdle()
        {
            float t = 0f;
            const float period = 3f;

            while (true)
            {
                t += Time.unscaledDeltaTime;
                float sinValue = Mathf.Sin((t / period) * Mathf.PI * 2f);

                if (character != null)
                {
                    Vector2 pos = _characterStartPos;
                    pos.y += sinValue * 8f;
                    character.anchoredPosition = pos;
                }

                yield return null;
            }
        }

        public void SkipIntro()
        {
            _introSkipped = true;

            if (_introRoutine != null)
            {
                StopCoroutine(_introRoutine);
                _introRoutine = null;
            }

            if (logo != null)
            {
                logo.localScale = Vector3.one;
            }

            if (_logoGroup != null)
            {
                _logoGroup.alpha = 1f;
            }

            if (_keyVisualGroup != null)
            {
                _keyVisualGroup.alpha = 1f;
            }

            if (!IsReady)
            {
                ShowTouchToStart();
            }
        }

        private void PlayTapSfx()
        {
            if (sfxSource != null && tapSfx != null)
            {
                sfxSource.PlayOneShot(tapSfx);
            }
        }

        public void PlayExit(Action onDone)
        {
            StartCoroutine(PlayExitRoutine(onDone));
        }

        private IEnumerator PlayExitRoutine(Action onDone)
        {
            if (_blinkRoutine != null)
            {
                StopCoroutine(_blinkRoutine);
                _blinkRoutine = null;
            }

            // 빠른 점멸 0.3s
            float t = 0f;
            const float blinkDuration = 0.3f;
            const float blinkPeriod = 0.1f;

            while (t < blinkDuration)
            {
                t += Time.unscaledDeltaTime;
                float sinValue = Mathf.Sin((t / blinkPeriod) * Mathf.PI * 2f);
                float alpha = Mathf.Lerp(0.25f, 1f, (sinValue + 1f) * 0.5f);
                if (_touchGroup != null)
                {
                    _touchGroup.alpha = alpha;
                }

                yield return null;
            }

            PlayTapSfx();

            if (whiteFade != null)
            {
                whiteFade.raycastTarget = true;
                Color c = whiteFade.color;
                float fadeT = 0f;
                const float fadeDuration = 0.35f;

                while (fadeT < fadeDuration)
                {
                    fadeT += Time.unscaledDeltaTime;
                    c.a = Mathf.Clamp01(fadeT / fadeDuration);
                    whiteFade.color = c;
                    yield return null;
                }

                c.a = 1f;
                whiteFade.color = c;
            }

            onDone?.Invoke();
        }
    }
}
