using NUnit.Framework;
using UnityEngine;
using BoolPyeon.Dialogue;

namespace BoolPyeon.Dialogue.Tests
{
    public class PortraitResolverTests
    {
        private Sprite MakeSprite()
        {
            var tex = new Texture2D(1, 1);
            var sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        private SpeakerExpressionRegistry registry;
        private SpeakerExpressionSet set;
        private Sprite portraitSprite;
        private Sprite defaultSprite;
        private Sprite joySprite;

        [SetUp]
        public void SetUp()
        {
            portraitSprite = MakeSprite();
            defaultSprite = MakeSprite();
            joySprite = MakeSprite();

            set = ScriptableObject.CreateInstance<SpeakerExpressionSet>();
            set.speakerName = "hero";
            set.expressions.Add(new ExpressionEntry { key = "Default", sprite = defaultSprite });
            set.expressions.Add(new ExpressionEntry { key = "Joy", sprite = joySprite });

            registry = ScriptableObject.CreateInstance<SpeakerExpressionRegistry>();
            registry.sets.Add(set);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(set);
            Object.DestroyImmediate(registry);
            Object.DestroyImmediate(portraitSprite);
            Object.DestroyImmediate(defaultSprite);
            Object.DestroyImmediate(joySprite);
        }

        [Test]
        public void Portrait_TakesPriorityOverRegistry()
        {
            var line = new DialogueLine { speakerName = "hero", expression = "Joy", portrait = portraitSprite };
            var result = PortraitResolver.Resolve(line, registry);
            Assert.AreSame(portraitSprite, result);
        }

        [Test]
        public void Portrait_SetAndRegistryNull_ReturnsPortrait()
        {
            var line = new DialogueLine { speakerName = "hero", portrait = portraitSprite };
            var result = PortraitResolver.Resolve(line, null);
            Assert.AreSame(portraitSprite, result);
        }

        [Test]
        public void Expression_KeyFound_ReturnsMatchingSprite()
        {
            var line = new DialogueLine { speakerName = "hero", expression = "Joy" };
            var result = PortraitResolver.Resolve(line, registry);
            Assert.AreSame(joySprite, result);
        }

        [Test]
        public void Expression_UnknownKey_FallsBackToDefault()
        {
            var line = new DialogueLine { speakerName = "hero", expression = "Angry" };
            var result = PortraitResolver.Resolve(line, registry);
            Assert.AreSame(defaultSprite, result);
        }

        [Test]
        public void Expression_Empty_UsesDefault()
        {
            var line = new DialogueLine { speakerName = "hero", expression = "" };
            var result = PortraitResolver.Resolve(line, registry);
            Assert.AreSame(defaultSprite, result);
        }

        [Test]
        public void Expression_UnknownKeyAndNoDefault_ReturnsNull()
        {
            var bareSet = ScriptableObject.CreateInstance<SpeakerExpressionSet>();
            bareSet.speakerName = "cook";
            var bareRegistry = ScriptableObject.CreateInstance<SpeakerExpressionRegistry>();
            bareRegistry.sets.Add(bareSet);

            var line = new DialogueLine { speakerName = "cook", expression = "Angry" };
            var result = PortraitResolver.Resolve(line, bareRegistry);

            Assert.IsNull(result);

            Object.DestroyImmediate(bareSet);
            Object.DestroyImmediate(bareRegistry);
        }

        [Test]
        public void SpeakerSet_NotFound_ReturnsNull()
        {
            var line = new DialogueLine { speakerName = "unknown", expression = "Joy" };
            var result = PortraitResolver.Resolve(line, registry);
            Assert.IsNull(result);
        }

        [Test]
        public void NullLineOrRegistry_ReturnsNull_NoException()
        {
            var line = new DialogueLine { speakerName = "hero", expression = "Joy" };
            Assert.IsNull(PortraitResolver.Resolve(null, registry));
            Assert.IsNull(PortraitResolver.Resolve(line, null));
        }
    }
}
