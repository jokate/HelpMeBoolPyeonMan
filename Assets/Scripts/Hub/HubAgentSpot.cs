using System;
using UnityEngine;
using UnityEngine.UI;

namespace BoolPyeon.Hub
{
    // T5: 본부에서 요원 한 명을 탭하는 스폿. 전신 스프라이트 + 하이라이트 + 이름표를 보여주고
    // 클릭되면 Clicked 이벤트를 알린다(어느 인덱스인지는 HubScreen 이 판단한다).
    [RequireComponent(typeof(Button))]
    public class HubAgentSpot : MonoBehaviour
    {
        public Image body;
        public GameObject highlight;
        public Text nameTag;

        public event Action<HubAgentSpot> Clicked;

        private Button button;
        public Button Button => button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => Clicked?.Invoke(this));
        }

        public void Bind(AgentDefinition a)
        {
            if (a == null)
            {
                gameObject.SetActive(false);
                return;
            }
            if (body != null)
            {
                body.sprite = a.fullBody != null ? a.fullBody : a.portrait;
                body.preserveAspect = true;
            }
            if (nameTag != null) nameTag.text = a.displayName;
        }

        public void SetHighlighted(bool on)
        {
            if (highlight != null) highlight.SetActive(on);
        }
    }
}
