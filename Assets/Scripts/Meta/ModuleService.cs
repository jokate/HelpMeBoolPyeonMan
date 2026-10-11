using System.Collections.Generic;
using BoolPyeon.Battle;
using BoolPyeon.Save;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-28: 모듈 요원 메뉴 — PermanentUpgradeService 와 같은 패턴(SaveService 주입, 정적 적용).
    // D-13: 슬롯 1개로 시작해 최대 3개까지. 저장에 진행 중인 런이 있으면(hasRun) 장착·해제를 막는다.
    public class ModuleService
    {
        public const int MaxSlots = 3;

        readonly SaveService save;

        public ModuleService(SaveService save)
        {
            this.save = save;
        }

        public int SlotCount
        {
            get
            {
                int n = save.Data.moduleSlots;
                if (n <= 0) n = 1;
                return Mathf.Clamp(n, 1, MaxSlots);
            }
        }

        public bool IsEquipped(ModuleDefinition def)
        {
            return def != null && save.Data.equippedModules.Contains(def.name);
        }

        public bool CanEquip(ModuleDefinition def)
        {
            if (def == null || save.Data.hasRun) return false;
            if (IsEquipped(def)) return false;
            return save.Data.equippedModules.Count < SlotCount;
        }

        public bool CanUnequip(ModuleDefinition def)
        {
            return def != null && !save.Data.hasRun && IsEquipped(def);
        }

        public bool TryEquip(ModuleDefinition def)
        {
            if (!CanEquip(def)) return false;
            save.Data.equippedModules.Add(def.name);
            save.Save(save.Data);
            return true;
        }

        public bool TryUnequip(ModuleDefinition def)
        {
            if (!CanUnequip(def)) return false;
            save.Data.equippedModules.Remove(def.name);
            save.Save(save.Data);
            return true;
        }

        public List<ModuleDefinition> GetEquipped(IEnumerable<ModuleDefinition> defs)
        {
            var result = new List<ModuleDefinition>();
            if (defs == null) return result;
            foreach (var d in defs)
                if (d != null && IsEquipped(d)) result.Add(d);
            return result;
        }

        // 웨이브 State 를 만들 때 장착된 모듈만 Attach 한다. 돌려준 리스트를 웨이브가 끝나면 Detach 해야 한다.
        public static List<IModuleEffect> AttachEquipped(IEnumerable<ModuleDefinition> defs, SaveService save,
            BattleEvents events, BattleState state, CharacterDefinition character)
        {
            var result = new List<IModuleEffect>();
            if (defs == null || save == null || events == null || state == null) return result;
            foreach (var def in defs)
            {
                if (def == null || !save.Data.equippedModules.Contains(def.name)) continue;
                var effect = def.CreateEffect();
                if (effect == null) continue;
                effect.Attach(events, state, character);
                result.Add(effect);
            }
            return result;
        }
    }
}
