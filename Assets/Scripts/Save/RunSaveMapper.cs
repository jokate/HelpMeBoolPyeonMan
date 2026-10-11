using System;
using System.Collections.Generic;
using UnityEngine;
using BoolPyeon.Battle;

namespace BoolPyeon.Save
{
    // F-20: RunState <-> RunSnapshot. 맵과 스킬 목록을 직접 직렬화하지 않고, 시드로 맵을 다시 만들고
    // 경로를 재생하고 강화를 이름 순서대로 다시 Apply해 되살린다.
    public static class RunSaveMapper
    {
        public static RunSnapshot Capture(RunState state, int combatRoomCount, int eliteRoomCount)
        {
            var snap = new RunSnapshot
            {
                combatRoomCount = combatRoomCount,
                eliteRoomCount = eliteRoomCount,
                currentHp = state.CurrentHp ?? -1,
                currentHps = ToHpArray(state.CurrentHps),
                kills = state.Kills,
                crystalsEarned = state.CrystalsEarned,
                stageNumber = state.StageNumber,
            };
            if (state.Map != null)
            {
                snap.seed = state.Map.Seed;
                foreach (var p in state.Map.VisitedPath)
                    snap.pathCells.Add(state.Map.ToCellIndex(p));
                for (int x = 0; x < state.Map.Width; x++)
                {
                    for (int y = 0; y < state.Map.Height; y++)
                    {
                        var room = state.Map.GetRoom(x, y);
                        if (room != null && room.cleared)
                            snap.clearedCells.Add(state.Map.ToCellIndex(new Vector2Int(x, y)));
                    }
                }
            }
            else
            {
                snap.seed = state.Seed;
            }
            foreach (var def in state.Acquired)
            {
                if (def != null) snap.upgradeIds.Add(def.name);
            }
            return snap;
        }

        static int[] ToHpArray(List<int?> hps)
        {
            if (hps == null || hps.Count == 0) return new int[0];
            var arr = new int[hps.Count];
            for (int i = 0; i < hps.Count; i++) arr[i] = hps[i] ?? -1;
            return arr;
        }

        public static RunState Restore(RunSnapshot snap, CharacterDefinition character, IList<UpgradeDefinition> pool)
        {
            return Restore(snap, new List<CharacterDefinition> { character }, pool);
        }

        // T2.1: 편성 전원을 받는 버전. RunState.CreateSquad가 이 순서대로 멤버 전원을 만든다.
        public static RunState Restore(RunSnapshot snap, IReadOnlyList<CharacterDefinition> squad, IList<UpgradeDefinition> pool)
        {
            var state = new RunState(squad);

            if (snap.upgradeIds != null && pool != null)
            {
                foreach (var id in snap.upgradeIds)
                {
                    UpgradeDefinition found = null;
                    foreach (var def in pool)
                    {
                        if (def != null && def.name == id) { found = def; break; }
                    }
                    if (found == null)
                    {
                        Debug.LogWarning($"[Save] restore: 강화 {id} 를 찾지 못해 건너뜁니다.");
                        continue;
                    }
                    state.Apply(found);
                }
            }

            state.Seed = snap.seed;
            state.Map = StageMaze.Generate(snap.seed, snap.combatRoomCount, snap.eliteRoomCount);
            state.Map.RestoreProgress(snap.pathCells, snap.clearedCells);

            // T2: 새 세이브(currentHps)가 있으면 멤버별로, 없으면(옛 세이브) 단일 HP 로 복원한다.
            if (snap.currentHps != null && snap.currentHps.Length > 0)
            {
                var hps = new List<int?>();
                int count = Math.Min(snap.currentHps.Length, squad != null ? squad.Count : snap.currentHps.Length);
                for (int i = 0; i < count; i++)
                {
                    var v = snap.currentHps[i];
                    hps.Add(v < 0 ? (int?)null : v);
                }
                state.RestoreHps(hps);
            }
            else
            {
                state.RestoreHp(snap.currentHp < 0 ? (int?)null : snap.currentHp);
            }
            state.RestoreProgress(snap.kills, snap.crystalsEarned);
            state.StageNumber = snap.stageNumber > 0 ? snap.stageNumber : 1;
            return state;
        }
    }
}
