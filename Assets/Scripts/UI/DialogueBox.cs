using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Dialogue;

namespace BoolPyeon.UI
{
    // F-22: 대사 박스 UI. 화면 전체를 덮는 tapButton 을 눌러 DialogueSequence 를 한 줄씩 진행시킨다.
    public class DialogueBox : MonoBehaviour
    {
        public GameObject root;
        public Image portrait;
        public Text nameText;
        public Text bodyText;
        public Button tapButton;
        // T1: 선택 참조. 비어 있으면 결과가 line.portrait 와 같아 기존 동작이 유지된다. 연결은 T2.
        public SpeakerExpressionRegistry expressionRegistry;
        // T3: 초상화 펀치 연출. 이전 줄과 스프라이트가 달라지면 재생한다.
        public float punchDuration = PortraitPunch.DefaultDuration;
        public float punchPeak = PortraitPunch.DefaultPeak;

        public event Action Finished;

        private DialogueSequence sequence;
        private Sprite lastSprite;
        private Coroutine punchRoutine;

        // 주의: 씬은 이 오브젝트를 처음부터 비활성 상태로 둔다. Awake 에서 스스로 SetActive(false) 를
        // 부르지 않는다 — AgentMenuPanel.Awake(Assets/Scripts/Hub/AgentMenuPanel.cs L38-40)와 같은 이유로,
        // Play() 가 SetActive(true) 로 처음 켤 때 같은 프레임에 다시 꺼버리는 버그를 막기 위해서다.
        private void Awake()
        {
            EnsureListener();
        }

        public bool IsPlaying { get; private set; }
        public int CurrentIndex => sequence != null ? sequence.Index : 0;

        public void Play(DialogueDefinition dialogue)
        {
            Play(dialogue != null ? dialogue.lines : null);
        }

        public void Play(IReadOnlyList<DialogueLine> lines)
        {
            // 재생 중에 다시 부르면 처음부터 다시 재생한다.
            EnsureListener();
            StopPunch();
            lastSprite = null;
            IsPlaying = true;
            sequence = new DialogueSequence(lines);
            sequence.Finished += OnFinished;

            var rootObj = root != null ? root : gameObject;
            rootObj.SetActive(true);

            sequence.Begin();
            if (!sequence.IsFinished)
            {
                Render(sequence.Current);
            }
        }

        public void Advance()
        {
            if (sequence == null || !IsPlaying) return;
            if (sequence.Advance())
            {
                Render(sequence.Current);
            }
        }

        public void Render(DialogueLine line)
        {
            if (line == null) return;
            if (nameText != null) nameText.text = line.speakerName;
            if (bodyText != null) bodyText.text = line.text;
            if (portrait != null)
            {
                var sprite = PortraitResolver.Resolve(line, expressionRegistry);
                var shouldPunch = PortraitPunch.ShouldPunch(lastSprite, sprite);
                StopPunch();
                portrait.sprite = sprite;
                portrait.enabled = sprite != null;
                if (shouldPunch && isActiveAndEnabled)
                {
                    punchRoutine = StartCoroutine(PunchRoutine());
                }
                lastSprite = sprite;
            }
        }

        private IEnumerator PunchRoutine()
        {
            float elapsed = 0f;
            while (elapsed < punchDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (portrait != null)
                {
                    portrait.rectTransform.localScale = Vector3.one * PortraitPunch.EvaluateScale(elapsed, punchDuration, punchPeak);
                }
                yield return null;
            }

            if (portrait != null)
            {
                portrait.rectTransform.localScale = Vector3.one;
            }
            punchRoutine = null;
        }

        private void StopPunch()
        {
            if (punchRoutine != null)
            {
                StopCoroutine(punchRoutine);
                punchRoutine = null;
            }
            if (portrait != null)
            {
                portrait.rectTransform.localScale = Vector3.one;
            }
        }

        private void OnFinished()
        {
            IsPlaying = false;
            StopPunch();
            var rootObj = root != null ? root : gameObject;
            rootObj.SetActive(false);
            Finished?.Invoke();
        }

        private void OnDisable()
        {
            StopPunch();
        }

        private void EnsureListener()
        {
            if (tapButton == null) return;
            tapButton.onClick.RemoveListener(Advance);
            tapButton.onClick.AddListener(Advance);
        }
    }
}
