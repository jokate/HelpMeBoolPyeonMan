namespace BoolPyeon.Battle
{
    // F-18(재설계): 격자 미로 칸 하나. StageMaze 가 Width*Height 개를 만들어 보관한다.
    public class MazeRoom
    {
        public int x;
        public int y;
        public MazeRoomType type = MazeRoomType.Empty;
        // Combat/Elite 일 때 combatRooms/eliteRooms 배열의 인덱스. 그 외는 쓰지 않는다(-1).
        public int roomIndex = -1;
        public bool visited;
        public bool cleared;
        // 인접 칸으로의 통로 존재 여부(양방향으로 대칭이 되도록 StageMaze.Generate 가 맞춰 둔다).
        public bool up;
        public bool down;
        public bool left;
        public bool right;
    }
}
