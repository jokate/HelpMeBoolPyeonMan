using System.Collections.Generic;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 출제 풀에서 용도(∩ 불편 유형)가 맞는 미니게임만 고른다 (F-31).
    /// </summary>
    public static class MiniGamePoolFilter
    {
        public static List<MiniGameDefinition> Filter(IEnumerable<MiniGameDefinition> pool, MiniGameUsage usage,
            MiniGameCategory? category = null)
        {
            var result = new List<MiniGameDefinition>();
            if (pool == null) return result;
            foreach (MiniGameDefinition d in pool)
            {
                if (d == null || !d.Has(usage)) continue;
                if (category.HasValue && d.category != category.Value) continue;
                if (!result.Contains(d)) result.Add(d);
            }
            return result;
        }
    }
}
