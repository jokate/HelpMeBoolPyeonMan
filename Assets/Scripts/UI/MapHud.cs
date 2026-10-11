using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoolPyeon.Battle;

namespace BoolPyeon.UI
{
    // T3: 격자 미로 지도 HUD. 현재 위치 강조·방문/클리어 표시·안개(hideUnvisitedTypes)·방향 버튼 4개만 활성화한다.
    // buttons/labels 의 0~3번을 상/하/좌/우로 쓴다. showRequested 로 Awake 자동 숨김을 막는 기존 동작은 유지한다.
    public class MapHud : MonoBehaviour
    {
        public GameObject panel;
        public Text titleText;
        public List<Button> buttons = new List<Button>();
        public List<Text> labels = new List<Text>();

        [Header("Grid")]
        public RectTransform gridRoot;
        public float cellSize = 100f;
        public float cellSpacing = 14f;
        public bool hideUnvisitedTypes = true;

        // T4.1: 방 유형 아이콘(MazeIcon_5종). 비어 있으면 기존 글자(LetterFor)로 대체 표시한다.
        [Header("Room Icons (MazeIcon_5종, 비우면 글자로 대체)")]
        public Sprite startIcon;
        public Sprite combatIcon;
        public Sprite eliteIcon;
        public Sprite restIcon;
        public Sprite bossIcon;

        public static readonly Color CombatColor = Color.white;
        public static readonly Color EliteColor = Color.red;
        public static readonly Color RestColor = Color.green;
        public static readonly Color BossColor = new Color(0.4f, 0f, 0.6f);
        public static readonly Color VisitedColor = Color.gray;
        public static readonly Color CurrentHighlight = Color.yellow;
        public static readonly Color FogColor = new Color(0.25f, 0.25f, 0.25f);
        public static readonly Color ClearedColor = new Color(0.2f, 0.2f, 0.2f);

        static readonly Vector2Int[] DirOrder = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        static readonly string[] DirLabel = { "위", "아래", "왼쪽", "오른쪽" };

        public event Action<Vector2Int> DirectionSelected;

        private bool showRequested;

        // 런타임 생성 그리드 캐시. 칸 수(Width*Height)가 바뀔 때만 다시 만든다.
        private int builtWidth = -1;
        private int builtHeight = -1;
        private RectTransform[,] cellRects;
        private Image[,] cellImages;
        private Text[,] cellTexts;
        private Image[,] cellOutlines;
        private Image[,] cellIcons;
        // 통로 bar: passH[x,y] 는 (x,y)-(x+1,y) 사이 가로 통로, passV[x,y] 는 (x,y)-(x,y+1) 사이 세로 통로.
        private Image[,] passH;
        private Image[,] passV;

