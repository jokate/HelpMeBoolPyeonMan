using System;
using System.IO;
using UnityEngine;

namespace BoolPyeon.Save
{
    // F-20: JSON(persistentDataPath/save.json) 기반 저장. 순수 C# — dir 을 주입할 수 있어 테스트가
    // 임시 폴더를 쓰고 실제 저장을 오염시키지 않는다.
    public class SaveService
    {
        private static SaveService defaultInstance;
        public static SaveService Default => defaultInstance ??= new SaveService(Application.persistentDataPath);

        public readonly string Directory;
        public readonly string FilePath;
        public readonly string BackupPath;

        private SaveData cached;
        public SaveData Data => cached ??= Load();

        public SaveService(string dir, string fileName = "save.json")
        {
            Directory = dir;
            FilePath = Path.Combine(dir, fileName);
            BackupPath = FilePath + ".bak";
        }

        public SaveData Load()
        {
            SaveData data;
            try
            {
                if (!File.Exists(FilePath))
                {
                    data = new SaveData();
                }
                else
                {
                    string json = File.ReadAllText(FilePath);
                    data = JsonUtility.FromJson<SaveData>(json);
                    if (data == null) throw new Exception("empty save data");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] load failed, using default: {e.Message}");
                TryBackupCorrupt();
                data = new SaveData();
            }
            data = Migrate(data);
            cached = data;
            return data;
        }

        private void TryBackupCorrupt()
        {
            try
            {
                if (File.Exists(FilePath))
                    File.Copy(FilePath, BackupPath, true);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] backup failed: {e.Message}");
            }
        }

        // T2: 저장 스키마 버전. v1→v2 는 highestClearedStage·stageNumber 필드가 추가됐다.
        public const int CurrentVersion = 2;

        private static SaveData Migrate(SaveData data)
        {
            if (data == null) data = new SaveData();
            if (data.permanentUpgrades == null) data.permanentUpgrades = new System.Collections.Generic.List<UpgradeLevelEntry>();
            if (data.settings == null) data.settings = new SettingsData();
            if (data.run == null) data.run = new RunSnapshot();
            if (data.equippedModules == null) data.equippedModules = new System.Collections.Generic.List<string>();
            if (data.moduleSlots <= 0) data.moduleSlots = 1;
            if (data.affinities == null) data.affinities = new System.Collections.Generic.List<AffinityEntry>();
            if (data.unlockedModules == null) data.unlockedModules = new System.Collections.Generic.List<string>();
            if (data.squadIds == null) data.squadIds = new System.Collections.Generic.List<string>();
            // F-23: 예전 저장(진행 흔적이 있음)은 튜토리얼을 다시 보여주지 않는다.
            if (!data.tutorialDone && (data.runsStarted > 0 || data.hasRun || data.crystals > 0 || data.permanentUpgrades.Count > 0))
                data.tutorialDone = true;
            // T2: v1→v2 - 새 필드를 안전한 기본값으로 보정한다.
            if (data.version < 2)
            {
                if (data.highestClearedStage < 0) data.highestClearedStage = 0;
                if (data.run != null && data.run.stageNumber <= 0) data.run.stageNumber = 1;
            }
            if (data.highestClearedStage < 0) data.highestClearedStage = 0;
            if (data.run != null && data.run.stageNumber <= 0) data.run.stageNumber = 1;
            data.version = CurrentVersion;
            return data;
        }

        public void Save(SaveData data)
        {
            cached = data;
            try
            {
                if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);
                string json = JsonUtility.ToJson(data, true);
                string tmp = FilePath + ".tmp";
                File.WriteAllText(tmp, json);
                if (File.Exists(FilePath)) File.Delete(FilePath);
                File.Move(tmp, FilePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] save failed: {e.Message}");
            }
        }

        public void Delete()
        {
            try
            {
                if (File.Exists(FilePath)) File.Delete(FilePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] delete failed: {e.Message}");
            }
            cached = new SaveData();
        }

        public void AddCrystals(int n)
        {
            Data.crystals += n;
            Save(Data);
        }

        public int GetUpgradeLevel(string id)
        {
            foreach (var e in Data.permanentUpgrades)
                if (e.id == id) return e.level;
            return 0;
        }

        public void SetUpgradeLevel(string id, int lv)
        {
            foreach (var e in Data.permanentUpgrades)
            {
                if (e.id == id) { e.level = lv; Save(Data); return; }
            }
            Data.permanentUpgrades.Add(new UpgradeLevelEntry { id = id, level = lv });
            Save(Data);
        }

        public void SaveRun(BoolPyeon.Battle.RunState state, int combatRoomCount, int eliteRoomCount)
        {
            Data.run = RunSaveMapper.Capture(state, combatRoomCount, eliteRoomCount);
            Data.hasRun = true;
            Save(Data);
        }

        public void ClearRun()
        {
            Data.hasRun = false;
            Data.run = new RunSnapshot();
            Save(Data);
        }

        // T2: 스테이지 클리어 진행을 기록한다. 낮은 스테이지를 다시 클리어해도 값이 내려가지 않는다.
        public void MarkStageCleared(int stageNumber)
        {
            if (stageNumber > Data.highestClearedStage)
                Data.highestClearedStage = stageNumber;
            Save(Data);
            Debug.Log($"[Stage] progress highest={Data.highestClearedStage}");
        }

        // F-23: 튜토리얼 런 완료 플래그를 세우고 저장한다.
        public void CompleteTutorial()
        {
            Data.tutorialDone = true;
            Save(Data);
            Debug.Log("[Tutorial] complete saved tutorialDone=true");
        }
    }
}
