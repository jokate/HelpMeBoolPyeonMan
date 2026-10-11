using UnityEngine;

namespace BoolPyeon.Core
{
    /// <summary>
    /// Boot 씬에서 GameManager/SceneLoader 를 확보한 뒤 Title 씬으로 전환한다.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        public string titleSceneName = "Title";

        private void Start()
        {
            EnsureGameManager();
            EnsureSceneLoader();
            GameManager.Instance.ChangeState(GameState.Title);
            SceneLoader.Instance.LoadSingle(titleSceneName);
        }

        private void EnsureGameManager()
        {
            if (GameManager.Instance == null)
            {
                new GameObject("GameManager").AddComponent<GameManager>();
            }
        }

        private void EnsureSceneLoader()
        {
            if (SceneLoader.Instance == null)
            {
                new GameObject("SceneLoader").AddComponent<SceneLoader>();
            }
        }
    }
}
