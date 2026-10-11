using System.Collections;
using UnityEngine;

namespace BoolPyeon.BattleFlow
{
    /// <summary>
    /// 피해량을 잠깐 띄워 보여주는 월드 공간 숫자. 0.6초 동안 위로 떠오르며 사라진다.
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        private const float Duration = 0.6f;
        private const float RiseDistance = 0.8f;

        public static void Spawn(Vector3 pos, int amount, Color c)
        {
            Spawn(pos, amount.ToString(), c);
        }

        public static void Spawn(Vector3 pos, string text, Color c)
        {
            Spawn(pos, text, c, null, 1f);
        }

        // splash 가 있으면 숫자 뒤에 깔고(코믹 데미지 스플래시) 숫자는 흰색으로 쓴다. scale 은 강타 강조용.
        public static void Spawn(Vector3 pos, string text, Color c, Sprite splash, float scale)
        {
            var go = new GameObject("DamagePopup");
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * scale;
            TextMesh tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.2f;
            tm.fontSize = 48;
            tm.fontStyle = FontStyle.Bold;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = splash != null ? Color.white : c;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = 13;

            // 글자 뒤 그림자(TextMesh 는 외곽선이 없어서 어두운 사본을 살짝 비켜 깐다)
            var shadowGo = new GameObject("Shadow");
            shadowGo.transform.SetParent(go.transform, false);
            shadowGo.transform.localPosition = new Vector3(0.04f, -0.04f, 0.01f);
            TextMesh shadow = shadowGo.AddComponent<TextMesh>();
            shadow.text = text;
            shadow.characterSize = tm.characterSize;
            shadow.fontSize = tm.fontSize;
            shadow.fontStyle = tm.fontStyle;
            shadow.anchor = tm.anchor;
            shadow.alignment = tm.alignment;
            shadow.color = new Color(0.1f, 0.08f, 0.08f, 1f);
            shadowGo.GetComponent<MeshRenderer>().sortingOrder = 12;

            SpriteRenderer bg = null;
            if (splash != null)
            {
                var bgGo = new GameObject("Splash");
                bgGo.transform.SetParent(go.transform, false);
                float h = splash.bounds.size.y;
                bgGo.transform.localScale = Vector3.one * (h > 0f ? 1.1f / h : 1f);
                bg = bgGo.AddComponent<SpriteRenderer>();
                bg.sprite = splash;
                bg.sortingOrder = 11;
            }

            DamagePopup popup = go.AddComponent<DamagePopup>();
            popup.StartCoroutine(popup.Run(tm, shadow, bg));
        }

        private IEnumerator Run(TextMesh tm, TextMesh shadow, SpriteRenderer bg)
        {
            Vector3 start = transform.position;
            Vector3 end = start + new Vector3(0f, RiseDistance, 0f);
            float t = 0f;
            Color startColor = tm.color;
            Color shadowColor = shadow.color;
            while (t < Duration)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / Duration);
                transform.position = Vector3.Lerp(start, end, p);
                float a = 1f - p * p;
                tm.color = new Color(startColor.r, startColor.g, startColor.b, a);
                shadow.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, a);
                if (bg != null) bg.color = new Color(1f, 1f, 1f, a);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
