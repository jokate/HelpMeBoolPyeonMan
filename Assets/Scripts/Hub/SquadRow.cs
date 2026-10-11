using System;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Battle;
using BoolPyeon.Meta;

namespace BoolPyeon.Hub
{
    // F-30: SquadPanel 안의 후보 요원 행 하나. ModuleRow 와 같은 패턴.
    public class SquadRow : MonoBehaviour
    {
        public CharacterDefinition def;
        public Text nameText;
        public Text traitText;
        public Image selectedImage;
        public Button button;

        private SquadService service;
        private Action onChanged;

        private void Awake()
        {
            EnsureListener();
        }

        public void Bind(SquadService svc, Action onChangedCallback)
        {
            EnsureListener();
            service = svc;
            onChanged = onChangedCallback;
            Refresh();
        }

        private void EnsureListener()
        {
            if (button == null) return;
            button.onClick.RemoveListener(OnClicked);
            button.onClick.AddListener(OnClicked);
        }

        public void Refresh()
        {
            if (def == null || service == null) return;
            bool selected = service.IsSelected(def.id);
            if (nameText != null) nameText.text = def.displayName;
            if (traitText != null) traitText.text = TraitLabel(def.trait);
            if (selectedImage != null) selectedImage.color = selected ? new Color(1f, 0.85f, 0.2f) : Color.white;
        }

        // SquadEnergyRule 의 규칙을 한 줄로 보여준다.
        private static string TraitLabel(CharacterTrait trait)
        {
            switch (trait)
            {
                case CharacterTrait.HighScore: return "고점형 · 성공마다 에너지 3";
                case CharacterTrait.Volume: return "물량형 · 성공 1 + 콤보 보너스";
                default: return trait.ToString();
            }
        }

        private void OnClicked()
        {
            if (service == null || def == null) return;
            bool changed = service.Toggle(def.id);
            if (changed)
            {
                Refresh();
                onChanged?.Invoke();
            }
        }
    }
}
