using System.Collections.Generic;
using BoolPyeon.Dialogue;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-29: 요원 한 명의 호감도 데이터. agentId 는 AgentDefinition.name 과 맞춘다.
    [CreateAssetMenu(menuName = "BoolPyeon/Meta/Affinity", fileName = "Affinity_")]
    public class AffinityDefinition : ScriptableObject
    {
        public string agentId;
        public DialogueDefinition talkDialogue;
        public List<AffinityStage> stages = new List<AffinityStage>();
    }
}
