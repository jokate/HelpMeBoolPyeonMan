using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.MiniGames.Phase
{
    /// <summary>
    /// 페이즈 규칙을 계산하는 순수 C# 상태. MonoBehaviour 에 의존하지 않아
    /// EditMode 테스트로 검증할 수 있다.
    /// </summary>
    public class MiniGamePhaseState
    {
        private readonly MiniGamePhaseConfig config;
        // F-30: null 이면 기존(레거시) 에너지 계산을 그대로 쓴다.
        private IEnergyRule rule;

        public int Successes { get; private set; }
        public int Failures { get; private set; }
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public int Energy { get; private set; }
        public int TotalSuccesses { get; private set; }

        private string lastDefinitionId;

        public MiniGamePhaseState(MiniGamePhaseConfig config) : this(config, null)
        {
        }

        public MiniGamePhaseState(MiniGamePhaseConfig config, IEnergyRule rule)
        {
            this.config = config;
            this.rule = rule;
        }

        public void SetRule(IEnergyRule newRule)
        {
            rule = newRule;
        }

        public float Speed => Mathf.Pow(config.speedPerSuccess, TotalSuccesses);

        public int DifficultyFor(MiniGameDefinition def)
        {
            int difficulty = def.minDifficulty + TotalSuccesses / Mathf.Max(1, config.difficultyStepSuccesses);
            return Mathf.Clamp(difficulty, def.minDifficulty, def.maxDifficulty);
        }

        public MiniGameContext BuildContext(MiniGameDefinition def, float remainingPhaseTime)
        {
            float speed = Speed;
            float desiredTimeLimit = def.baseTimeLimit / Mathf.Max(0.0001f, speed);
            return new MiniGameContext
            {
                Difficulty = DifficultyFor(def),
                Speed = speed,
                TimeLimit = Mathf.Min(desiredTimeLimit, Mathf.Max(0f, remainingPhaseTime))
            };
        }

        public MiniGameDefinition PickNext(IReadOnlyList<MiniGameDefinition> pool, Func<int, int> rng)
        {
            if (pool == null || pool.Count == 0)
            {
                return null;
            }
            if (pool.Count == 1)
            {
                lastDefinitionId = pool[0].id;
                return pool[0];
            }

            MiniGameDefinition picked;
            int guard = 0;
            do
            {
                picked = pool[rng(pool.Count)];
                guard++;
            }
            while (picked.id == lastDefinitionId && guard < 8);

            lastDefinitionId = picked.id;
            return picked;
        }

        public void Record(MiniGameResult result)
        {
            Record(result, null);
        }

        // F-30: def 를 주고 rule 이 설정돼 있으면 rule.OnSuccess 로 에너지를 계산한다(스쿼드 규칙).
        // rule 이 없으면(레거시) 기존 config 기반 계산을 그대로 쓴다.
        public void Record(MiniGameResult result, MiniGameDefinition def)
        {
            if (result == MiniGameResult.Success)
            {
                Successes++;
                TotalSuccesses++;
                Combo++;
                MaxCombo = Mathf.Max(MaxCombo, Combo);
                if (rule != null && def != null)
                {
                    Energy += rule.OnSuccess(def, Combo);
                }
                else
                {
                    Energy += config.energyPerSuccess;
                    if (config.comboBonusEvery > 0 && Combo % config.comboBonusEvery == 0)
                    {
                        Energy += 1;
                    }
                }
                Energy = Mathf.Min(Energy, config.energyCap);
            }
            else
            {
                Failures++;
                Combo = 0;
            }
        }

        public MiniGamePhaseResult ToResult()
        {
            return new MiniGamePhaseResult
            {
                Successes = Successes,
                Failures = Failures,
                Energy = Energy,
                MaxCombo = MaxCombo
            };
        }
    }
}
