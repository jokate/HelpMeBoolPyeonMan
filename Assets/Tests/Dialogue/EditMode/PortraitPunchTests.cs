using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Dialogue;

namespace BoolPyeon.Dialogue.Tests
{
    public class PortraitPunchTests
    {
        private static Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
        }

        [Test]
        public void ShouldPunch_SameSprite_ReturnsFalse()
        {
            var s = MakeSprite();
            Assert.IsFalse(PortraitPunch.ShouldPunch(s, s));
        }

        [Test]
        public void ShouldPunch_DifferentSprites_ReturnsTrue()
        {
            var a = MakeSprite();
            var b = MakeSprite();
            Assert.IsTrue(PortraitPunch.ShouldPunch(a, b));
        }

        [Test]
        public void ShouldPunch_PrevNull_ReturnsFalse()
        {
            var b = MakeSprite();
            Assert.IsFalse(PortraitPunch.ShouldPunch(null, b));
        }

        [Test]
        public void ShouldPunch_NextNull_ReturnsFalse()
        {
            var a = MakeSprite();
            Assert.IsFalse(PortraitPunch.ShouldPunch(a, null));
        }

        [Test]
        public void EvaluateScale_AtZero_ReturnsOne()
        {
            Assert.AreEqual(1f, PortraitPunch.EvaluateScale(0f, 0.12f, 1.1f));
        }

        [Test]
        public void EvaluateScale_AtHalfDuration_ReturnsPeak()
        {
            Assert.AreEqual(1.1f, PortraitPunch.EvaluateScale(0.06f, 0.12f, 1.1f), 0.0001f);
        }

        [Test]
        public void EvaluateScale_AtDuration_ReturnsOne()
        {
            Assert.AreEqual(1f, PortraitPunch.EvaluateScale(0.12f, 0.12f, 1.1f));
        }

        [Test]
        public void EvaluateScale_QuarterDuration_BetweenOneAndPeak()
        {
            var value = PortraitPunch.EvaluateScale(0.03f, 0.12f, 1.1f);
            Assert.Greater(value, 1f);
            Assert.Less(value, 1.1f);
        }

        [Test]
        public void EvaluateScale_ZeroDuration_ReturnsOne()
        {
            Assert.AreEqual(1f, PortraitPunch.EvaluateScale(0.05f, 0f, 1.1f));
        }
    }
}
