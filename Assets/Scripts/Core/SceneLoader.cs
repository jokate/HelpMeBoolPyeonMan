using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BoolPyeon.Core
{
    /// <summary>
    /// 씬 전환을 맡는 싱글턴. Single/Additive 로드와 전체 화면 페이드를 담당한다.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        public float fadeDuration = 0.3f;

        private CanvasGroup fadeGroup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureFadeGroup();
        }

        public void LoadSingle(string sceneName, Action onDone = null)
        {
            StartCoroutine(LoadSingleRoutine(sceneName, onDone));
        }

        public void LoadAdditive(string sceneName, Action onDone = null)
        {
            StartCoroutine(LoadAdditiveRoutine(sceneName, onDone));
        }

        public void UnloadAsync(string sceneName, Action onDone = null)
        {
            StartCoroutine(UnloadRoutine(sceneName, onDone));
        }

        private IEnumerator LoadSingleRoutine(string sceneName, Action onDone)
        {
            yield return Fade(1f);
            DestroyCurrentEventSystems();
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (op != null && !op.isDone)
            {
                yield return null;
            }
            yield return Fade(0f);
            onDone?.Invoke();
        }

        private IEnumerator LoadAdditiveRoutine(string sceneName, Action onDone)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (op != null && !op.isDone)
            {
                yield return null;
            }
            onDone?.Invoke();
        }

        private IEnumerator UnloadRoutine(string sceneName, Action onDone)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
            while (op != null && !op.isDone)
            {
                yield return null;
            }
            onDone?.Invoke();
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (fadeGroup == null)
            {
                yield break;
            }
            fadeGroup.blocksRaycasts = targetAlpha > 0f;
            float start = fadeGroup.alpha;
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, time / fadeDuration);
                yield return null;
            }
            fadeGroup.alpha = targetAlpha;
        }

        // Single 로드 직전에 현재 남아있는 EventSystem 을 즉시 파괴한다.
        // enabled=false 로만 끄면 새 씬의 EventSystem 이 OnEnable 되는 시점까지
        // 이전 씬의 언로드가 끝나지 않아 "There can be only one active Event System" 경고가 난다.
        // DestroyImmediate 로 이번 프레임 안에서 확실히 제거해 새 EventSystem 과 겹치지 않게 한다.
        private void DestroyCurrentEventSystems()
        {
            EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            foreach (EventSystem es in eventSystems)
            {
                DestroyImmediate(es.gameObject);
            }
        }

        private void EnsureFadeGroup()
        {
            GameObject canvasObj = new GameObject("SceneFadeCanvas");
            canvasObj.transform.SetParent(transform);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            canvasObj.AddComponent<CanvasScaler>();

            GameObject panelObj = new GameObject("FadePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            Image image = panelObj.AddComponent<Image>();
            image.color = Color.black;
            RectTransform rt = panelObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            fadeGroup = panelObj.AddComponent<CanvasGroup>();
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
        }
    }
}
