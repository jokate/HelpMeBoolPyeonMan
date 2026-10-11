using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BoolPyeon.UI;
using BoolPyeon.Hub;

namespace BoolPyeon.BattleFlow
{
    // T18(설계) 씬을 넘나드는 선택형 자동 진행. Boot->Title->튜토리얼 전투->HQ->출격->전투... 를
    // 기존 버튼의 onClick.Invoke() 만 호출해 실제 경로를 그대로 지나간다. 규칙 코드는 건드리지 않는다.
    public class FlowAutoPilot : MonoBehaviour
    {
        public const string PrefsKey = "FlowAutoPilot";

        public int targetCycles = 2;
        public float stepDelay = 0.5f;
        public float timeoutPerScene = 60f;

        private static FlowAutoPilot instance;
        private int cyclesCompleted;
        private bool hqVisited;
        private bool stopped;
        private Coroutine timeoutRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (PlayerPrefs.GetInt(PrefsKey, 0) != 1) return;
            if (instance != null) return;
            var go = new GameObject("FlowAutoPilot");
            instance = go.AddComponent<FlowAutoPilot>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.logMessageReceived += OnLogMessage;
            Debug.Log("[Flow] start targetCycles=" + targetCycles);
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.logMessageReceived -= OnLogMessage;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            {
                Debug.Log("[Flow] F10 stop (manual takeover)");
                Stop();
            }
        }

        private void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (stopped) return;
            if (type == LogType.Exception || type == LogType.Error)
            {
                if (message.StartsWith("[Flow]") || message.StartsWith("[AutoPilot]")) return;
                Debug.Log("[Flow] FAIL error=" + message);
                Stop();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (stopped) return;
            if (timeoutRoutine != null) StopCoroutine(timeoutRoutine);
            timeoutRoutine = StartCoroutine(SceneTimeout(scene.name));

            switch (scene.name)
            {
                case "Title":
                    StartCoroutine(DriveTitle());
                    break;
                case "HQ":
                    StartCoroutine(DriveHQ());
                    break;
                case "Battle":
                    StartCoroutine(DriveBattle());
                    break;
            }
        }

        private IEnumerator SceneTimeout(string sceneName)
        {
            yield return new WaitForSeconds(timeoutPerScene);
            if (stopped) yield break;
            Debug.Log("[Flow] STUCK scene=" + sceneName);
            Stop();
        }

        private IEnumerator DriveTitle()
        {
            yield return new WaitForSeconds(stepDelay);
            var title = FindObjectOfType<TitleScreen>();
            if (title == null || title.startButton == null)
            {
                Debug.Log("[Flow] Title FAIL startButton missing");
                Stop();
                yield break;
            }
            Debug.Log("[Flow] Title start");
            title.startButton.onClick.Invoke();
        }

        private IEnumerator DriveHQ()
        {
            yield return new WaitForSeconds(stepDelay);
            if (!hqVisited)
            {
                hqVisited = true;
                Debug.Log("[Flow] HQ arrive (post-tutorial, not counted)");
            }
            else
            {
                cyclesCompleted++;
                Debug.Log("[Flow] HQ arrive cycles=" + cyclesCompleted);
            }

            if (cyclesCompleted >= targetCycles)
            {
                Debug.Log("[Flow] DONE cycles=" + cyclesCompleted);
                Stop();
                yield break;
            }

            var hub = FindObjectOfType<HubScreen>();
            if (hub == null || hub.sortieButton == null)
            {
                Debug.Log("[Flow] HQ FAIL sortieButton missing");
                Stop();
                yield break;
            }
            Debug.Log("[Flow] HQ sortie");
            hub.sortieButton.onClick.Invoke();
        }

        private IEnumerator DriveBattle()
        {
            bool endSeen = false;
            void Handler(string message, string stackTrace, LogType type)
            {
                if (message.StartsWith("[AutoPilot] END")) endSeen = true;
            }
            Application.logMessageReceived += Handler;
            float waited = 0f;
            while (!endSeen && waited < timeoutPerScene && !stopped)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }
            Application.logMessageReceived -= Handler;
            if (stopped) yield break;
            if (!endSeen)
            {
                Debug.Log("[Flow] STUCK scene=Battle (no AutoPilot END)");
                Stop();
                yield break;
            }

            // T8.1: END 를 이미 확인했으면 마무리 대기 중에 SceneTimeout 이 따로 만료돼
            // Stop()으로 이 코루틴까지 죽이는 경합을 막는다(재시도 버튼을 못 누르고 멈추는 버그).
            if (timeoutRoutine != null)
            {
                StopCoroutine(timeoutRoutine);
                timeoutRoutine = null;
            }

            yield return new WaitForSeconds(stepDelay);
            var controller = FindObjectOfType<BattleController>();
            var hud = controller != null ? controller.hud : null;
            if (hud == null || hud.retryButton == null)
            {
                Debug.Log("[Flow] Battle FAIL retryButton missing");
                Stop();
                yield break;
            }
            Debug.Log("[Flow] Battle retry/continue");
            hud.retryButton.onClick.Invoke();
        }

        private void Stop()
        {
            if (stopped) return;
            stopped = true;
            PlayerPrefs.SetInt(PrefsKey, 0);
            if (timeoutRoutine != null) StopCoroutine(timeoutRoutine);
            StopAllCoroutines();
        }
    }
}
