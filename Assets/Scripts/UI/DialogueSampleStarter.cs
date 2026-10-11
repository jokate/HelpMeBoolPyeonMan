using UnityEngine;
using BoolPyeon.Dialogue;

namespace BoolPyeon.UI
{
    // F-22 검증용: DialogueSample 씬에서 샘플 대사를 재생한다.
    public class DialogueSampleStarter : MonoBehaviour
    {
        public DialogueDefinition dialogue;
        public DialogueBox box;

        private void Start()
        {
            if (box == null || dialogue == null) return;
            box.Finished += OnFinished;
            box.Play(dialogue);
        }

        private void OnFinished()
        {
            Debug.Log("[Dialogue] 샘플 대사 완료");
        }
    }
}
