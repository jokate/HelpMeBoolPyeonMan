using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Battle
{
    // T2: 시드로 만든 5x5(기본) 격자 미로. 순수 C# — RunState.Current 에 붙어 씬 재로드 사이에도 유지된다.
    // 무작위 DFS 로 25칸 전체를 잇는 완전 미로를 만들고, 시작(0,0)에서 BFS 로 가장 먼 칸을 보스로 둔다.
    // 시작→보스 최단 경로 칸을 우선으로 중간 방 10개(엘리트2·휴식1·전투7)를 고르고, 나머지는 Empty(통로)다.
    public class StageMaze
    {
        static readonly Vector2Int[] Dirs =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        MazeRoom[,] rooms;

        public int Seed { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2Int StartPos { get; private set; }
        public Vector2Int BossPos { get; private set; }
        public Vector2Int CurrentPos { get; private set; }
        public List<Vector2Int> VisitedPath { get; } = new List<Vector2Int>();

        public MazeRoom Current => GetRoom(CurrentPos);

        public bool IsFinished
        {
            get
            {
                var boss = GetRoom(BossPos);
                return boss != null && boss.cleared;
            }
        }

        public static StageMaze Generate(int seed, int combatRoomCount, int eliteRoomCount, int width = 5, int height = 5)
        {
            var maze = new StageMaze { Seed = seed, Width = width, Height = height };
            var rng = new System.Random(seed);
            maze.rooms = new MazeRoom[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    maze.rooms[x, y] = new MazeRoom { x = x, y = y };

            maze.StartPos = new Vector2Int(0, 0);

            // 무작위 DFS: 스택 기반으로 통로를 뚫어 25칸을 하나의 신장 트리로 잇는다 → 모든 칸이 연결된다.
            var visited = new bool[width, height];
            var stack = new Stack<Vector2Int>();
            visited[0, 0] = true;
            stack.Push(maze.StartPos);
            while (stack.Count > 0)
            {
                var cur = stack.Peek();
                var candidates = new List<Vector2Int>();
                foreach (var d in Dirs)
                {
                    var np = cur + d;
                    if (np.x >= 0 && np.x < width && np.y >= 0 && np.y < height && !visited[np.x, np.y])
                        candidates.Add(d);
                }
                if (candidates.Count == 0)
                {
                    stack.Pop();
                    continue;
                }
                var dir = candidates[rng.Next(candidates.Count)];
                var next = cur + dir;
                SetPassage(maze, cur, dir);
                visited[next.x, next.y] = true;
                stack.Push(next);
            }

            var boss = FarthestCell(maze, maze.StartPos);
            maze.BossPos = boss;

            var path = ShortestPath(maze, maze.StartPos, boss);
            var middleCandidates = new List<Vector2Int>();
            foreach (var p in path)
            {
                if (p == maze.StartPos || p == boss) continue;
                middleCandidates.Add(p);
            }

            var chosen = new List<Vector2Int>();
            const int neededMiddle = 10;
            foreach (var p in middleCandidates)
            {
                if (chosen.Count >= neededMiddle) break;
                chosen.Add(p);
            }
            if (chosen.Count < neededMiddle)
            {
                var others = new List<Vector2Int>();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var p = new Vector2Int(x, y);
                        if (p == maze.StartPos || p == boss || chosen.Contains(p)) continue;
                        others.Add(p);
                    }
                }
                Shuffle(others, rng);
                foreach (var p in others)
                {
                    if (chosen.Count >= neededMiddle) break;
                    chosen.Add(p);
                }
            }
            Shuffle(chosen, rng);

            for (int i = 0; i < chosen.Count; i++)
            {
                MazeRoomType type = i < 2 ? MazeRoomType.Elite : (i < 3 ? MazeRoomType.Rest : MazeRoomType.Combat);
                var room = maze.GetRoom(chosen[i]);
                room.type = type;
                if (type == MazeRoomType.Elite && eliteRoomCount > 0)
                    room.roomIndex = rng.Next(eliteRoomCount);
                else if (type == MazeRoomType.Combat && combatRoomCount > 0)
                    room.roomIndex = rng.Next(combatRoomCount);
            }

            var startRoom = maze.GetRoom(maze.StartPos);
            startRoom.type = MazeRoomType.Start;
            startRoom.visited = true;
            startRoom.cleared = true;

            var bossRoom = maze.GetRoom(boss);
            bossRoom.type = MazeRoomType.Boss;

            maze.CurrentPos = maze.StartPos;
            maze.VisitedPath.Add(maze.StartPos);
            return maze;
        }

        static void SetPassage(StageMaze maze, Vector2Int cur, Vector2Int dir)
        {
            var a = maze.GetRoom(cur);
            var b = maze.GetRoom(cur + dir);
            if (dir == Vector2Int.up) { a.up = true; b.down = true; }
            else if (dir == Vector2Int.down) { a.down = true; b.up = true; }
            else if (dir == Vector2Int.left) { a.left = true; b.right = true; }
            else if (dir == Vector2Int.right) { a.right = true; b.left = true; }
        }

        static Vector2Int FarthestCell(StageMaze maze, Vector2Int from)
        {
            var dist = new Dictionary<Vector2Int, int> { [from] = 0 };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(from);
            var farthest = from;
            int maxDist = 0;
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                foreach (var d in Dirs)
                {
                    if (!maze.HasPassage(cur, d)) continue;
                    var np = cur + d;
                    if (dist.ContainsKey(np)) continue;
                    int nd = dist[cur] + 1;
                    dist[np] = nd;
                    if (nd > maxDist) { maxDist = nd; farthest = np; }
                    queue.Enqueue(np);
                }
            }
            return farthest;
        }

        static List<Vector2Int> ShortestPath(StageMaze maze, Vector2Int from, Vector2Int to)
        {
            var parent = new Dictionary<Vector2Int, Vector2Int>();
            var visited = new HashSet<Vector2Int> { from };
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(from);
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (cur == to) break;
                foreach (var d in Dirs)
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
                if (!parent.TryGetValue(node, out node)) break;
                path.Add(node);
            }
            path.Reverse();
            return path;
        }

        static void Shuffle<T>(List<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public MazeRoom GetRoom(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
            return rooms[x, y];
        }

        public MazeRoom GetRoom(Vector2Int p) => GetRoom(p.x, p.y);

        public bool HasPassage(Vector2Int pos, Vector2Int dir)
        {
            var room = GetRoom(pos);
            if (room == null) return false;
            if (dir == Vector2Int.up) return room.up;
            if (dir == Vector2Int.down) return room.down;
            if (dir == Vector2Int.left) return room.left;
            if (dir == Vector2Int.right) return room.right;
            return false;
        }

        // 지금 방을 클리어해야 이동할 수 있다(Empty·Start 는 자동 클리어). 벽·격자 밖 방향은 빠진다.
        public List<Vector2Int> GetMovableDirections()
        {
            var result = new List<Vector2Int>();
            var cur = Current;
            if (cur == null || !cur.cleared) return result;
            foreach (var d in Dirs)
            {
                if (!HasPassage(CurrentPos, d)) continue;
                if (GetRoom(CurrentPos + d) == null) continue;
                result.Add(d);
            }
            return result;
        }

        public bool TryMove(Vector2Int dir)
        {
            var cur = Current;
            if (cur == null || !cur.cleared) return false;
            if (!HasPassage(CurrentPos, dir)) return false;
            var next = CurrentPos + dir;
            var nextRoom = GetRoom(next);
            if (nextRoom == null) return false;

            CurrentPos = next;
            nextRoom.visited = true;
            if (nextRoom.type == MazeRoomType.Empty) nextRoom.cleared = true;
            VisitedPath.Add(next);
            return true;
        }

        public void CompleteCurrent()
        {
            var cur = Current;
            if (cur != null) cur.cleared = true;
        }

        public int ToCellIndex(Vector2Int p) => p.y * Width + p.x;
        public Vector2Int FromCellIndex(int idx) => new Vector2Int(idx % Width, idx / Width);

        // 저장본 복원 전용. 통행 규칙 검사 없이 방문·클리어 상태와 현재 위치를 그대로 되살린다.
        public void RestoreProgress(IList<int> path, IList<int> cleared)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    rooms[x, y].visited = false;
                    rooms[x, y].cleared = false;
                }

            VisitedPath.Clear();
            if (path == null || path.Count == 0)
            {
                CurrentPos = StartPos;
                VisitedPath.Add(StartPos);
                var startOnly = GetRoom(StartPos);
                startOnly.visited = true;
            }
            else
            {
                foreach (int idx in path)
                {
                    var p = FromCellIndex(idx);
                    VisitedPath.Add(p);
                    var r = GetRoom(p);
                    if (r != null) r.visited = true;
                }
                CurrentPos = VisitedPath[VisitedPath.Count - 1];
            }

            if (cleared != null)
            {
                foreach (int idx in cleared)
                {
                    var r = GetRoom(FromCellIndex(idx));
                    if (r != null) r.cleared = true;
                }
            }

            var start = GetRoom(StartPos);
            start.visited = true;
            start.cleared = true;
        }
    }
}
