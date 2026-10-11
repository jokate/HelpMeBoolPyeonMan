namespace BoolPyeon.Battle
{
    // T2: 멤버마다 독립 에너지 풀을 가진다 (F-37).
    public class SquadMember
    {
        public CharacterDefinition Definition { get; }
        public Combatant Combatant { get; }
        public EnergyPool Energy { get; }

        public SquadMember(CharacterDefinition definition, Combatant combatant, int energyCap = EnergyPool.DefaultCap)
        {
            Definition = definition;
            Combatant = combatant;
            Energy = new EnergyPool(energyCap);
        }
    }
}
