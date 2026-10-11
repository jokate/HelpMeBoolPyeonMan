using System;

namespace BoolPyeon.MiniGames.Phase
{
    /// <summary>
    /// 페이즈(연속 미니게임) 진행 규칙 수치.
    /// </summary>
    [Serializable]
    public struct MiniGamePhaseConfig
    {
        public float phaseDuration;
        // T4: 멤버 한 명이 수집 페이즈를 도는 시간(F-37 기준 8초). phaseDuration(레거시 Run())은 그대로 둔다.
        public float memberPhaseDuration;
        public float speedPerSuccess;
        public int energyPerSuccess;
        public int comboBonusEvery;
        public int energyCap;
        public int difficultyStepSuccesses;

        public static MiniGamePhaseConfig Default => new MiniGamePhaseConfig
        {
            phaseDuration = 20f,
            memberPhaseDuration = 8f,
            speedPerSuccess = 1.1f,
            energyPerSuccess = 1,
            comboBonusEvery = 3,
            energyCap = 10,
            difficultyStepSuccesses = 3
        };

        // 기존 씬 직렬화 값은 새 필드가 0으로 읽힌다. 0 이하이면 8초로 대체한다.
        public float MemberPhaseDurationOrDefault => memberPhaseDuration > 0f ? memberPhaseDuration : 8f;
    }
}
