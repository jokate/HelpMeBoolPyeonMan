using System;

namespace BoolPyeon.Battle
{
    // D-3: 상한 10. D-11: 스쿼드 공용 풀.
    public class EnergyPool
    {
        public const int DefaultCap = 10;

        public int Current { get; private set; }
        public int Cap { get; }

        public event Action<int> Changed;

        public EnergyPool(int cap = DefaultCap)
        {
            Cap = Math.Max(0, cap);
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Set(Math.Min(Cap, Current + amount));
        }

        public bool CanSpend(int cost) => cost >= 0 && Current >= cost;

        public bool TrySpend(int cost)
        {
            if (!CanSpend(cost)) return false;
            Set(Current - cost);
            return true;
        }

        void Set(int value)
        {
            if (value == Current) return;
            Current = value;
            Changed?.Invoke(Current);
        }
    }
}
