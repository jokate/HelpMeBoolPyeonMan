namespace BoolPyeon.Battle
{
    // 턴 시작 때 계산한 몬스터 행동 예고
    public class EnemyIntent
    {
        public int MonsterIndex { get; }
        public EnemyIntentKind Kind { get; }
        // 방어·약화 적용 전 원래 피해
        public int Damage { get; }

        // AI 가 고른 행동 데이터 (Legacy 경로면 null)
        public MonsterActionDefinition Action { get; }

        // T3: 공격 대상 멤버 인덱스. 공격이 아닌 인텐트(Wait/Guard/Buff/Heal)는 -1.
        public int TargetMemberIndex { get; }

        public EnemyIntent(int monsterIndex, EnemyIntentKind kind, int damage)
            : this(monsterIndex, kind, damage, null, -1) { }

        public EnemyIntent(int monsterIndex, EnemyIntentKind kind, int damage, MonsterActionDefinition action)
            : this(monsterIndex, kind, damage, action, -1) { }

        public EnemyIntent(int monsterIndex, EnemyIntentKind kind, int damage, MonsterActionDefinition action, int targetMemberIndex)
        {
            MonsterIndex = monsterIndex;
            Kind = kind;
            Damage = damage;
            Action = action;
            TargetMemberIndex = targetMemberIndex;
        }
    }
}
