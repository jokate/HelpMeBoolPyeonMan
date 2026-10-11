using System.Collections;
using UnityEngine;

namespace BoolPyeon.BattleFlow
{
    /// <summary>
    /// 스프라이트 프레임을 한 번 재생하고 사라지는 월드 이펙트(피격 스파크, 회복 반짝임 등).
    /// </summary>
    public class SpriteFx : MonoBehaviour
    {
        public static void Play(Sprite[] frames, Vector3 pos, float height, int sortingOrder, float fps = 16f)
        {
            if (frames == null || frames.Length == 0 || frames[0] == null) return;
            var go = new GameObject("SpriteFx");
            go.transform.position = pos;
            float h = frames[0].bounds.size.y;
            go.transform.localScale = Vector3.one * (h > 0f ? height / h : 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = sortingOrder;
            var fx = go.AddComponent<SpriteFx>();
            fx.StartCoroutine(fx.Run(sr, frames, fps));
        }

        private IEnumerator Run(SpriteRenderer sr, Sprite[] frames, float fps)
        {
            float step = 1f / Mathf.Max(1f, fps);
            foreach (var f in frames)
            {
                sr.sprite = f;
                yield return new WaitForSeconds(step);
            }
            Destroy(gameObject);
        }
    }
}
