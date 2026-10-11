using System;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Meta;

namespace BoolPyeon.Hub
{
    // F-28: 모듈 요원 메뉴(moduleSection) 안의 모듈 행 하나. PermanentUpgradeRow 와 같은 패턴.
    public class ModuleRow : MonoBehaviour
    {
        public ModuleDefinition def;
        public Text nameText;
        public Text descText;
        public Text statusText;
        public Button equipButton;
        public Text equipButtonText;
        public Image iconImage;

        private ModuleService service;
        private Meta.AffinityService affinity;
        private Action onChanged;

        private void Awake()
        {
            EnsureListener();
        }

        public void Bind(ModuleService svc, Action onChangedCallback)
        {
            Bind(svc, null, onChangedCallback);
        }

        // F-29: affinitySvc 를 주면 IsModuleUnlocked 로 잠금 여부를 표시한다(없으면 항상 해금된 것으로 본다).
        public void Bind(ModuleService svc, Meta.AffinityService affinitySvc, Action onChangedCallback)
        {
            // Bind() 는 AgentMenuPanel.Show() 가 moduleSection 을 활성화한 직후 같은 프레임에서 부른다.
            // moduleSection 의 조상(AgentMenuPanel)이 그 시점까지 비활성이면 이 행의 Awake() 가 아직
            // 돌지 않아(계층 활성화가 나중에 한꺼번에 일어남) onClick 리스너가 안 붙은 채로 Refresh() 만
            // 실행되는 경우가 있었다("장착" 클릭이 아무 반응이 없던 원인). 여기서도 한 번 더 보장한다.
            EnsureListener();
            service = svc;
            affinity = affinitySvc;
            onChanged = onChangedCallback;
            Refresh();
        }

        private void EnsureListener()
        {
            if (equipButton == null) return;
            equipButton.onClick.RemoveListener(OnClicked);
            equipButton.onClick.AddListener(OnClicked);
        }

        public void Refresh()
        {
            if (def == null || service == null) return;
            bool unlocked = affinity == null || affinity.IsModuleUnlocked(def);
            bool equipped = service.IsEquipped(def);
            if (nameText != null) nameText.text = def.displayName;
            if (descText != null) descText.text = def.description;
            if (iconImage != null)
            {
                iconImage.sprite = def.icon;
                iconImage.enabled = def.icon != null;
            }
            if (statusText != null) statusText.text = !unlocked ? "잠김" : (equipped ? "장착 중" : "미장착");
            if (equipButtonText != null) equipButtonText.text = equipped ? "해제" : "장착";
            if (equipButton != null)
                equipButton.interactable = !unlocked ? false : (equipped ? service.CanUnequip(def) : service.CanEquip(def));
        }

        private void OnClicked()
        {
            if (service == null || def == null) return;
            if (affinity != null && !affinity.IsModuleUnlocked(def)) return;
            bool changed = service.IsEquipped(def) ? service.TryUnequip(def) : service.TryEquip(def);
            if (changed)
            {
                Refresh();
                onChanged?.Invoke();
            }
        }
    }
}
