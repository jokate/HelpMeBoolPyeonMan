using UnityEngine;

namespace BoolPyeon.Hub
{
    // F-21: 본부에서 탭하는 요원 하나의 데이터. 강화 구매(F-27)·모듈 장착(F-28)·대화(F-29)는
    // 다음 작업에서 role 에 따라 메뉴 내용을 채운다. 지금은 이름·초상화·역할·대사만 보여준다.
    public enum AgentRole
    {
        Upgrade,
        Module,
        Story
    }

    [CreateAssetMenu(menuName = "BoolPyeon/Hub/Agent", fileName = "Agent_")]
    public class AgentDefinition : ScriptableObject
    {
        public string displayName;
        public Sprite portrait;
        // T5: HQ 전신 스프라이트
        public Sprite fullBody;
        public AgentRole role;
        [TextArea]
        public string[] lines;
    }
}
