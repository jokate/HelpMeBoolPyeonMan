using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoolPyeon.UI
{
    /// <summary>
    /// 전투 화면 UI(legacy Text/Image/Button). 상태는 BattleController 가 밀어 넣고,
    /// 버튼 입력은 이벤트로 내보낸다.
    /// T9: 하단 커맨드 바(commandPanel)와 스킬 목록(skillPanel)이 서로 대체하며,
    /// 스킬 버튼이 모자라면 skillButtonPrefab 으로 동적 생성한다.
    /// </summary>
    public class BattleHud : MonoBehaviour
    {
        public Text titleCutText;
        public Image hpBar;
        public Text hpText;
        public Text energyText;
        public Text playerStatusText;
        public GameObject topBar;
        public GameObject commandPanel;
        public Button attackButton;
        public Button skillButton;
        public Button guardButton;
        public GameObject skillPanel;
        public List<Button> skillButtons = new List<Button>();
        public Button skillBackButton;
        public Button endTurnButton;
        // T9: 스킬 버튼이 모자랄 때 쓰는 공용 선택지 프리팹과 부모. skillListRoot 가 없으면 skillPanel 을 쓴다.
        public Button skillButtonPrefab;
        public RectTransform skillListRoot;
        [SerializeField] private float skillItemHeight = 120f;
        public Text logText;
        public GameObject resultPanel;
        public Text resultText;
        public Button retryButton;
        [SerializeField] private int logLines = 4;

        // T11: 화면 상단 보스 HP 바. bossHpRoot 는 기본 비활성.
        public GameObject bossHpRoot;
        public Image bossHpBar;
        public Text bossHpText;
        public Text bossNameText;
        static readonly Color BossHpColorNormal = new Color(0.8f, 0.1f, 0.1f);
        static readonly Color BossHpColorPhase2 = new Color(1f, 0.5f, 0f);

        public event Action AttackClicked;
        public event Action GuardClicked;
        public event Action<int> SkillClicked;
        public event Action EndTurnClicked;
        public event Action RetryClicked;

        private readonly List<string> logs = new List<string>();
        private bool commandVisible;
        private bool skillOpen;

        private void Awake()
        {
            if (attackButton != null) attackButton.onClick.AddListener(() => AttackClicked?.Invoke());
            if (guardButton != null) guardButton.onClick.AddListener(() => GuardClicked?.Invoke());
            if (skillButton != null) skillButton.onClick.AddListener(() => ShowSkillPanel(true));
            if (skillBackButton != null) skillBackButton.onClick.AddListener(() => ShowSkillPanel(false));
            if (endTurnButton != null) endTurnButton.onClick.AddListener(() => EndTurnClicked?.Invoke());
            if (retryButton != null) retryButton.onClick.AddListener(() => RetryClicked?.Invoke());
            for (int i = 0; i < skillButtons.Count; i++)
            {
                if (skillButtons[i] != null) HookSkillButton(skillButtons[i], i);
            }
            ShowTitle(null);
            ShowCommand(false);
            if (resultPanel != null) resultPanel.SetActive(false);
        }

        // T9: 스킬 버튼 클릭 리스너를 한 곳에서 등록(기존 버튼·동적 생성 버튼 공용)
        private void HookSkillButton(Button b, int index)
        {
            int captured = index;
            b.onClick.AddListener(() => SkillClicked?.Invoke(captured));
        }

        public void ShowTitle(string text)
        {
            if (titleCutText == null) return;
            titleCutText.gameObject.SetActive(!string.IsNullOrEmpty(text));
            titleCutText.text = text ?? string.Empty;
        }

        public void ShowTopBar(bool show)
        {
            if (topBar != null) topBar.SetActive(show);
        }

        public void SetHp(int hp, int max)
        {
            if (hpBar != null) hpBar.fillAmount = max > 0 ? Mathf.Clamp01((float)hp / max) : 0f;
            if (hpText != null) hpText.text = $"HP {hp}/{max}";
        }

        // T11: 보스 HP 바 표시/숨김. bossHpRoot 가 없으면 조용히 넘어간다.
        public void ShowBossHp(bool show, string name)
        {
            if (bossHpRoot == null) return;
            bossHpRoot.SetActive(show);
            if (show && bossNameText != null) bossNameText.text = name ?? string.Empty;
            if (!show && bossHpBar != null) bossHpBar.color = BossHpColorNormal;
        }

        public void SetBossHp(int hp, int max)
        {
            if (bossHpBar != null) bossHpBar.fillAmount = max > 0 ? Mathf.Clamp01((float)hp / max) : 0f;
            if (bossHpText != null) bossHpText.text = $"{hp}/{max}";
        }

        // T11: 2페이즈 전환 시 바 색을 바꿔 격노를 알린다
        public void SetBossPhase2(bool phase2)
        {
            if (bossHpBar == null) return;
            bossHpBar.color = phase2 ? BossHpColorPhase2 : BossHpColorNormal;
        }

        public void SetEnergy(int current, int cap)
        {
            if (energyText != null) energyText.text = $"에너지 {current}/{cap}";
        }

        // T2.1: 멤버가 2명 이상일 때 '이름 cur/cap' 을 이어 붙여 보여준다(전용 게이지는 T6).
        public void SetEnergyText(string text)
        {
            if (energyText != null) energyText.text = text ?? string.Empty;
        }

        // T8: 주인공 상태이상 임시 글자 배지. 비어 있으면 숨긴다.
        public void SetPlayerStatuses(string text)
        {
            if (playerStatusText == null) return;
            bool has = !string.IsNullOrEmpty(text);
            playerStatusText.gameObject.SetActive(has);
            playerStatusText.text = has ? text : string.Empty;
        }

        public void ShowCommand(bool show)
        {
            commandVisible = show;
            ShowSkillPanel(false);
        }

        // 스킬 목록이 열려 있는 동안 기본 커맨드(하단 바)는 완전히 대체된다
        public void ShowSkillPanel(bool show)
        {
            skillOpen = show && commandVisible;
            if (skillPanel != null) skillPanel.SetActive(skillOpen);
            if (commandPanel != null) commandPanel.SetActive(commandVisible && !skillOpen);
            if (endTurnButton != null) endTurnButton.gameObject.SetActive(commandVisible && !skillOpen);
        }

        public void SetCommandInteractable(bool attack, bool guard, bool anySkill)
        {
            if (attackButton != null) attackButton.interactable = attack;
            if (guardButton != null) guardButton.interactable = guard;
            if (skillButton != null) skillButton.interactable = anySkill;
        }

        // labels.Count 개만 켜고 나머지 버튼은 숨긴다. 모자라면 skillButtonPrefab 으로 새로 만든다.
        public void SetSkills(IList<string> labels, IList<bool> usable)
        {
            SetSkills(labels, usable, null);
        }

        // icons[i] 는 버튼 i 의 아이콘 칸(버튼 자식 중 targetGraphic 이 아닌 첫 Image)에 들어간다. null 이면 숨긴다.
        public void SetSkills(IList<string> labels, IList<bool> usable, IList<Sprite> icons)
        {
            int count = labels != null ? labels.Count : 0;
            if (count > skillButtons.Count && skillButtonPrefab != null)
            {
                RectTransform parent = skillListRoot != null
                    ? skillListRoot
                    : (skillPanel != null ? skillPanel.transform as RectTransform : null);
                if (parent != null)
                {
                    while (skillButtons.Count < count)
                    {
                        Button created = Instantiate(skillButtonPrefab, parent, false);
                        HookSkillButton(created, skillButtons.Count);
                        skillButtons.Add(created);
                        if (skillBackButton != null && skillBackButton.transform.parent == parent)
                        {
                            skillBackButton.transform.SetAsLastSibling();
                        }
                    }
                }
            }

            for (int i = 0; i < skillButtons.Count; i++)
            {
                Button b = skillButtons[i];
                if (b == null) continue;
                bool has = i < count;
                b.gameObject.SetActive(has);
                if (!has) continue;
                Text t = b.GetComponentInChildren<Text>();
                if (t != null) t.text = labels[i];
                Image icon = FindIcon(b);
                if (icon != null)
                {
                    Sprite sprite = icons != null && i < icons.Count ? icons[i] : null;
                    icon.sprite = sprite;
                    icon.enabled = sprite != null;
                }
                b.interactable = usable != null && i < usable.Count && usable[i];
            }

            if (skillPanel != null)
            {
                RectTransform panelRect = skillPanel.transform as RectTransform;
                if (panelRect != null)
                {
                    int itemCount = count + (skillBackButton != null ? 1 : 0);
                    Vector2 size = panelRect.sizeDelta;
                    size.y = skillItemHeight * itemCount;
                    panelRect.sizeDelta = size;
                }
            }
        }

        private static Image FindIcon(Button b)
        {
            foreach (Image img in b.GetComponentsInChildren<Image>(true))
            {
                if (img != b.targetGraphic) return img;
            }
            return null;
        }

        public void Log(string line)
        {
            logs.Add(line);
            while (logs.Count > logLines) logs.RemoveAt(0);
            if (logText != null) logText.text = string.Join("\n", logs);
        }

        public void ClearLog()
        {
            logs.Clear();
            if (logText != null) logText.text = string.Empty;
        }

        public void ShowResult(bool victory)
        {
            ShowCommand(false);
            if (resultText != null) resultText.text = victory ? "승리!" : "패배...";
            if (resultPanel != null) resultPanel.SetActive(true);
        }

        // F-21: 사망·런 클리어 결과에 처치 수·이번 런에서 얻은 정화 결정을 함께 보여준다.
        public void ShowResult(bool victory, int kills, int crystalsEarned)
        {
            ShowCommand(false);
            if (resultText != null)
                resultText.text = $"{(victory ? "승리!" : "패배...")}\n처치 {kills} · 정화 결정 +{crystalsEarned}";
            if (resultPanel != null) resultPanel.SetActive(true);
        }

        // T3: 클리어·실패한 스테이지 번호와 다음 출격 스테이지를 함께 보여준다.
        public void ShowResult(bool victory, int kills, int crystalsEarned, int stage, int nextStage, bool lastStage)
        {
            ShowCommand(false);
            if (resultText != null)
            {
                string line1 = victory ? $"스테이지 {stage} 클리어!" : $"스테이지 {stage} 실패...";
                string line2 = $"처치 {kills} · 정화 결정 +{crystalsEarned}";
                string line3 = $"다음 출격: 스테이지 {nextStage}" + (lastStage ? " (마지막 스테이지)" : "");
                resultText.text = $"{line1}\n{line2}\n{line3}";
            }
            if (resultPanel != null) resultPanel.SetActive(true);
        }
    }
}
