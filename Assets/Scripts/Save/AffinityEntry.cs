using System;
using System.Collections.Generic;

namespace BoolPyeon.Save
{
    // F-29: 요원 한 명의 저장된 호감도 상태.
    [Serializable]
    public class AffinityEntry
    {
        public string agentId;
        public int points;
        public int lastTalkRun = -1;
        public List<int> claimedStages = new List<int>();
    }
}
