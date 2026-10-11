using System.Collections.Generic;
using BoolPyeon.Battle;
using BoolPyeon.Save;

namespace BoolPyeon.Meta
{
    // F-27: 순수 C# — SaveService 를 주입받는다. EditMode 테스트가 임시 디렉터리 SaveService 로
    // 씬 없이 구매→저장→다시 열어 읽기를 확인할 수 있다(SaveServiceTests 패턴과 같음).
    public class PermanentUpgradeService
    {
        readonly SaveService save;

        public PermanentUpgradeService(SaveService save)
        {
            this.save = save;
        }

        public int GetLevel(PermanentUpgradeDefinition def)
        {
            if (def == null) return 0;
            return save.GetUpgradeLevel(def.name);
        }

        public bool CanBuy(PermanentUpgradeDefinition def)
        {
            if (def == null) return false;
            int level = GetLevel(def);
            if (level >= def.MaxLevel) return false;
            int cost = def.CostFor(level);
            return cost >= 0 && save.Data.crystals >= cost;
        }

        // 성공 = 결정 차감 + 단계 올림 + 저장. 실패(결정 부족·최대 단계)면 아무것도 바꾸지 않는다.
        public bool TryBuy(PermanentUpgradeDefinition def)
        {
            if (!CanBuy(def)) return false;
            int level = GetLevel(def);
            int cost = def.CostFor(level);
            save.Data.crystals -= cost;
            save.SetUpgradeLevel(def.name, level + 1); // SetUpgradeLevel 이 Save 까지 한 번에 한다.
            return true;
        }

        // 새 런을 만들 때 / 저장된 런을 복원할 때 둘 다 불러 보너스를 다시 계산한다(스냅샷에는 넣지 않는다).
        public static void ApplyTo(RunState state, IEnumerable<PermanentUpgradeDefinition> defs, SaveService save)
        {
            if (state == null || defs == null || save == null) return;
            foreach (var def in defs)
            {
                if (def == null) continue;
                int level = save.GetUpgradeLevel(def.name);
                int total = def.TotalValue(level);
                if (total == 0) continue;
                state.ApplyPermanent(def.kind, total);
            }
        }
    }
}
