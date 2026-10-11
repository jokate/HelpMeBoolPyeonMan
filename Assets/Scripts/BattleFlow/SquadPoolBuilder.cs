using System.Collections.Generic;
using BoolPyeon.MiniGames;
using BoolPyeon.MiniGames.Phase;

namespace BoolPyeon.Battle
{
    // F-30: 스쿼드 멤버 편성으로 에너지 페이즈의 출제 풀과 에너지 규칙을 만든다.
    // 풀 선택 순서(멤버별): usage=Energy 이면서
    //   1) category == member.type(같은 이름)
    //   2) 없으면 energyStyle == member.trait(같은 이름, HighScore/Volume)
    //   3) 없으면 전체 Energy 풀
    public static class SquadPoolBuilder
    {
        private static List<MiniGameDefinition> SelectFor(CharacterDefinition member, List<MiniGameDefinition> energyPool)
        {
            var byCategory = new List<MiniGameDefinition>();
            var byStyle = new List<MiniGameDefinition>();
            foreach (var d in energyPool)
            {
                if (d.category.ToString() == member.type.ToString()) byCategory.Add(d);
                if (d.energyStyle.ToString() == member.trait.ToString()) byStyle.Add(d);
            }

            return byCategory.Count > 0 ? byCategory
                : byStyle.Count > 0 ? byStyle
                : energyPool;
        }

        public static (List<MiniGameDefinition> pool, IEnergyRule rule) Build(
            IReadOnlyList<CharacterDefinition> members, IReadOnlyList<MiniGameDefinition> allPool)
        {
            var pool = new List<MiniGameDefinition>();
            var ownerByGameId = new Dictionary<string, CharacterDefinition>();
            if (members == null || allPool == null)
                return (pool, new SquadEnergyRule(ownerByGameId));

            var energyPool = new List<MiniGameDefinition>();
            foreach (var d in allPool)
                if (d != null && d.Has(MiniGameUsage.Energy)) energyPool.Add(d);

            foreach (var member in members)
            {
                if (member == null) continue;
                List<MiniGameDefinition> chosen = SelectFor(member, energyPool);

                foreach (var d in chosen)
                {
                    if (!pool.Contains(d)) pool.Add(d);
                    if (!ownerByGameId.ContainsKey(d.id)) ownerByGameId[d.id] = member;
                }
            }

            return (pool, new SquadEnergyRule(ownerByGameId));
        }

        // T4: 멤버 한 명의 담당 풀·규칙만 돌려준다. member 나 allPool 이 없으면 빈 풀을 돌려준다.
        public static (List<MiniGameDefinition> pool, IEnergyRule rule) BuildForMember(
            CharacterDefinition member, IReadOnlyList<MiniGameDefinition> allPool)
        {
            var pool = new List<MiniGameDefinition>();
            var ownerByGameId = new Dictionary<string, CharacterDefinition>();
            if (member == null || allPool == null)
                return (pool, new SquadEnergyRule(ownerByGameId));

            var energyPool = new List<MiniGameDefinition>();
            foreach (var d in allPool)
                if (d != null && d.Has(MiniGameUsage.Energy)) energyPool.Add(d);

            List<MiniGameDefinition> chosen = SelectFor(member, energyPool);
            foreach (var d in chosen)
            {
                pool.Add(d);
                ownerByGameId[d.id] = member;
            }

            return (pool, new SquadEnergyRule(ownerByGameId));
        }
    }
}
