using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Battle;
using BoolPyeon.Meta;
using BoolPyeon.Save;

namespace BoolPyeon.Hub
{
    // F-30: 후방 지원 스쿼드 편성 패널. HubScreen 의 squadButton 이 연다.
    // '닫기'를 누르면 SetActive(false) 한다 — 저장은 SquadRow.OnClicked 시점(토글마다)에 이미 반영된다.
    public class SquadPanel : MonoBehaviour
    {
        public List<CharacterDefinition> candidates = new List<CharacterDefinition>();
        public List<SquadRow> rows = new List<SquadRow>();
        public Button closeButton;

        private SquadService service;
        private SquadService Service => service ??= new SquadService(SaveService.Default);

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
                closeButton.onClick.AddListener(Hide);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] == null) continue;
                if (i < candidates.Count) rows[i].def = candidates[i];
                rows[i].Bind(Service, RefreshAll);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void RefreshAll()
        {
            foreach (var row in rows)
                if (row != null) row.Refresh();
        }
    }
}
