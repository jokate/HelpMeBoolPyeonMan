using System;
using BoolPyeon.Dialogue;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-29: 호감도 단계 하나. requiredPoints 에 도달하면 storyDialogue 가 해금되고 보상이 한 번 지급된다.
    [Serializable]
    public class AffinityStage
    {
        public int requiredPoints;
        public DialogueDefinition storyDialogue;
        public AffinityRewardKind rewardKind;
        public int amount;
        public ModuleDefinition module;
    }
}
