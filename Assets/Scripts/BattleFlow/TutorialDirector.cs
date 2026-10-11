using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoolPyeon.Dialogue;
using BoolPyeon.UI;

namespace BoolPyeon.BattleFlow
{
    // F-23: 튜토리얼 런에서 단계마다 대사를 재생한다. key 마다 한 번만 재생한다.
    public class TutorialDirector : MonoBehaviour
    {
        public DialogueBox box;
        public DialogueDefinition opening;
        public DialogueDefinition energyGuide;
        public DialogueDefinition commandGuide;
        // T6: 일반 공격 회피 제거로 더 이상 재생하지 않는다. 데이터 보존용으로 숨긴다.
        [HideInInspector]
        [System.Obsolete("T6: 일반 공격 회피 제거, 데이터 보존용")]
        public DialogueDefinition evasionGuide;
        public DialogueDefinition bossIntro;
        public DialogueDefinition outro;

        private readonly HashSet<string> played = new HashSet<string>();

        public IEnumerator PlayOnce(DialogueDefinition d, string key)
        {
            if (played.Contains(key)) yield break;
            played.Add(key);
            if (box == null)
            {
                Debug.Log($"[Tutorial] step {key} (no box)");
                yield break;
            }
            if (d == null)
            {
                Debug.Log($"[Tutorial] step {key} (no dialogue)");
                yield break;
            }
            bool finished = false;
            void OnFinished() => finished = true;
            box.Finished += OnFinished;
            box.Play(d);
            // T7.2: BattleAutoPilot 이 켜져 있으면(자동 플레이/테스트) 실제 탭 없이도 대사를 자동으로 넘긴다.
            // 그렇지 않으면 이전처럼 사람의 탭(box.tapButton)만 기다린다.
            bool auto = PlayerPrefs.GetInt(BattleAutoPilot.PrefsKey, 0) == 1;
            while (!finished)
            {
                if (auto) box.Advance();
                yield return null;
            }
            box.Finished -= OnFinished;
            Debug.Log($"[Tutorial] step {key}");
        }
    }
}
