using UnityEngine;

namespace BoolPyeon.UI
{
    public static class UiTheme
    {
        // Colors (Docs/UI_DESIGN.md §2.5)
        public static readonly Color BgDark = new Color32(0x14, 0x14, 0x1C, 0xFF);
        public static readonly Color PanelBase = new Color32(0x23, 0x23, 0x2E, 0xFF);
        public static readonly Color PanelBorder = new Color32(0x3A, 0x3A, 0x4A, 0xFF);
        public static readonly Color TextPrimary = new Color32(0xF2, 0xF2, 0xF5, 0xFF);
        public static readonly Color TextSecondary = new Color32(0xA0, 0xA0, 0xAE, 0xFF);
        public static readonly Color AccentPrimary = new Color32(0x4E, 0x8C, 0xFF, 0xFF);
        public static readonly Color HpRed = new Color32(0xE5, 0x48, 0x4D, 0xFF);
        public static readonly Color EnergyBlue = new Color32(0x3E, 0xC5, 0xE8, 0xFF);
        public static readonly Color BossPurple = new Color32(0x9B, 0x5D, 0xE5, 0xFF);
        public static readonly Color SuccessGreen = new Color32(0x3D, 0xD6, 0x8C, 0xFF);
        public static readonly Color FailRed = new Color32(0xD6, 0x45, 0x45, 0xFF);
        public static readonly Color DisabledGray = new Color32(0x5A, 0x5A, 0x66, 0xFF);

        // Font sizes (§2.4)
        public const int FontDisplay = 72;
        public const int FontH1 = 56;
        public const int FontH2 = 44;
        public const int FontBody = 36;
        public const int FontCaption = 28;
        public const int FontMin = 24;

        // Sizes (§2.3 / §4 / §8)
        public const int ButtonMinSize = 100;
        public const int ButtonW = 320;
        public const int ButtonH = 120;
        public const int CommandButtonW = 360;
        public const int CommandButtonH = 140;
        public const int ButtonSpacing = 16;
        public const int CommandSpacing = 24;
        public const int TopBarHeight = 140;
        public const int SafeAreaX = 48;
        public const int SafeAreaY = 32;
        public const int PanelBorderWidth = 4;

        // Canvas sorting order (§2.6)
        public const int SortBackground = 0;
        public const int SortWorld = 10;
        public const int SortHud = 20;
        public const int SortPanel = 30;
        public const int SortDialogue = 40;
        public const int SortFade = 50;

        public static Font DefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }
}
