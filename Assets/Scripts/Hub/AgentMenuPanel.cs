using System;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Dialogue;
using BoolPyeon.Meta;
using BoolPyeon.Save;
using BoolPyeon.UI;

namespace BoolPyeon.Hub
{
    // F-21: 요원 탭 시 여는 메뉴. F-27: role==Upgrade 면 upgradeSection(강화 구매 5행)을 보여준다.
    // F-28: role==Module 이면 moduleSection(모듈 장착 행)을 보여준다. F-29: role==Story 면 storySection.
    public class AgentMenuPanel : MonoBehaviour
    {
        public Image portrait;
        public Text nameText;
        public Text roleText;
        public Text lineText;
        public Button closeButton;
        // 준비 중 안내. role==Upgrade/Module/Story 일 때는 각 section 이 대신 켜진다.
        public GameObject noticeText;
        // F-27
        public GameObject upgradeSection;
        public PermanentUpgradeRow[] upgradeRows = Array.Empty<PermanentUpgradeRow>();
        // F-28
        public GameObject moduleSection;
        public ModuleRow[] moduleRows = Array.Empty<ModuleRow>();
        // F-29
        public GameObject storySection;
        public Text affinityText;
        public Button talkButton;
        public Text rewardText;
        public DialogueBox dialogueBox;
        // agentId(=AgentDefinition.name) 로 찾는다.
        public AffinityDefinition[] affinityDefs = Array.Empty<AffinityDefinition>();

        // 구매·대화가 일어날 때마다 알림(HubScreen 이 정화 결정 표시·'!' 마크를 갱신한다).
        public event Action Bought;
        // T5: 메뉴가 닫힐 때 알림(HubScreen 이 스폿 하이라이트를 끈다).
        public event Action Hidden;

        private PermanentUpgradeService upgradeService;
        private PermanentUpgradeService UpgradeService => upgradeService ??= new PermanentUpgradeService(SaveService.Default);
        private ModuleService moduleService;
        private ModuleService Modules => moduleService ??= new ModuleService(SaveService.Default);
        private AffinityService affinityService;
        private AffinityService Affinity => affinityService ??= new AffinityService(SaveService.Default);
        private AffinityDefinition currentAffinity;

        private void Awake()
        {
            if (closeButton != null) closeButton.onClick.AddListener(Hide);
            if (talkButton != null) talkButton.onClick.AddListener(OnTalkClicked);
            // 주의: 씬은 이 오브젝트를 처음부터 비활성 상태로 둔다(HubScreen.Awake 가 Hide() 로 한 번 더 보장한다).
            // 여기서 gameObject.SetActive(false) 를 부르면, Show() 가 SetActive(true) 로 처음 켤 때
            // 그 안에서 지연 실행되는 이 Awake 가 같은 프레임에 다시 꺼버려 "첫 탭에서 메뉴가 안 열리는" 버그가 난다.
        }

        public void Show(AgentDefinition agent)
        {
            if (agent == null) return;
            // 내용(초상화 등)을 먼저 채우고 마지막에 SetActive(true) 한다 —
            // 이전 요원의 내용이 한 프레임이라도 보일 여지를 없앤다.
            if (portrait != null) portrait.sprite = agent.portrait;
            if (nameText != null) nameText.text = agent.displayName;
            if (roleText != null) roleText.text = RoleLabel(agent.role);
            if (lineText != null)
                lineText.text = agent.lines != null && agent.lines.Length > 0 ? agent.lines[0] : string.Empty;

            bool isUpgrade = agent.role == AgentRole.Upgrade;
            bool isModule = agent.role == AgentRole.Module;
            bool isStory = agent.role == AgentRole.Story;
            if (upgradeSection != null) upgradeSection.SetActive(isUpgrade);
            if (moduleSection != null) moduleSection.SetActive(isModule);
            if (storySection != null) storySection.SetActive(isStory);
            if (noticeText != null) noticeText.SetActive(!isUpgrade && !isModule && !isStory);
            if (lineText != null) lineText.gameObject.SetActive(!isUpgrade && !isModule);
            if (isUpgrade && upgradeRows != null)
            {
                foreach (var row in upgradeRows)
                {
                    if (row != null) row.Bind(UpgradeService, OnBought);
                }
            }
            if (isModule && moduleRows != null)
            {
                foreach (var row in moduleRows)
                {
                    if (row != null) row.Bind(Modules, Affinity, OnModuleChanged);
                }
            }
            if (isStory)
            {
                currentAffinity = FindAffinity(agent.name);
                if (rewardText != null) rewardText.text = string.Empty;
                RefreshStory();
            }
            gameObject.SetActive(true);
        }