        private void Awake()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                if (buttons[i] != null)
                    buttons[i].onClick.AddListener(() => OnButtonClicked(index));
            }
            if (!showRequested) Hide();
        }

        private void OnButtonClicked(int buttonIndex)
        {
            if (buttonIndex < 0 || buttonIndex >= DirOrder.Length) return;
            DirectionSelected?.Invoke(DirOrder[buttonIndex]);
        }

        public void Show(StageMaze maze)
        {
            if (maze == null) return;
            if (titleText != null) titleText.text = $"미로 ({maze.CurrentPos.x}, {maze.CurrentPos.y})";

            BuildGrid(maze);
            RefreshGrid(maze);

            var movable = maze.GetMovableDirections();
            for (int b = 0; b < buttons.Count; b++)
            {
                bool exists = b < DirOrder.Length;
                if (buttons[b] != null) buttons[b].gameObject.SetActive(exists);
                if (!exists) continue;

                var dir = DirOrder[b];
                bool isSelectable = movable.Contains(dir);
                var neighborPos = maze.CurrentPos + dir;
                var neighbor = maze.GetRoom(neighborPos);

                bool showType = neighbor != null && (!hideUnvisitedTypes || neighbor.visited
                    || neighbor.type == MazeRoomType.Boss || neighbor.type == MazeRoomType.Start);

                if (b < labels.Count && labels[b] != null)
                    labels[b].text = neighbor == null ? $"{DirLabel[b]}: 벽"
                        : showType ? $"{DirLabel[b]}: {LabelFor(neighbor.type)}" : $"{DirLabel[b]}: ?";

                var image = buttons[b].targetGraphic as Image;
                Color color = neighbor == null ? VisitedColor
                    : !showType ? FogColor
                    : neighbor.cleared ? ClearedColor
                    : ColorFor(neighbor.type);
                if (image != null) image.color = color;
                buttons[b].interactable = isSelectable;
            }
            showRequested = true;
            if (panel != null) panel.SetActive(true);
        }

        public void Hide()
        {
            showRequested = false;
            if (panel != null) panel.SetActive(false);
        }

        // 칸 수(Width*Height)가 바뀌었을 때만 gridRoot 아래에 Cell_x_y 를 새로 만든다.
        void BuildGrid(StageMaze maze)
        {
            if (gridRoot == null) return;
            if (builtWidth == maze.Width && builtHeight == maze.Height) return;

            for (int i = gridRoot.childCount - 1; i >= 0; i--)
                DestroyImmediate(gridRoot.GetChild(i).gameObject);

            int w = maze.Width;
            int h = maze.Height;
            cellRects = new RectTransform[w, h];
            cellImages = new Image[w, h];
            cellTexts = new Text[w, h];
            cellOutlines = new Image[w, h];
            cellIcons = new Image[w, h];
            passH = new Image[w, h];
            passV = new Image[w, h];

            Font font = titleText != null ? titleText.font : Resources.GetBuiltinResource<Font>("Arial.ttf");
            float step = cellSize + cellSpacing;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var cellGo = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
                    var rt = cellGo.GetComponent<RectTransform>();
                    rt.SetParent(gridRoot, false);
                    rt.sizeDelta = new Vector2(cellSize, cellSize);
                    rt.anchoredPosition = new Vector2((x - (w - 1) / 2f) * step, (y - (h - 1) / 2f) * step);

                    var img = cellGo.AddComponent<Image>();
                    img.color = FogColor;
                    cellImages[x, y] = img;

                    var outlineGo = new GameObject("Outline", typeof(RectTransform));
                    var ort = outlineGo.GetComponent<RectTransform>();
                    ort.SetParent(rt, false);
                    ort.anchorMin = Vector2.zero;
                    ort.anchorMax = Vector2.one;
                    ort.offsetMin = Vector2.zero;
                    ort.offsetMax = Vector2.zero;
                    var outlineImg = outlineGo.AddComponent<Image>();
                    outlineImg.color = Color.clear;
                    var outline = outlineGo.AddComponent<Outline>();
                    outline.effectColor = CurrentHighlight;
                    outline.effectDistance = new Vector2(3f, 3f);
                    cellOutlines[x, y] = outlineImg;

                    var textGo = new GameObject("Text", typeof(RectTransform));
                    var trt = textGo.GetComponent<RectTransform>();
                    trt.SetParent(rt, false);
                    trt.anchorMin = Vector2.zero;
                    trt.anchorMax = Vector2.one;
                    trt.offsetMin = Vector2.zero;
                    trt.offsetMax = Vector2.zero;
                    var text = textGo.AddComponent<Text>();
                    text.font = font;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.color = Color.black;
                    text.fontSize = 24;
                    cellTexts[x, y] = text;

                    // 방 유형 아이콘(MazeIcon_5종). sprite 가 없으면 비활성 상태로 두고 위 text(글자)로 대체된다.
                    var iconGo = new GameObject("Icon", typeof(RectTransform));
                    var irt = iconGo.GetComponent<RectTransform>();
                    irt.SetParent(rt, false);
                    irt.anchorMin = new Vector2(0.2f, 0.2f);
                    irt.anchorMax = new Vector2(0.8f, 0.8f);
                    irt.offsetMin = Vector2.zero;
                    irt.offsetMax = Vector2.zero;
                    var iconImg = iconGo.AddComponent<Image>();
                    iconImg.raycastTarget = false;
                    iconGo.SetActive(false);
                    cellIcons[x, y] = iconImg;

                    // 가로 통로(오른쪽 이웃과의 사이): x < w-1 인 칸에만 만든다.
                    if (x < w - 1)
                    {
                        var barGo = new GameObject($"PassH_{x}_{y}", typeof(RectTransform));
                        var brt = barGo.GetComponent<RectTransform>();
                        brt.SetParent(gridRoot, false);
                        brt.sizeDelta = new Vector2(cellSpacing, cellSize * 0.3f);
                        brt.anchoredPosition = new Vector2((x - (w - 1) / 2f) * step + step / 2f, (y - (h - 1) / 2f) * step);
                        var bimg = barGo.AddComponent<Image>();
                        bimg.color = VisitedColor;
                        barGo.SetActive(false);
                        passH[x, y] = bimg;
                    }
                    // 세로 통로(위쪽 이웃과의 사이): y < h-1 인 칸에만 만든다.
                    if (y < h - 1)
                    {
                        var barGo = new GameObject($"PassV_{x}_{y}", typeof(RectTransform));
                        var brt = barGo.GetComponent<RectTransform>();
                        brt.SetParent(gridRoot, false);
                        brt.sizeDelta = new Vector2(cellSize * 0.3f, cellSpacing);
                        brt.anchoredPosition = new Vector2((x - (w - 1) / 2f) * step, (y - (h - 1) / 2f) * step + step / 2f);
                        var bimg = barGo.AddComponent<Image>();
                        bimg.color = VisitedColor;
                        barGo.SetActive(false);
                        passV[x, y] = bimg;
                    }
                }
            }

            builtWidth = w;
            builtHeight = h;
        }

        // 칸 색·글자·강조를 최신 미로 상태로 갱신한다. 안개(hideUnvisitedTypes) 규칙: Start/Boss 는 항상 보인다.
        void RefreshGrid(StageMaze maze)
        {
            if (gridRoot == null || cellRects == null) return;

            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    var room = maze.GetRoom(x, y);
                    if (room == null) continue;

                    bool isCurrent = maze.CurrentPos.x == x && maze.CurrentPos.y == y;
                    bool alwaysVisible = room.type == MazeRoomType.Start || room.type == MazeRoomType.Boss;
                    bool showType = !hideUnvisitedTypes || room.visited || alwaysVisible;

                    var img = cellImages[x, y];
                    if (img != null)
                    {
                        Color c = !showType ? FogColor
                            : room.cleared ? ClearedColor
                            : room.visited ? VisitedColor
                            : ColorFor(room.type);
                        img.color = c;
                    }

                    var icon = cellIcons[x, y];
                    Sprite iconSprite = showType ? IconFor(room.type) : null;
                    if (icon != null)
                    {
                        icon.gameObject.SetActive(iconSprite != null);
                        if (iconSprite != null) icon.sprite = iconSprite;
                    }

                    var text = cellTexts[x, y];
                    if (text != null)
                    {
                        // 아이콘이 있으면 글자를 대체(숨김)한다. 아이콘이 없는 유형(통로 등)은 기존 글자를 유지한다.
                        if (iconSprite != null)
                        {
                            text.text = "";
                        }
                        else
                        {
                            string letter = showType ? LetterFor(room.type) : "?";
                            if (showType && room.cleared) letter += "v";
                            text.text = letter;
                        }
                    }

                    var rt = cellRects[x, y];
                    if (rt != null) rt.localScale = isCurrent ? new Vector3(1.1f, 1.1f, 1f) : Vector3.one;

                    var outline = cellOutlines[x, y];
                    if (outline != null) outline.color = isCurrent ? CurrentHighlight : Color.clear;

                    if (x < maze.Width - 1 && passH[x, y] != null)
                        passH[x, y].gameObject.SetActive(maze.HasPassage(new Vector2Int(x, y), Vector2Int.right));
                    if (y < maze.Height - 1 && passV[x, y] != null)
                        passV[x, y].gameObject.SetActive(maze.HasPassage(new Vector2Int(x, y), Vector2Int.up));
                }
            }
        }

        static string LetterFor(MazeRoomType type)
        {
            switch (type)
            {
                case MazeRoomType.Start: return "S";
                case MazeRoomType.Combat: return "전";
                case MazeRoomType.Elite: return "엘";
                case MazeRoomType.Rest: return "휴";
                case MazeRoomType.Boss: return "B";
                default: return "";
            }
        }

        // MazeIcon_5종 참조. 필드가 비어 있으면 null 반환 → 호출부가 글자 표시로 대체한다.
        Sprite IconFor(MazeRoomType type)
        {
            switch (type)
            {
                case MazeRoomType.Start: return startIcon;
                case MazeRoomType.Combat: return combatIcon;
                case MazeRoomType.Elite: return eliteIcon;
                case MazeRoomType.Rest: return restIcon;
                case MazeRoomType.Boss: return bossIcon;
                default: return null;
            }
        }

        static string LabelFor(MazeRoomType type)
        {
            switch (type)
            {
                case MazeRoomType.Combat: return "전투";
                case MazeRoomType.Elite: return "엘리트";
                case MazeRoomType.Rest: return "휴식";
                case MazeRoomType.Boss: return "보스";
                case MazeRoomType.Start: return "시작";
                default: return "통로";
            }
        }

        static Color ColorFor(MazeRoomType type)
        {
            switch (type)
            {
                case MazeRoomType.Combat: return CombatColor;
                case MazeRoomType.Elite: return EliteColor;
                case MazeRoomType.Rest: return RestColor;
                case MazeRoomType.Boss: return BossColor;
                default: return CombatColor;
            }
        }
    }
}
