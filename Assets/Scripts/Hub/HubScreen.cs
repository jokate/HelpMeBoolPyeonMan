using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BoolPyeon.Battle;
using BoolPyeon.Core;
using BoolPyeon.Meta;
using BoolPyeon.Save;

namespace BoolPyeon.Hub
{
    // F-21: 사망·런 클리어 뒤 도착하는 본부 허브. 요원 탭 → 메뉴, '출격' → Battle 씬에서 새 런.
    public class HubScreen : MonoBehaviour
    {
        public List<AgentDefinition> agents = new List<AgentDefinition>();
        public List<Button> agentButtons = new List<Button>();
        // T5: 전신 스폿(있으면 이쪽을 우선 쓴다). agents 와 같은 인덱스로 짝짓는다.
        public List<HubAgentSpot> agentSpots = new List<HubAgentSpot>();
        // F-29: 대화 가능한 Story 요원 머리 위 '!' 표시. agents 와 같은 인덱스.
        public List<GameObject> exclamationMarks = new List<GameObject>();
        public List<AffinityDefinition> affinities = new List<AffinityDefinition>();
        public AgentMenuPanel menuPanel;
        public Button sortieButton;
        // F-30: 후방 지원 스쿼드 편성
        public Button squadButton;
        public SquadPanel squadPanel;
        public Text crystalText;
        // T3: 출격 버튼 옆에 '스테이지 N 출격'을 보여준다.
        public Text stageText;
        public List<StageDefinition> stages = new List<StageDefinition>();
        public string battleSceneName = "Battle";

        private int currentStage = 1;

        private AffinityService affinityService;
        private AffinityService Affinity => affinityService ??= new AffinityService(SaveService.Default);

        private void Start()
        {
            if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.HQ);

            for (int i = 0; i < agentSpots.Count; i++)
            {
                int index = i;
                HubAgentSpot spot = agentSpots[i];
                if (spot == null) continue;
                spot.Bind(index < agents.Count ? agents[index] : null);
                spot.Clicked += _ => OpenAgent(index);
            }

            for (int i = 0; i < agentButtons.Count; i++)
            {
                int index = i;
                Button btn = agentButtons[i];
                if (btn == null) continue;
                if (IsSpotButton(btn)) continue; // T5: 스폿 쪽에서 이미 등록했다(중복 방지)
                btn.onClick.AddListener(() => OpenAgent(index));
            }
            if (sortieButton != null) sortieButton.onClick.AddListener(OnSortie);
            if (menuPanel != null)
            {
                menuPanel.Hide();
                menuPanel.Bought += OnMenuChanged; // F-27/29: 구매·대화 후 정화 결정·'!' 마크 갱신
                menuPanel.Hidden += () => SetSelectedSpot(-1); // T5: 닫히면 하이라이트 해제
            }
            if (squadPanel != null) squadPanel.Hide();
            if (squadButton != null) squadButton.onClick.AddListener(() => squadPanel?.Show());

            RefreshCrystals();
            RefreshMarks();
            RefreshStage();
        }

        private void OnMenuChanged()
        {
            RefreshCrystals();
            RefreshMarks();
        }

        private void RefreshCrystals()
        {
            if (crystalText != null)
                crystalText.text = $"정화 결정 {SaveService.Default.Data.crystals}";
        }

        private void RefreshMarks()
        {
            for (int i = 0; i < exclamationMarks.Count; i++)
            {
                if (exclamationMarks[i] == null) continue;
                bool canTalk = false;
                if (i < agents.Count && agents[i] != null && agents[i].role == AgentRole.Story)
                {
                    var def = FindAffinity(agents[i].name);
                    canTalk = def != null && Affinity.CanTalk(def);
                }
                exclamationMarks[i].SetActive(canTalk);
            }
        }

        // T3: HQ 표시 스테이지 = BattleController.BattleLoop 219행과 같은 공식.
        private int MaxStageNumber()
        {
            if (stages == null || stages.Count == 0) return int.MaxValue;
            int max = 1;
            foreach (var s in stages)
                if (s != null && s.stageNumber > max) max = s.stageNumber;
            return max;
        }

        private void RefreshStage()
        {
            if (stageText == null) return;
            SaveData data = SaveService.Default.Data;
            if (!data.tutorialDone)
            {
                stageText.text = string.Empty;
                currentStage = 1;
                Debug.Log("[Hub] stage label=(tutorial)");
                return;
            }

            bool resuming = data.hasRun && data.run != null;
            currentStage = resuming
                ? (data.run.stageNumber > 0 ? data.run.stageNumber : 1)
                : RunState.ComputeStartStage(data.highestClearedStage, MaxStageNumber());

            string label = resuming ? $"스테이지 {currentStage} 이어서 출격" : $"스테이지 {currentStage} 출격";
            stageText.text = label;
            Debug.Log($"[Hub] stage label={label}");
        }

        private AffinityDefinition FindAffinity(string agentId)
        {
            foreach (var a in affinities)
                if (a != null && a.agentId == agentId) return a;
            return null;
        }

        public void OpenAgent(int index)
        {
            if (menuPanel == null || agents == null || index < 0 || index >= agents.Count) return;
            AgentDefinition agent = agents[index];
            if (agent == null) return;
            menuPanel.Show(agent);
            SetSelectedSpot(index);
            Debug.Log($"[Hub] open {agent.displayName}");
        }

        // T5: 탭한 스폿만 하이라이트한다. idx<0 이면 전부 끈다(메뉴 닫힘).
        private void SetSelectedSpot(int idx)
        {
            for (int k = 0; k < agentSpots.Count; k++)
            {
                if (agentSpots[k] != null) agentSpots[k].SetHighlighted(k == idx);
            }
        }

        private bool IsSpotButton(Button btn)
        {
            for (int k = 0; k < agentSpots.Count; k++)
            {
                if (agentSpots[k] != null && agentSpots[k].Button == btn) return true;
            }
            return false;
        }

        private void OnSortie()
        {
            if (sortieButton != null) sortieButton.interactable = false;

            AffinityService.NotifyRunStarted(SaveService.Default); // F-29: 런 1회당 1회 대화 판정용 카운터

            if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Battle);

            if (SceneLoader.Instance != null)
                SceneLoader.Instance.LoadSingle(battleSceneName);
            else
                SceneManager.LoadScene(battleSceneName);

            Debug.Log($"[Hub] sortie stage={currentStage}");
        }
    }
}
