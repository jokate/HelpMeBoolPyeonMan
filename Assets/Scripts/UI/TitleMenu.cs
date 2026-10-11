using UnityEngine;
using UnityEngine.UI;

namespace BoolPyeon.UI
{
    /// <summary>
    /// Title 하단 보조 메뉴(설정·공지·크레딧) 버튼과 팝업 하나를 다룬다.
    /// 팝업이 열려 있는 동안에는 시작 입력이 막혀야 한다 (TitleScreen.menu 참고).
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {
        public Button settingsButton;
        public Button noticeButton;
        public Button creditsButton;
        public Button closeButton;
        public Button dimButton;
        public GameObject popupRoot;
        public Text titleLabel;
        public Text bodyLabel;

        public bool IsOpen { get; private set; }

        private void Start()
        {
            if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
            if (noticeButton != null) noticeButton.onClick.AddListener(OpenNotice);
            if (creditsButton != null) creditsButton.onClick.AddListener(OpenCredits);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (dimButton != null) dimButton.onClick.AddListener(Close);

            if (popupRoot != null)
            {
                popupRoot.SetActive(false);
            }

            IsOpen = false;
        }

        public void OpenSettings()
        {
            Open("설정", "설정 준비 중");
        }

        public void OpenNotice()
        {
            Open("공지", "공지 없음");
        }

        public void OpenCredits()
        {
            Open("크레딧", "제작: 불편맨 팀");
        }

        private void Open(string title, string body)
        {
            if (titleLabel != null) titleLabel.text = title;
            if (bodyLabel != null) bodyLabel.text = body;

            if (popupRoot != null)
            {
                popupRoot.SetActive(true);
            }

            IsOpen = true;
        }

        public void Close()
        {
            if (popupRoot != null)
            {
                popupRoot.SetActive(false);
            }

            IsOpen = false;
        }
    }
}
