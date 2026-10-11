using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 미니게임 씬을 Additive 로 불러와 지시문 → 실행 → 제한 시간 → 결과 → 언로드
    /// 순서로 진행하는 호스트.
    /// </summary>
    public class MiniGameRunner : MonoBehaviour
    {
        public static MiniGameRunner Instance { get; private set; }

        public Text instructionText;
        public Image timeBarImage;
        public float instructionDisplayTime = 0.7f;
        [SerializeField] private float resultHoldSeconds = 0.6f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Run(MiniGameDefinition def, MiniGameContext ctx, Action<MiniGameResult> onComplete)
        {
            StartCoroutine(RunRoutine(def, ctx, onComplete));
        }

        private IEnumerator RunRoutine(MiniGameDefinition def, MiniGameContext ctx, Action<MiniGameResult> onComplete)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(def.sceneName, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = false;
            while (loadOp.progress < 0.9f)
            {
                yield return null;
            }
            Scene loadedScene = SceneManager.GetSceneByName(def.sceneName);

            // 활성화(Awake/OnEnable) 전에 호스트가 이미 가진 EventSystem 을 먼저 제거해
            // "multiple EventSystems" 경고가 아예 나지 않게 한다.
            DisableHostedComponents(loadedScene);

            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
            {
                yield return null;
            }
            MiniGameBase instance = FindMiniGameInScene(loadedScene);

            if (instructionText != null)
            {
                instructionText.text = def.instruction;
                instructionText.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(instructionDisplayTime);
            if (instructionText != null)
            {
                instructionText.gameObject.SetActive(false);
            }

            MiniGameResult? result = null;
            Action<MiniGameResult> handler = r => result = r;

            if (instance != null)
            {
                instance.IsHosted = true;
                instance.Finished += handler;
                instance.Begin(ctx);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"MiniGameRunner: {def.sceneName} 씬에서 MiniGameBase 를 찾지 못했습니다.");
            }

            float timeLimit = ctx.TimeLimit > 0f ? ctx.TimeLimit : def.baseTimeLimit;
            float elapsed = 0f;
            while (result == null && elapsed < timeLimit)
            {
                elapsed += Time.deltaTime;
                if (timeBarImage != null)
                {
                    timeBarImage.fillAmount = 1f - Mathf.Clamp01(elapsed / timeLimit);
                }
                yield return null;
            }

            if (instance != null)
            {
                instance.Finished -= handler;
            }

            MiniGameResult finalResult = result ?? def.timeoutResult;

            if (timeBarImage != null)
            {
                timeBarImage.fillAmount = 0f;
            }

            // 게임이 직접 결과를 알렸으면 결과 애니메이션을 보여 줄 시간을 준다.
            if (result != null && resultHoldSeconds > 0f)
            {
                yield return new WaitForSeconds(resultHoldSeconds);
            }

            if (loadedScene.IsValid())
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(loadedScene);
                while (unloadOp != null && !unloadOp.isDone)
                {
                    yield return null;
                }
            }

            onComplete?.Invoke(finalResult);
        }

        private void DisableHostedComponents(Scene scene)
        {
            if (!scene.IsValid())
            {
                return;
            }
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
                {
                    cam.enabled = false;
                }
                foreach (AudioListener listener in root.GetComponentsInChildren<AudioListener>(true))
                {
                    listener.enabled = false;
                }
                foreach (EventSystem es in root.GetComponentsInChildren<EventSystem>(true))
                {
                    // 비활성화만 하면 오브젝트가 씬에 남아 FindObjectsByType 등에서 중복으로 잡힌다.
                    // 호스트(Battle) 의 EventSystem 을 그대로 쓰므로 미니게임 쪽은 아예 파괴한다.
                    Destroy(es.gameObject);
                }
            }
        }

        private MiniGameBase FindMiniGameInScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return null;
            }
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                MiniGameBase found = root.GetComponentInChildren<MiniGameBase>(true);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
    }
}
