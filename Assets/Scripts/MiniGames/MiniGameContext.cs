using System;

namespace BoolPyeon.MiniGames
{
    /// <summary>
    /// 호스트가 미니게임에 넘기는 난이도 파라미터.
    /// </summary>
    [Serializable]
    public struct MiniGameContext
    {
        public int Difficulty;
        public float Speed;
        public float TimeLimit;

        public static MiniGameContext Default => new MiniGameContext
        {
            Difficulty = 1,
            Speed = 1f,
            TimeLimit = 5f
        };
    }
}
