using UnityEngine;

namespace BoolPyeon.MiniGames.Debug
{
    /// <summary>
    /// Title 씬의 Start 버튼에서 호출: MiniGameRunner 로 정의 하나를 돌리고
    /// 결과를 콘솔에 로그로 남긴다.
    /// </summary>
    public class MiniGameRunnerDebug : MonoBehaviour
    {
        public MiniGameRunner runner;
        public MiniGameDefinition definition;

        [Tooltip("Play 중 이 키를 누르면 RunDebug 를 호출한다 (None 이면 끔).")]
        public KeyCode runKey = KeyCode.T;

        private void Update()
        {
            if (runKey != KeyCode.None && UnityEngine.Input.GetKeyDown(runKey))
            {
                RunDebug();
            }
        }

        [ContextMenu("Run Debug")]
        public void RunDebug()
        {
            if (runner == null || definition == null)
            {
                UnityEngine.Debug.LogWarning("MiniGameRunnerDebug: runner 또는 definition 이 비어 있습니다.");
                return;
            }
            runner.Run(definition, MiniGameContext.Default, OnResult);
        }

        private void OnResult(MiniGameResult result)
        {
            UnityEngine.Debug.Log($"MiniGame result: {result}");
        }
    }
}
