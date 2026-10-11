using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // T1: DialogueLine 에서 실제로 표시할 초상화 스프라이트를 정하는 순수 함수.
    // 순서: line.portrait 지정됨 -> 그대로. 아니면 registry 에서 speakerName+expression 조회 -> 없으면 Default -> 그래도 없으면 null.
    public static class PortraitResolver
    {
        public static Sprite Resolve(DialogueLine line, SpeakerExpressionRegistry registry)
        {
            if (line == null) return null;
            if (line.portrait != null) return line.portrait;
            if (registry == null) return null;

            var set = registry.Find(line.speakerName);
            if (set == null) return null;

            var key = string.IsNullOrEmpty(line.expression) ? SpeakerExpressionSet.DefaultKey : line.expression;
            if (set.TryGet(key, out var sprite)) return sprite;

            if (!string.Equals(key, SpeakerExpressionSet.DefaultKey, System.StringComparison.Ordinal))
            {
                if (set.TryGet(SpeakerExpressionSet.DefaultKey, out var defaultSprite)) return defaultSprite;
            }

            return null;
        }
    }
}
