using System;
using System.Collections.Generic;

namespace BoolPyeon.Save
{
    // F-20: 방 사이 시점의 진행 중 런 스냅샷. 맵은 시드로 다시 만들고 경로를 재생해 복원한다.
    [Serializable]
    public class RunSnapshot
    {
        public int seed;
        public int combatRoomCount;
        public int eliteRoomCount;
        // 구버전(갈래 맵) 저장본 호환용 필드. 더 이상 쓰지 않고 복원 시 무시한다.
        public List<int> pathIndices = new List<int>();
        // T2: 격자 미로 저장. 칸 번호(y*Width+x)로 방문 순서·클리어한 칸을 남긴다.
        public List<int> pathCells = new List<int>();
        public List<int> clearedCells = new List<int>();
        // -1 이면 null(아직 전투 전, 가득 찬 상태). 옛 세이브 호환용(멤버0 HP).
        public int currentHp = -1;
        // T2: 멤버별 HP. 각 값이 -1 이면 null(가득 찬 상태). 비어 있으면 옛 세이브로 보고 currentHp 를 쓴다.
        public int[] currentHps = new int[0];
        // UpgradeDefinition.name, 적용 순서
        public List<string> upgradeIds = new List<string>();
        // F-21: 이번 런 누적 처치 수·정화 결정. 기본값 0 이라 기존 세이브와 호환된다.
        public int kills;
        public int crystalsEarned;
        // T2: 이 런이 진행 중인 스테이지 번호. 기본값 1(구버전 세이브 호환).
        public int stageNumber = 1;
    }
}
