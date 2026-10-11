using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // F-22: 대사 시퀀스 하나(컷신·이벤트 대화 등)를 담는 데이터. 패턴은 ModuleDefinition/AgentDefinition 과 같다.
    [CreateAssetMenu(menuName = "BoolPyeon/Dialogue/Dialogue", fileName = "DLG_")]
    public class DialogueDefinition : ScriptableObject
    {
        public string id;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }
}
