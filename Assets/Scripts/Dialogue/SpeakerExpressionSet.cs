using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // T1: 화자 한 명이 가진 표정 세트(키 -> 스프라이트).
    [CreateAssetMenu(menuName = "BoolPyeon/Dialogue/Speaker Expression Set", fileName = "EXPR_")]
    public class SpeakerExpressionSet : ScriptableObject
    {
        public const string DefaultKey = "Default";

        public string speakerName;
        public List<ExpressionEntry> expressions = new List<ExpressionEntry>();

        public bool TryGet(string key, out Sprite sprite)
        {
            sprite = null;
            if (string.IsNullOrEmpty(key) || expressions == null) return false;

            for (int i = 0; i < expressions.Count; i++)
            {
                var entry = expressions[i];
                if (entry == null || string.IsNullOrEmpty(entry.key)) continue;
                if (string.Equals(entry.key, key, System.StringComparison.Ordinal))
                {
                    sprite = entry.sprite;
                    return true;
                }
            }
            return false;
        }
    }
}
