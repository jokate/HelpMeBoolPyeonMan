using System;
using System.Collections.Generic;

namespace BoolPyeon.Save
{
    // F-20: 저장 파일 하나 전체. version 은 지금 v1 뿐이고, Migrate 훅을 통해 앞으로의 변경에 대응한다.
    [Serializable]
    public class SaveData
    {
        public int version = 2;
        public int crystals;
        public List<UpgradeLevelEntry> permanentUpgrades = new List<UpgradeLevelEntry>();
        public SettingsData settings = new SettingsData();
        public bool hasRun;
        public RunSnapshot run = new RunSnapshot();
        // F-28: 장착한 모듈(ModuleDefinition.name) 목록과 슬롯 수. 기본 1개(D-13).
        public List<string> equippedModules = new List<string>();
        public int moduleSlots = 1;
        // F-29: 요원별 호감도, 보상으로 해금된 모듈, 출격(런 시작) 누적 횟수.
        public List<AffinityEntry> affinities = new List<AffinityEntry>();
        public List<string> unlockedModules = new List<string>();
        public int runsStarted;
        // F-23: 튜토리얼 런을 완료했는지. false 면 BattleController 가 맵 대신 튜토리얼 방을 진행한다.
        public bool tutorialDone;
        // F-30: 후방 지원 스쿼드 편성(CharacterDefinition.id, 최대 3, Members[0] 이 리더/전투 참가자).
        // 비어 있으면 ['hero'] 로 취급한다(SquadService).
        public List<string> squadIds = new List<string>();
        // T2: 지금까지 클리어한 최고 스테이지 번호. 새 런의 시작 스테이지 계산에 쓴다. 기본 0(아직 없음).
        public int highestClearedStage;
    }
}
