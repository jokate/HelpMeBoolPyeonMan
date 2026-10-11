using System.Collections.Generic;
using BoolPyeon.MiniGames;
using BoolPyeon.MiniGames.Phase;

namespace BoolPyeon.Battle
{
    // F-37: 스쿼드 멤버의 trait 에 따라 에너지 획득량을 계산한다.
    // 고점형(HighScore): 성공마다 2, 콤보 보너스 없음.
    // 물량형(Volume): 성공마다 1, 콤보 4마다 +1.
    public class SquadEnergyRule : IEnergyRule
    {
        public const int HighScorePerSuccess = 2;
        public const int VolumePerSuccess = 1;
        public const int VolumeComboBonusEvery = 4;
        public const int VolumeComboBonus = 1;

        // 게임 id → 담당 멤버. SquadPoolBuilder 가 채운다.
        private readonly Dictionary<string, CharacterDefinition> ownerByGameId;

        public SquadEnergyRule(Dictionary<string, CharacterDefinition> ownerByGameId)
        {
            this.ownerByGameId = ownerByGameId ?? new Dictionary<string, CharacterDefinition>();
        }

        private CharacterDefinition Owner(MiniGameDefinition def)
        {
            if (def == null) return null;
            ownerByGameId.TryGetValue(def.id, out var owner);
            return owner;
        }

        public int OnSuccess(MiniGameDefinition def, int combo)
        {
            var owner = Owner(def);
            CharacterTrait trait = owner != null ? owner.trait : CharacterTrait.Volume;
            if (trait == CharacterTrait.HighScore)
            {
                return HighScorePerSuccess;
            }

            int gain = VolumePerSuccess;
            if (combo > 0 && combo % VolumeComboBonusEvery == 0)
            {
                gain += VolumeComboBonus;
            }
            return gain;
        }

        // T10: 시뮬레이터용 정적 헬퍼. OnSuccess 의 동작은 바꾸지 않고, 한 번의 수집(성공 successes 회)의 총 획득량을 계산한다.
        public static int GainForSuccesses(CharacterTrait trait, int successes)
        {
            if (successes <= 0) return 0;
            if (trait == CharacterTrait.HighScore)
                return successes * HighScorePerSuccess;
            int gain = successes * VolumePerSuccess;
            gain += (successes / VolumeComboBonusEvery) * VolumeComboBonus;
            return gain;
        }

        public string OwnerLabel(MiniGameDefinition def)
        {
            var owner = Owner(def);
            if (owner == null) return "?/?";
            return $"{owner.id}/{owner.trait}";
        }
    }
}
