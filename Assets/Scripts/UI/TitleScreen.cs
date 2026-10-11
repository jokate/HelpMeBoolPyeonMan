using BoolPyeon.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BoolPyeon.UI
{
    /// <summary>
    /// Title 씬의 시작 버튼을 눌러 Battle 씬으로 전환한다.
    /// </summary>
    public class TitleScreen : MonoBehaviour
    {
        public Button startButton;
        public string battleSceneName = "Battle";
        public TitleIntro intro;
        public TitleMenu menu;

        private bool isTransitioning;

        private void Start()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStart);
            }
        }

        private void OnStart()
        {
            if (menu != null && menu.IsOpen)
            {
                return;
            }

            if (isTransitioning)
            {
                return;
            }

            if (intro != null && !intro.IsReady)
            {
                intro.SkipIntro();
                return;
            }

            isTransitioning = true;

            if (startButton != null)
            {
                startButton.interactable = false;
            }

            if (intro != null)
            {
                intro.PlayExit(DoLoad);
            }
            else
            {
                DoLoad();
            }
        }

        private void DoLoad()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Battle);
            }

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadSingle(battleSceneName);
            }
            else
            {
                SceneManager.LoadScene(battleSceneName);
            }
        }
    }
}
