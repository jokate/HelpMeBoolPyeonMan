namespace BoolPyeon.Battle
{
    // F-18(재설계): 격자 미로 방 종류. Empty 는 통로(자동 클리어), 나머지는 방문 시 해당 규칙을 따른다.
    public enum MazeRoomType
    {
        Start,
        Combat,
        Elite,
        Rest,
        Boss,
        Empty
    }
}
