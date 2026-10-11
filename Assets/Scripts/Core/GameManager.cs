using UnityEngine;

namespace BoolPyeon.Core
{
    public enum GameState
    {
        Boot,
        Title,
        HQ,
        Map,
        Battle,
        Result
    }

    /// <summary>
    /// 게임 전역 상태를 들고 있는 싱글턴.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Boot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
        }

        public void ChangeState(GameState state)
        {
            State = state;
        }
    }
}
