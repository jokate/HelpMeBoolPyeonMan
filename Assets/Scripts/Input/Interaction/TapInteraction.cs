using System;
using UnityEngine;

namespace BoolPyeon.Input.Interaction
{
    /// <summary>
    /// 클릭(탭)형 입력. 매니저의 Update 에서 Tick 을 부른다.
    /// </summary>
    [Serializable]
    public class TapInteraction
    {
        public bool ignoreOverUI = true;

        /// <summary>이번 프레임에 탭이 있었으면 true. hit 은 맞은 오브젝트(없으면 null).</summary>
        public bool Tick(out GameObject hit)
        {
            hit = null;
            if (!PointerInput.Down)
            {
                return false;
            }
            RaycastHit2D result = PointerInput.RaycastWorld2D(null, ignoreOverUI);
            if (result.collider != null)
            {
                hit = result.transform.gameObject;
            }
            return true;
        }
    }
}
