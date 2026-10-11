using System;

namespace BoolPyeon.Save
{
    // F-20: 영구 재화·강화 저장용 항목 하나. id 는 UpgradeDefinition.name(에셋 이름)을 쓴다.
    [Serializable]
    public class UpgradeLevelEntry
    {
        public string id;
        public int level;
    }
}
