using UnityEngine;
using BoolPyeon.Battle;

namespace BoolPyeon.Meta
{
    // F-27: 허브 강화 요원 메뉴에서 파는 영구 능력치 강화 하나. 단계별 costs[]/values[] 를 누적한다.
    [CreateAssetMenu(menuName = "BoolPyeon/Meta/Permanent Upgrade", fileName = "PU_")]
    public class PermanentUpgradeDefinition : ScriptableObject
    {
        public string displayName;
        public PermanentUpgradeKind kind;
        // 강화 행 아이콘(없으면 행에서 숨긴다)
        public Sprite icon;
        public int[] costs = System.Array.Empty<int>();
        public int[] values = System.Array.Empty<int>();

        public int MaxLevel => costs != null ? costs.Length : 0;

        public int CostFor(int level)
        {
            if (costs == null || level < 0 || level >= costs.Length) return -1;
            return costs[level];
        }

        public int TotalValue(int level)
        {
            if (values == null) return 0;
            int total = 0;
            int n = Mathf.Min(level, values.Length);
            for (int i = 0; i < n; i++) total += values[i];
            return total;
        }
    }
}
