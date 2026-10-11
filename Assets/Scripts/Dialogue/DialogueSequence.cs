using System;
using System.Collections.Generic;

namespace BoolPyeon.Dialogue
{
    // F-22: 순수 C# 진행 로직. UI(DialogueBox)는 이 클래스를 감싸서 화면에 그리기만 한다.
    public class DialogueSequence
    {
        private readonly IReadOnlyList<DialogueLine> lines;
        private bool finishedRaised;

        public int Index { get; private set; }
        public bool IsFinished { get; private set; }
        public DialogueLine Current =>
            (lines == null || lines.Count == 0 || Index >= lines.Count) ? null : lines[Index];

        public event Action Finished;

        public DialogueSequence(IReadOnlyList<DialogueLine> lines)
        {
            this.lines = lines;
            if (lines == null || lines.Count == 0)
            {
                IsFinished = true;
            }
        }

        // 생성 직후 한 번 불러야 한다(구독을 먼저 걸 수 있도록 생성자에서는 이벤트를 내지 않는다).
        // 빈 목록이면 여기서 바로 Finished 가 1회 난다. 줄이 있으면 아무 일도 하지 않는다(Current 가 첫 줄).
        public void Begin()
        {
            if (IsFinished) RaiseFinishedOnce();
        }

        // 다음 줄로 넘어간다. 넘어갈 줄이 있으면 true, 이미 끝이면(혹은 지금 끝나면) false 를 돌려주고
        // Finished 이벤트를 정확히 1회만 낸다.
        public bool Advance()
        {
            if (IsFinished)
            {
                RaiseFinishedOnce();
                return false;
            }

            Index++;
            if (Index >= lines.Count)
            {
                IsFinished = true;
                RaiseFinishedOnce();
                return false;
            }

            return true;
        }

        private void RaiseFinishedOnce()
        {
            if (finishedRaised) return;
            finishedRaised = true;
            Finished?.Invoke();
        }
    }
}
