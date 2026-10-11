using System;
using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // F-22: 대화 한 줄. 화자 이름·초상화·본문.
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite portrait;
        // T1: speakerName + expression 으로 표정 세트를 조회한다. 비면 Default 를 쓴다. portrait 가 지정돼 있으면 이 필드는 무시된다(기존 호환).
        public string expression;
        [TextArea]
        public string text;
    }
}
