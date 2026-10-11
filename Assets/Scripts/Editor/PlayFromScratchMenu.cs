using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using BoolPyeon.BattleFlow;
using BoolPyeon.Save;

namespace BoolPyeon.EditorTools
{
    // T18(설계) '처음부터 플레이': 세이브를 백업·초기화한 뒤 Boot 씬을 열고 Play 에 들어간다.
    // 씬/프리팹은 만들지 않는다 — FlowAutoPilot 은 RuntimeInitializeOnLoadMethod 로 스스로 생긴다.
    public static class PlayFromScratchMenu
    {
        private const string BootScenePath = "Assets/Scenes/Boot.unity";

        [MenuItem("BoolPyeon/Play From Scratch")]
        public static void PlayFreshManual() => PlayFresh(false);

        [MenuItem("BoolPyeon/Play From Scratch (Auto)")]
        public static void PlayFreshAuto() => PlayFresh(true);

        // T11.1: 보스 연출(HP 바 등)을 확인하기 위해 미로를 다 돌지 않고 보스 방으로 바로 들어간다.
        [MenuItem("BoolPyeon/Play Boss Room (Auto)")]
        public static void PlayBossRoomAuto()
        {
            PlayerPrefs.SetInt(BattleController.DebugBossRoomKey, 1);
            PlayerPrefs.Save();
            PlayFresh(true);
        }

        // T6.2: 신규 몬스터 3종 인텐트 예고를 실제 전투 흐름에서 확인하기 위해 엘리트 방으로 바로 들어간다.
        [MenuItem("BoolPyeon/Play Elite Room (Auto)")]
        public static void PlayEliteRoomAuto()
        {
            PlayerPrefs.SetInt(BattleController.DebugEliteRoomKey, 1);
            PlayerPrefs.Save();
            PlayFresh(true);
        }

        [MenuItem("BoolPyeon/Restore Save Backup")]
        public static void RestoreBackup()
        {
            string path = SaveService.Default.FilePath;
            string backup = Path.Combine(Path.GetDirectoryName(path), "save.backup.json");
            if (!File.Exists(backup))
            {
                Debug.LogWarning("[Flow] Restore Save Backup: 백업 파일이 없습니다 (" + backup + ")");
                return;
            }
            File.Copy(backup, path, true);
            Debug.Log("[Flow] Restore Save Backup: " + backup + " -> " + path);
        }

        public static void PlayFresh(bool auto)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[Flow] PlayFresh: 이미 Play 중입니다.");
                return;
            }

            string path = SaveService.Default.FilePath;
            string backup = Path.Combine(Path.GetDirectoryName(path), "save.backup.json");
            if (File.Exists(path))
            {
                File.Copy(path, backup, true);
                Debug.Log("[Flow] save backed up -> " + backup);
            }
            SaveService.Default.Delete();

            PlayerPrefs.SetInt(FlowAutoPilot.PrefsKey, auto ? 1 : 0);
            PlayerPrefs.SetInt(BattleAutoPilot.PrefsKey, auto ? 1 : 0);
            PlayerPrefs.Save();

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning("[Flow] PlayFresh: 저장 취소로 중단합니다.");
                return;
            }

            var opened = EditorSceneManager.OpenScene(BootScenePath);
            if (!opened.IsValid())
            {
                Debug.LogError("[Flow] PlayFresh: Boot 씬을 열지 못했습니다 (" + BootScenePath + ")");
                return;
            }

            Debug.Log("[Flow] PlayFresh start auto=" + auto);
            EditorApplication.isPlaying = true;
        }
    }
}
