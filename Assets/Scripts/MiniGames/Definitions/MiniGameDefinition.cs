using System;
using UnityEngine;

namespace BoolPyeon.MiniGames
{
    // 레거시: usage 로 대체됨. 기존 에셋 직렬화 호환을 위해 남겨 둔다
    public enum MiniGameTag
    {
        Normal,
        Special
    }

    // 출제 용도 (F-31). 한 게임이 여러 용도를 가질 수 있다
    [Flags]
    public enum MiniGameUsage
    {
        None = 0,
        Energy = 1,
        Evasion = 2,
        Special = 4
    }

    // 불편 유형. BoolPyeon.Battle.InconvenienceType 과 멤버 이름·순서를 같게 유지한다 (Enum.TryParse(name) 로 연결)
    public enum MiniGameCategory
    {
        Food,
        Wear,
        Device,
        Home,
        Outside,
        Peel
    }

    public enum MiniGameInputType
    {
        Tap,
        Mash,
        Hold,
        Drag,
        Slash,
        Rub,
        Rotate
    }

    public enum MiniGameEnergyStyle
    {
        Volume,
        HighScore
    }

    [CreateAssetMenu(fileName = "MiniGameDefinition", menuName = "BoolPyeon/MiniGame Definition")]
    public class MiniGameDefinition : ScriptableObject
    {
        public string id;
        public string sceneName;

        [TextArea]
        public string instruction;

        public float baseTimeLimit = 5f;
        public MiniGameResult timeoutResult = MiniGameResult.Failed;
        public int minDifficulty = 1;
        public int maxDifficulty = 1;

        public MiniGameUsage usage = MiniGameUsage.Energy;
        public MiniGameCategory category = MiniGameCategory.Food;
        public MiniGameInputType inputType = MiniGameInputType.Tap;
        public MiniGameEnergyStyle energyStyle = MiniGameEnergyStyle.Volume;

        [HideInInspector]
        public MiniGameTag tag = MiniGameTag.Normal;

        public bool Has(MiniGameUsage u)
        {
            return u != MiniGameUsage.None && (usage & u) == u;
        }
    }
}
