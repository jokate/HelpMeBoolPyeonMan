using System.Collections.Generic;

namespace BoolPyeon.Battle
{
    public class Squad
    {
        public const int MaxMembers = 3;

        readonly List<SquadMember> members = new List<SquadMember>();
        public IReadOnlyList<SquadMember> Members => members;

        // T2: 공용 에너지는 폐지. 멤버마다 독립 EnergyPool(F-37). 이 상한은 새 멤버를 만들 때 넘긴다.
        readonly int memberEnergyCap;

        // 편성 전원 에너지 합계(읽기 전용). 공용 풀을 대신하지 않는다.
        public int TotalEnergy
        {
            get
            {
                int total = 0;
                foreach (var m in members) total += m.Energy.Current;
                return total;
            }
        }

        public Squad(int memberEnergyCap = EnergyPool.DefaultCap)
        {
            this.memberEnergyCap = memberEnergyCap;
        }

        public bool TryAdd(CharacterDefinition def)
        {
            if (def == null) return false;
            return TryAdd(def, Combatant.FromCharacter(def));
        }

        public bool TryAdd(CharacterDefinition def, Combatant combatant)
        {
            if (combatant == null || members.Count >= MaxMembers) return false;
            members.Add(new SquadMember(def, combatant, memberEnergyCap));
            return true;
        }

        public SquadMember Leader
        {
            get
            {
                foreach (var m in members)
                    if (!m.Combatant.IsDead) return m;
                return null;
            }
        }

        public bool IsDefeated
        {
            get
            {
                foreach (var m in members)
                    if (!m.Combatant.IsDead) return false;
                return true;
            }
        }
    }
}
