using System.Collections.Generic;
using NUnit.Framework;
using BoolPyeon.Dialogue;

namespace BoolPyeon.Dialogue.Tests
{
    public class DialogueSequenceTests
    {
        private static List<DialogueLine> FiveLines()
        {
            var lines = new List<DialogueLine>();
            for (int i = 0; i < 5; i++)
                lines.Add(new DialogueLine { speakerName = "S" + i, text = "line" + i });
            return lines;
        }

        [Test]
        public void FiveLines_AdvanceFourTimesTrue_FifthFalse()
        {
            var seq = new DialogueSequence(FiveLines());
            seq.Begin();

            Assert.IsTrue(seq.Advance());
            Assert.IsTrue(seq.Advance());
            Assert.IsTrue(seq.Advance());
            Assert.IsTrue(seq.Advance());
            Assert.IsFalse(seq.Advance());
            Assert.IsTrue(seq.IsFinished);
        }

        [Test]
        public void FiveLines_FinishedRaisedExactlyOnce()
        {
            var seq = new DialogueSequence(FiveLines());
            int count = 0;
            seq.Finished += () => count++;
            seq.Begin();

            for (int i = 0; i < 4; i++) seq.Advance();
            seq.Advance(); // finishes here
            seq.Advance(); // already finished, must not raise again
            seq.Advance();

            Assert.AreEqual(1, count);
        }

        [Test]
        public void EmptyList_FinishesImmediately()
        {
            var seq = new DialogueSequence(new List<DialogueLine>());
            int count = 0;
            seq.Finished += () => count++;

            Assert.IsTrue(seq.IsFinished); // 생성자에서 이미 끝 상태(이벤트는 아직 안 났음)
            Assert.AreEqual(0, count);

            seq.Begin();

            Assert.IsTrue(seq.IsFinished);
            Assert.AreEqual(1, count);
            Assert.IsNull(seq.Current);
        }

        [Test]
        public void NullList_FinishesImmediately()
        {
            var seq = new DialogueSequence(null);
            int count = 0;
            seq.Finished += () => count++;
            seq.Begin();

            Assert.IsTrue(seq.IsFinished);
            Assert.AreEqual(1, count);
        }
    }
}
