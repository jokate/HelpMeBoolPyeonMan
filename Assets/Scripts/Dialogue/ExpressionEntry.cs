using System;
using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // T1: 표정 키 하나와 그에 대응하는 초상화 스프라이트.
    [Serializable]
    public class ExpressionEntry
    {
        public string key;
        public Sprite sprite;
    }
}
