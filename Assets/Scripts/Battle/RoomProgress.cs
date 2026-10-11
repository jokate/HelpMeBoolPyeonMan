using System;
using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    // F-17: 방 하나(웨이브 연속)의 진행. 순수 C# — MonoBehaviour 밖에서 BattleState 를 웨이브마다 새로 만든다.
    // Squad 는 웨이브 사이에도 그대로 써서 HP·에너지를 유지하고, 적·Turn 은 웨이브마다 새 BattleState 로 초기화한다.
    public class RoomProgress
    {
        public RoomDefinition Room { get; }
        public Squad Squad { get; }

        readonly int? seed;
        readonly float hpMul;
        readonly float atkMul;
        readonly List<WaveDefinition> waves = new List<WaveDefinition>();

        public int WaveIndex { get; private set; } = -1;
        public int WaveCount => waves.Count;
        public BattleState Current { get; private set; }
        public bool IsCleared { get; private set; }
        public bool IsFailed { get; private set; }

        // T1: hpMul/atkMul 은 StageDefinition 의 몬스터 배율. 기본 1f 라 기존 호출은 그대로 동작한다.
        public RoomProgress(RoomDefinition room, Squad squad, int? seed = null, float hpMul = 1f, float atkMul = 1f)
        {
            Room = room ?? throw new ArgumentNullException(nameof(room));
            Squad = squad ?? throw new ArgumentNullException(nameof(squad));
            this.seed = seed;
            this.hpMul = hpMul;
            this.atkMul = atkMul;
            if (room.waves != null)
            {
                foreach (var w in room.waves)
                    if (w != null) waves.Add(w);
            }
        }

        public bool HasNextWave => !IsCleared && !IsFailed && WaveIndex + 1 < waves.Count;

        // 다음 웨이브의 BattleState 를 만들어 반환한다. 호출측이 StartBattle() 을 부른다.
        // 더 진행할 웨이브가 없으면 null.
        public BattleState StartNextWave()
        {
            if (!HasNextWave) return null;
            WaveIndex++;
            WaveDefinition wave = waves[WaveIndex];
            Current = new BattleState(Squad, wave != null ? wave.monsters : null, seed, hpMul, atkMul);
            return Current;
        }

        // Current 가 끝난 뒤(Current.IsOver) 호출한다.
        // 패배면 IsFailed, 마지막 웨이브의 승리면 IsCleared 로 표시한다. 그 외(중간 웨이브 승리)는 표시하지 않고
        // 호출측이 StartNextWave() 로 다음 웨이브를 이어간다.
        public void CompleteCurrentWave()
        {
            if (Current == null || !Current.IsOver) return;
            if (Current.Phase == BattlePhase.Defeat)
            {
                IsFailed = true;
                return;
            }
            if (Current.Phase == BattlePhase.Victory && WaveIndex + 1 >= waves.Count)
            {
                IsCleared = true;
            }
        }
    }
}
