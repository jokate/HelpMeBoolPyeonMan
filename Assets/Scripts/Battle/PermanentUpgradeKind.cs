namespace BoolPyeon.Battle
{
    // F-27: 영구 능력치 강화 5종. RunState 가 직접 적용해야 해서(순환 참조를 피하려고) Battle 어셈블리에 둔다.
    // BoolPyeon.Meta.PermanentUpgradeDefinition 이 이 enum 을 데이터로 참조한다.
    public enum PermanentUpgradeKind
    {
        MaxHp,
        Attack,
        Defense,
        StartEnergy,
        EnergyAbsorb
    }
}
