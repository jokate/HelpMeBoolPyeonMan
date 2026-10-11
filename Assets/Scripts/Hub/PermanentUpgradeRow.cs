using System;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Meta;

namespace BoolPyeon.Hub
{
    // F-27: 강화 요원 메뉴(upgradeSection) 안의 구매 행 하나.
    public class PermanentUpgradeRow : MonoBehaviour
    {
        public PermanentUpgradeDefinition def;
        public Text nameText;
        public Text levelText;
        public Text effectText;
        public Text costText;
        public Button buyButton;
        public Image iconImage;

        private PermanentUpgradeService service;
        private Action onBought;

        private void Awake()
        {
            if (buyButton != null) buyButton.onClick.AddListener(OnBuyClicked);
        }

        public void Bind(PermanentUpgradeService svc, Action onBoughtCallback)
        {
            service = svc;
            onBought = onBoughtCallback;
            Refresh();
        }

        public void Refresh()
        {
            if (def == null || service == null) return;
            int level = service.GetLevel(def);
            int max = def.MaxLevel;
            bool maxed = level >= max;
            if (nameText != null) nameText.text = def.displayName;
            if (iconImage != null)
            {
                iconImage.sprite = def.icon;
                iconImage.enabled = def.icon != null;
            }
            if (levelText != null) levelText.text = $"Lv {level}/{max}";
            if (effectText != null) effectText.text = $"+{def.TotalValue(level)}";
            if (costText != null) costText.text = maxed ? "MAX" : def.CostFor(level).ToString();
            if (buyButton != null) buyButton.interactable = service.CanBuy(def);
        }

        private void OnBuyClicked()
        {
            if (service == null || def == null) return;
            if (service.TryBuy(def))
            {
                Refresh();
                onBought?.Invoke();
            }
        }
    }
}
