using System.Collections.Generic;
using BoolPyeon.Dialogue;
using BoolPyeon.Save;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-29: 호감도·요원 스토리 — ModuleService/PermanentUpgradeService 와 같은 패턴(SaveService 주입, 즉시 Save).
    // 대화는 런 1회당 1회만 점수를 준다(lastTalkRun < runsStarted). 단계 보상은 claimedStages 로 중복 지급을 막는다.
    public class AffinityService
    {
        readonly SaveService save;

        public AffinityService(SaveService save)
        {
            this.save = save;
        }

        private AffinityEntry GetOrCreateEntry(AffinityDefinition def)
        {
            if (def == null) return null;
            foreach (var e in save.Data.affinities)
                if (e.agentId == def.agentId) return e;
            var entry = new AffinityEntry { agentId = def.agentId, points = 0, lastTalkRun = -1 };
            save.Data.affinities.Add(entry);
            return entry;
        }

        public int GetPoints(AffinityDefinition def)
        {
            if (def == null) return 0;
            foreach (var e in save.Data.affinities)
                if (e.agentId == def.agentId) return e.points;
            return 0;
        }

        // 현재 도달한 단계 인덱스(도달한 게 없으면 -1).
        public int GetStage(AffinityDefinition def)
        {
            if (def == null || def.stages == null) return -1;
            int points = GetPoints(def);
            int stage = -1;
            for (int i = 0; i < def.stages.Count; i++)
            {
                if (points >= def.stages[i].requiredPoints) stage = i;
            }
            return stage;
        }

        public bool CanTalk(AffinityDefinition def)
        {
            if (def == null) return false;
            foreach (var e in save.Data.affinities)
                if (e.agentId == def.agentId) return e.lastTalkRun < save.Data.runsStarted;
            return true; // 아직 항목이 없으면(첫 방문) lastTalkRun=-1 인 것과 같다.
        }

        // 성공하면 points+1, 새로 도달한 단계마다 보상을 지급한다. reached 는 이번에 새로 도달한 마지막 단계.
        public bool TryTalk(AffinityDefinition def, out AffinityStage reached)
        {
            reached = null;
            if (!CanTalk(def)) return false;

            var entry = GetOrCreateEntry(def);
            int before = entry.points;
            entry.points = before + 1;
            entry.lastTalkRun = save.Data.runsStarted;

            if (def.stages != null)
            {
                for (int i = 0; i < def.stages.Count; i++)
                {
                    var stage = def.stages[i];
                    if (entry.points >= stage.requiredPoints && !entry.claimedStages.Contains(i))
                    {
                        entry.claimedStages.Add(i);
                        ApplyReward(stage);
                        reached = stage;
                    }
                }
            }

            save.Save(save.Data);
            return true;
        }

        private void ApplyReward(AffinityStage stage)
        {
            switch (stage.rewardKind)
            {
                case AffinityRewardKind.Crystals:
                    save.Data.crystals += stage.amount;
                    break;
                case AffinityRewardKind.ModuleSlot:
                    save.Data.moduleSlots = Mathf.Clamp(save.Data.moduleSlots + stage.amount, 1, ModuleService.MaxSlots);
                    break;
                case AffinityRewardKind.ModuleUnlock:
                    if (stage.module != null && !save.Data.unlockedModules.Contains(stage.module.name))
                        save.Data.unlockedModules.Add(stage.module.name);
                    break;
            }
        }

        public bool IsModuleUnlocked(ModuleDefinition module)
        {
            if (module == null) return false;
            return module.unlockedByDefault || save.Data.unlockedModules.Contains(module.name);
        }

        public List<DialogueDefinition> GetUnlockedStories(AffinityDefinition def)
        {
            var result = new List<DialogueDefinition>();
            if (def == null || def.stages == null) return result;
            var entry = GetOrCreateEntry(def);
            foreach (int i in entry.claimedStages)
            {
                if (i >= 0 && i < def.stages.Count && def.stages[i].storyDialogue != null)
                    result.Add(def.stages[i].storyDialogue);
            }
            return result;
        }

        // 매 출격마다 부른다(HubScreen.OnSortie). 이번 런에서 이미 대화했다면 다음 방문부터 다시 가능해진다.
        public static void NotifyRunStarted(SaveService save)
        {
            save.Data.runsStarted++;
            save.Save(save.Data);
        }
    }
}
