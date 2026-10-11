using UnityEngine;
using BoolPyeon.Battle;

namespace BoolPyeon.BattleFlow
{
    // F-14 검증: 사람 입력 없이 BattleController 를 끝까지 돌리는 디버그 전용 자동 진행기.
    // PlayerPrefs "BattleAutoPilot"==1 이거나 이 컴포넌트의 enabled 가 켜져 있으면 동작한다.
    public class BattleAutoPilot : MonoBehaviour
    {
        public const string PrefsKey = "BattleAutoPilot";

        public enum DefensePolicy { AlwaysSuccess, AlwaysFail, Alternate }

        public BattleController controller;
        public DefensePolicy defensePolicy = DefensePolicy.AlwaysSuccess;
        public int energyPerPhase = 3;
        [Tooltip("자동 진행일 때 TitleCut 대기 시간에 곱하는 배율")]
        public float titleCutScale = 0.1f;

        bool alternateToggle;
        int guardedTurn = -1;
        int guardedMemberIndex = -1;

        public bool IsActive() => enabled || PlayerPrefs.GetInt(PrefsKey, 0) == 1;

        public bool ResolveDefense()
        {
            bool ok;
            switch (defensePolicy)
            {
                case DefensePolicy.AlwaysFail: ok = false; break;
                case DefensePolicy.Alternate: alternateToggle = !alternateToggle; ok = alternateToggle; break;
                default: ok = true; break;
            }
            Debug.Log($"[AutoPilot] defense={(ok ? "success" : "fail")}");
            return ok;
        }

        // T8: 지금 차례인 멤버 기준으로 에너지를 얻는다. 로그에 멤버 id 를 남긴다.
        public int EnergyGain(BattleState state, int memberIndex)
        {
            string id = (state != null && memberIndex >= 0 && memberIndex < state.Members.Count)
                ? state.Members[memberIndex].Definition?.id
                : null;
            Debug.Log($"[AutoPilot] energy member={id} +{energyPerPhase}");
            return energyPerPhase;
        }

        // 공격을 우선하지만, 체력이 70% 이하면 이번 턴에 한 번만 방어(guardSkill)로 Guarded 상태를 걸어
        // 다음 적 턴 피해를 줄이고 나머지 에너지는 공격에 쓴다.
        // 예전엔 항상 공격만 골라 적 일반 공격을 그대로 다 맞았고(방어는 특수 공격에만 있는 미니게임 경로),
        // 그다음엔 CanCommand(guard)가 계속 true라 방어만 반복 소모해 공격을 못 해 적을 못 줄였다.
        // 이제 (턴, 멤버)당 한 번만 방어하도록 기억해 둔다 — 멤버가 1명인 튜토리얼처럼 교체할 사람이
        // 없으면 몇 턴 안에 죽어 자동 진행이 패배 재시도 루프에 갇히는 문제를 줄인다.
        public SkillDefinition ChooseCommand(BattleState state)
        {
            if (state == null || controller == null) return null;
            var activeMember = state.ActiveMember;
            var def = activeMember?.Definition;
            SkillDefinition attack = def?.attackSkill ?? controller.character?.attackSkill;
            SkillDefinition guard = def?.guardSkill ?? controller.character?.guardSkill;
            string id = def?.id;
            var combatant = activeMember?.Combatant;
            bool lowHp = combatant != null && combatant.MaxHp > 0 && combatant.Hp <= combatant.MaxHp * 0.7f;
            bool alreadyGuardedThisTurn = guardedTurn == state.Turn && guardedMemberIndex == state.ActiveMemberIndex;

            if (lowHp && !alreadyGuardedThisTurn && guard != null && state.CanCommand(guard))
            {
                guardedTurn = state.Turn;
                guardedMemberIndex = state.ActiveMemberIndex;
                Debug.Log($"[AutoPilot] command=guard member={id} hp={combatant.Hp}/{combatant.MaxHp}");
                return guard;
            }
            if (attack != null && state.CanCommand(attack))
            {
                Debug.Log($"[AutoPilot] command=attack member={id}");
                return attack;
            }
            if (!alreadyGuardedThisTurn && guard != null && state.CanCommand(guard))
            {
                guardedTurn = state.Turn;
                guardedMemberIndex = state.ActiveMemberIndex;
                Debug.Log($"[AutoPilot] command=guard(fallback) member={id}");
                return guard;
            }
            Debug.Log($"[AutoPilot] command=end turn member={id}");
            return null;
        }

        // T2 임시: 보스 쪽으로 BFS 해 다음 한 걸음의 방향을 고른다. 갈 곳이 없으면 Vector2Int.zero.
        public Vector2Int ChooseMazeDirection(StageMaze maze)
        {
            if (maze == null)
            {
                Debug.LogWarning("[AutoPilot] 미로 없음");
                return Vector2Int.zero;
            }
            var movable = maze.GetMovableDirections();
            if (movable.Count == 0)
            {
                Debug.LogWarning("[AutoPilot] 이동 가능한 방향 없음");
                return Vector2Int.zero;
            }

            var parent = new System.Collections.Generic.Dictionary<Vector2Int, Vector2Int>();
            var visited = new System.Collections.Generic.HashSet<Vector2Int> { maze.CurrentPos };
            var queue = new System.Collections.Generic.Queue<Vector2Int>();
            queue.Enqueue(maze.CurrentPos);
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            bool found = false;
            while (queue.Count > 0 && !found)
            {
                var cur = queue.Dequeue();
                foreach (var d in dirs)
                {
                    if (!maze.HasPassage(cur, d)) continue;
                    var np = cur + d;
                    if (visited.Contains(np)) continue;
                    visited.Add(np);
                    parent[np] = cur;
                    if (np == maze.BossPos) { found = true; break; }
                    queue.Enqueue(np);
                }
            }

            Vector2Int chosen = movable[0];
            if (found)
            {
                var node = maze.BossPos;
                while (parent.TryGetValue(node, out var prev) && prev != maze.CurrentPos)
                    node = prev;
                chosen = node - maze.CurrentPos;
            }
            Debug.Log($"[AutoPilot] maze move dir={chosen}");
            return chosen;
        }

        public int ChooseUpgrade()
        {
            Debug.Log("[AutoPilot] upgrade=0");
            return 0;
        }
    }
}