        private AffinityDefinition FindAffinity(string agentId)
        {
            if (affinityDefs == null) return null;
            foreach (var a in affinityDefs)
                if (a != null && a.agentId == agentId) return a;
            return null;
        }

        private void RefreshStory()
        {
            if (currentAffinity == null) return;
            int points = Affinity.GetPoints(currentAffinity);
            int stageIndex = Affinity.GetStage(currentAffinity);
            int level = stageIndex + 1;
            int nextRequired = -1;
            if (currentAffinity.stages != null)
            {
                foreach (var s in currentAffinity.stages)
                    if (points < s.requiredPoints) { nextRequired = s.requiredPoints; break; }
            }
            if (affinityText != null)
                affinityText.text = nextRequired >= 0
                    ? $"호감도 Lv {level} ({points}/{nextRequired})"
                    : $"호감도 Lv {level} (MAX)";
            bool canTalk = Affinity.CanTalk(currentAffinity);
            if (talkButton != null) talkButton.interactable = canTalk;
            if (lineText != null && !canTalk)
                lineText.text = "다음 출격 후 대화 가능";
        }

        private void OnTalkClicked()
        {
            if (currentAffinity == null || dialogueBox == null) return;
            if (!Affinity.CanTalk(currentAffinity)) return;

            var unlocked = Affinity.GetUnlockedStories(currentAffinity);
            var toPlay = unlocked.Count > 0 ? unlocked[unlocked.Count - 1] : currentAffinity.talkDialogue;
            if (toPlay == null) return;

            dialogueBox.Finished -= OnTalkFinished;
            dialogueBox.Finished += OnTalkFinished;
            dialogueBox.Play(toPlay);
        }

        private void OnTalkFinished()
        {
            dialogueBox.Finished -= OnTalkFinished;
            if (currentAffinity == null) return;
            if (Affinity.TryTalk(currentAffinity, out var reached))
            {
                if (rewardText != null)
                    rewardText.text = reached != null ? RewardLabel(reached) : "호감도 UP!";
                RefreshStory();
                Bought?.Invoke();
            }
        }

        private static string RewardLabel(AffinityStage stage)
        {
            switch (stage.rewardKind)
            {
                case AffinityRewardKind.Crystals: return $"호감도 UP! 정화 결정 +{stage.amount}";
                case AffinityRewardKind.ModuleSlot: return $"호감도 UP! 모듈 슬롯 +{stage.amount}";
                case AffinityRewardKind.ModuleUnlock: return $"호감도 UP! 모듈 해금: {(stage.module != null ? stage.module.displayName : string.Empty)}";
                default: return "호감도 UP!";
            }
        }

        private void OnBought()
        {
            if (upgradeRows != null)
            {
                foreach (var row in upgradeRows)
                    if (row != null) row.Refresh();
            }
            Bought?.Invoke();
        }

        private void OnModuleChanged()
        {
            if (moduleRows != null)
            {
                foreach (var row in moduleRows)
                    if (row != null) row.Refresh();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Hidden?.Invoke();
        }

        private static string RoleLabel(AgentRole role)
        {
            switch (role)
            {
                case AgentRole.Upgrade: return "강화 요원";
                case AgentRole.Module: return "모듈 요원";
                default: return "요원";
            }
        }
    }
}
