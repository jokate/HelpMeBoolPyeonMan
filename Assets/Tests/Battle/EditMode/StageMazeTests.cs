using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.Battle.Tests
{
    public class StageMazeTests
    {
        [Test]
        public void Generate_SameSeed_ProducesSameMaze()
        {
            var a = StageMaze.Generate(12345, 1, 1);
            var b = StageMaze.Generate(12345, 1, 1);

            for (int x = 0; x < a.Width; x++)
            {
                for (int y = 0; y < a.Height; y++)
                {
                    var ra = a.GetRoom(x, y);
                    var rb = b.GetRoom(x, y);
                    Assert.AreEqual(ra.type, rb.type, $"({x},{y}) type");
                    Assert.AreEqual(ra.roomIndex, rb.roomIndex, $"({x},{y}) roomIndex");
                    Assert.AreEqual(ra.up, rb.up, $"({x},{y}) up");
                    Assert.AreEqual(ra.down, rb.down, $"({x},{y}) down");
                    Assert.AreEqual(ra.left, rb.left, $"({x},{y}) left");
                    Assert.AreEqual(ra.right, rb.right, $"({x},{y}) right");
                }
            }
            Assert.AreEqual(a.BossPos, b.BossPos);
        }

        [Test]
        public void Generate_AllSeeds_BossReachableFromStart()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                var maze = StageMaze.Generate(seed, 1, 1);
                Assert.IsTrue(BfsReachable(maze, maze.StartPos, maze.BossPos), $"seed {seed} boss unreachable");
            }
        }

        [Test]
        public void Generate_RoomCounts_StartOneBossOneMiddleTen()
        {
            var maze = StageMaze.Generate(5, 1, 1);
            int start = 0, boss = 0, middle = 0;
            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    var room = maze.GetRoom(x, y);
                    if (room.type == MazeRoomType.Start) start++;
                    else if (room.type == MazeRoomType.Boss) boss++;
                    else if (room.type != MazeRoomType.Empty) middle++;
                }
            }
            Assert.AreEqual(1, start);
            Assert.AreEqual(1, boss);
            Assert.AreEqual(10, middle);
        }

        [Test]
        public void TryMove_IntoWallOrOutOfGrid_ReturnsFalse_PositionUnchanged()
        {
            var maze = StageMaze.Generate(9, 1, 1);
            var before = maze.CurrentPos;
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            bool anyWall = false;
            foreach (var d in dirs)
            {
                if (!maze.HasPassage(maze.CurrentPos, d))
                {
                    anyWall = true;
                    Assert.IsFalse(maze.TryMove(d));
                    Assert.AreEqual(before, maze.CurrentPos);
                }
            }
            // (0,0) 은 격자 밖으로 나가는 방향(아래/왼쪽)이 항상 막혀 있다.
            Assert.IsTrue(anyWall);
        }

        [Test]
        public void TryMove_FromUnclearedRoom_IsRejected()
        {
            var maze = StageMaze.Generate(3, 1, 1);
            var movable = maze.GetMovableDirections();
            Assert.IsTrue(movable.Count > 0);
            var dir = movable[0];
            Assert.IsTrue(maze.TryMove(dir));

            var current = maze.Current;
            if (current.type != MazeRoomType.Empty)
            {
                // 클리어하지 않은 방에서는 이동 목록이 비어야 하고 TryMove 도 실패해야 한다.
                Assert.AreEqual(0, maze.GetMovableDirections().Count);
                var anyDir = new Vector2Int(0, 1);
                Assert.IsFalse(maze.TryMove(anyDir));
            }
        }

        [Test]
        public void ReachBossAndClear_SetsIsFinished()
        {
            var maze = StageMaze.Generate(20, 1, 1);
            var path = ShortestPath(maze, maze.StartPos, maze.BossPos);
            for (int i = 1; i < path.Count; i++)
            {
                var dir = path[i] - path[i - 1];
                if (!maze.Current.cleared) maze.CompleteCurrent();
                Assert.IsTrue(maze.TryMove(dir), $"step {i} to {path[i]} failed");
            }
            Assert.IsFalse(maze.IsFinished);
            maze.CompleteCurrent();
            Assert.IsTrue(maze.IsFinished);
        }

        [Test]
        public void TryMove_Backtrack_IsAllowed()
        {
            var maze = StageMaze.Generate(4, 1, 1);
            var movable = maze.GetMovableDirections();
            var dir = movable[0];
            var start = maze.CurrentPos;
            Assert.IsTrue(maze.TryMove(dir));
            if (!maze.Current.cleared) maze.CompleteCurrent();
            Assert.IsTrue(maze.TryMove(dir * -1));
            Assert.AreEqual(start, maze.CurrentPos);
        }

