using System.Collections.Generic;
using UnityEngine;

namespace BoolPyeon.Dialogue
{
    // T1: 여러 화자의 SpeakerExpressionSet 을 이름으로 조회하는 레지스트리.
    // T1.1 확인(2026-10-01): DLG_* 에서 쓰는 화자 이름과 기존 초상화 이미지 폴더 매핑.
    // T2 에서 이 목록대로 SpeakerExpressionSet 에셋을 만들면 이미지 재생성 불필요.
    //   "불편맨"        -> Assets/SampleImage/Battle/Portraits/Hero_Portrait_*        (Default/Joy/Worried)
    //   "스토리 요원"    -> Assets/SampleImage/Hub/Agents/Agent_Story_Portrait_*       (Default/Concerned/Joy/Surprised/Cheer/Embarrassed)
    //   "강화 요원"      -> Assets/SampleImage/Hub/Agents/Agent_Upgrade_Portrait_*      (Default/Encourage)
    //   "모듈 요원"      -> Assets/SampleImage/Hub/Agents/Agent_Module_Portrait_*       (Default/Thinking)
    //   "불편의 악마"    -> Assets/SampleImage/Dialogue/Speakers/Devil_Portrait_*       (Default/Angry/Laugh/Shocked/Annoyed)
    //   "회사원"        -> Assets/SampleImage/Dialogue/Speakers/Coworker_Portrait_*     (Default/Panic/Determined/Relieved/Tired)
    //   "안내"          -> Assets/SampleImage/Dialogue/Speakers/Guide_Portrait_*        (Default 만)
    //   "COMFORT 사령관" -> Assets/SampleImage/Dialogue/Speakers/ComfortCommander_Portrait_* (Default 만)
    // Cook/Fashion(Assets/SampleImage/Battle/Portraits/Cook_*, Fashion_*)은 DLG_* 대사 화자로는 아직 안 쓰이지만 같은 Default/Joy/Worried 세트 보유.
    [CreateAssetMenu(menuName = "BoolPyeon/Dialogue/Speaker Expression Registry", fileName = "EXPR_Registry")]
    public class SpeakerExpressionRegistry : ScriptableObject
    {
        public List<SpeakerExpressionSet> sets = new List<SpeakerExpressionSet>();

        public SpeakerExpressionSet Find(string speakerName)
        {
            if (string.IsNullOrEmpty(speakerName) || sets == null) return null;

            for (int i = 0; i < sets.Count; i++)
            {
                var set = sets[i];
                if (set != null && string.Equals(set.speakerName, speakerName, System.StringComparison.Ordinal))
                    return set;
            }
            return null;
        }
    }
}
