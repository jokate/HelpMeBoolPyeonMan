using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-28: 모듈 요원 메뉴에서 장착하는 특수 모듈 하나.
    public enum ModuleKind
    {
        FirstStrike,
        RepeatTurn
    }

    [CreateAssetMenu(menuName = "BoolPyeon/Meta/Module", fileName = "Module_")]
    public class ModuleDefinition : ScriptableObject
    {
        public string displayName;
        [TextArea] public string description;
        // 모듈 행 아이콘(없으면 행에서 숨긴다)
        public Sprite icon;
        public bool unlockedByDefault = true;
        public ModuleKind kind;
        // RepeatTurn 전용: 추가 커맨드 페이즈 간격(기본 3). FirstStrike 는 쓰지 않는다.
        public int param = 3;

        public IModuleEffect CreateEffect()
        {
            switch (kind)
            {
                case ModuleKind.FirstStrike: return new FirstStrikeEffect();
                case ModuleKind.RepeatTurn: return new RepeatTurnEffect(param);
                default: return null;
            }
        }
    }
}
