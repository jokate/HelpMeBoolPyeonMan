using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Battle;

namespace BoolPyeon.UI
{
    // F-19: 방 클리어 강화 선택 3택 UI(legacy Text/Image/Button). BattleHud 의 skillButtons 패턴을 따른다.
    public class UpgradeChoiceHud : MonoBehaviour
    {
        public GameObject panel;
        public List<Button> buttons = new List<Button>();
        public List<Text> names = new List<Text>();
        public List<Text> descs = new List<Text>();
        public List<Image> icons = new List<Image>();

        public event Action<int> Chosen;

        private bool showRequested;

        private void Awake()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                if (buttons[i] != null)
                    buttons[i].onClick.AddListener(() => Chosen?.Invoke(index));
            }
            if (!showRequested) Hide();
        }

        public void Show(IList<UpgradeDefinition> choices)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                bool has = choices != null && i < choices.Count && choices[i] != null;
                if (buttons[i] != null) buttons[i].gameObject.SetActive(has);
                if (has)
                {
                    if (i < names.Count && names[i] != null) names[i].text = choices[i].displayName;
                    if (i < descs.Count && descs[i] != null) descs[i].text = choices[i].description;
                    if (i < icons.Count && icons[i] != null)
                    {
                        icons[i].sprite = choices[i].icon;
                        icons[i].enabled = choices[i].icon != null;
                    }
                }
            }
            showRequested = true;
            if (panel != null) panel.SetActive(true);
        }

        public void Hide()
        {
            showRequested = false;
            if (panel != null) panel.SetActive(false);
        }
    }
}