#if UNITY_EDITOR
        [Test]
        public void Stage2_Asset_GeneratesValidMaze()
        {
            var stage = UnityEditor.AssetDatabase.LoadAssetAtPath<StageDefinition>("Assets/Data/Battle/Stage2/Stage_2.asset");
            Assert.IsNotNull(stage, "Stage_2 asset missing");
            Assert.AreEqual(2, stage.stageNumber);

            Assert.IsNotNull(stage.bossRoom, "bossRoom null");
            CheckRoom(stage.bossRoom);
            foreach (var room in stage.combatRooms)
            {
                Assert.IsNotNull(room, "combatRoom null");
                CheckRoom(room);
            }
            foreach (var room in stage.eliteRooms)
            {
                Assert.IsNotNull(room, "eliteRoom null");
                CheckRoom(room);
            }

            bool hasBossMonster = false;
            bool hasPhase2GuardOrMagic = false;
            foreach (var wave in stage.bossRoom.waves)
            {
                foreach (var mon in wave.monsters)
                {
                    if (mon != null && mon.isBoss)
                    {
                        hasBossMonster = true;
                        Assert.Greater(mon.phase2Threshold, 0f, "boss phase2Threshold should be > 0");
                        bool anyGuardOrMagic = false;
                        foreach (var act in mon.phase2Actions)
                        {
                            if (act.kind == MonsterActionKind.Guard || act.kind == MonsterActionKind.Magic) anyGuardOrMagic = true;
                        }
                        Assert.IsTrue(anyGuardOrMagic, "phase2Actions should include Guard or Magic");
                        hasPhase2GuardOrMagic = anyGuardOrMagic;
                    }
                }
            }
            Assert.IsTrue(hasBossMonster, "boss wave should contain at least one isBoss monster");
            Assert.IsTrue(hasPhase2GuardOrMagic);

            for (int seed = 0; seed < 50; seed++)
            {
                var maze = StageMaze.Generate(seed, stage.combatRooms.Count, stage.eliteRooms.Count);
                Assert.IsTrue(BfsReachable(maze, maze.StartPos, maze.BossPos), $"seed {seed} boss unreachable (stage2)");
            }
        }

        static void CheckRoom(RoomDefinition room)
        {
            Assert.IsNotNull(room.waves, "room.waves null");
            Assert.Greater(room.waves.Count, 0, "room has no waves");
            foreach (var wave in room.waves)
            {
                Assert.IsNotNull(wave, "wave null");
                Assert.IsNotNull(wave.monsters, "wave.monsters null");
                Assert.Greater(wave.monsters.Count, 0, "wave has no monsters");
                foreach (var mon in wave.monsters)
                {
                    Assert.IsNotNull(mon, "monster null");
                }
            }
        }
#endif

        static bool BfsReachable(StageMaze maze, Vector2Int from, Vector2Int to)
        {
            var visited = new HashSet<Vector2Int> { from };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(from);
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (cur == to) return true;
                foreach (var d in dirs)
                {
                    if (!maze.HasPassage(cur, d)) continue;
                    var np = cur + d;
                    if (visited.Contains(np)) continue;
                    visited.Add(np);
                    queue.Enqueue(np);
                }
            }
            return false;
        }

        static List<Vector2Int> ShortestPath(StageMaze maze, Vector2Int from, Vector2Int to)
        {
            var parent = new Dictionary<Vector2Int, Vector2Int>();
            var visited = new HashSet<Vector2Int> { from };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(from);
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (cur == to) break;
                foreach (var d in dirs)
                {
                    if (!maze.HasPassage(cur, d)) continue;
                    var np = cur + d;
                    if (visited.Contains(np)) continue;
                    visited.Add(np);
                    parent[np] = cur;
                    queue.Enqueue(np);
                }
            }
            var path = new List<Vector2Int> { to };
            var node = to;
            while (node != from)
            {
                node = parent[node];
                path.Add(node);
            }
            path.Reverse();
            return path;
        }
    }
}
