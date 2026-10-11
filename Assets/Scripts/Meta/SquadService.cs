using System.Collections.Generic;
using BoolPyeon.Battle;
using BoolPyeon.Save;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-30: 후방 지원 스쿼드 편성 — ModuleService 와 같은 패턴(SaveService 주입, id 리스트 저장).
    // 편성은 SaveData.squadIds 에 저장한다. 비어 있으면 ['hero'] 로 취급한다(하위 호환).
    public class SquadService
    {
        public const int MaxMembers = 3;
        public const string LeaderId = "hero";

        readonly SaveService save;

        public SquadService(SaveService save)
        {
            this.save = save;
        }

        private List<string> Ids
        {
            get
            {
                if (save.Data.squadIds == null) save.Data.squadIds = new List<string>();
                if (save.Data.squadIds.Count == 0) save.Data.squadIds.Add(LeaderId);
                return save.Data.squadIds;
            }
        }

        public bool IsSelected(string id)
        {
            return !string.IsNullOrEmpty(id) && Ids.Contains(id);
        }

        // 현재 편성을 defs 순서를 무시하고 저장된 id 순서(Members[0] = 리더)로 돌려준다.
        // id 를 못 찾은 항목은 건너뛰고, 결과가 비면 리더(defs 안의 "hero")를 넣는다.
        public List<CharacterDefinition> GetSquad(IEnumerable<CharacterDefinition> defs)
        {
            var result = new List<CharacterDefinition>();
            foreach (var id in Ids)
            {
                var def = Find(defs, id);
                if (def != null) result.Add(def);
            }
            if (result.Count == 0)
            {
                var hero = Find(defs, LeaderId);
                if (hero != null) result.Add(hero);
            }
            return result;
        }

        public bool CanToggleOff(string id)
        {
            if (string.IsNullOrEmpty(id) || id == LeaderId) return false;
            return Ids.Contains(id) && Ids.Count > 1;
        }

        // 4번째 멤버를 넣으려 하면 거부하고 로그를 남긴다.
        public bool Toggle(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            var ids = Ids;
            if (ids.Contains(id))
            {
                if (id == LeaderId)
                {
                    Debug.Log("[Squad] reject unequip hero: leader cannot be removed");
                    return false;
                }
                if (ids.Count <= 1)
                {
                    Debug.Log("[Squad] reject unequip: minimum 1 member");
                    return false;
                }
                ids.Remove(id);
                save.Save(save.Data);
                return true;
            }

            if (ids.Count >= MaxMembers)
            {
                Debug.Log($"[Squad] reject {id}: squad full ({ids.Count}/{MaxMembers})");
                return false;
            }
            ids.Add(id);
            save.Save(save.Data);
            return true;
        }

        static CharacterDefinition Find(IEnumerable<CharacterDefinition> defs, string id)
        {
            if (defs == null) return null;
            foreach (var d in defs)
                if (d != null && d.id == id) return d;
            return null;
        }
    }
}
