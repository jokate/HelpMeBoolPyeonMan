using BoolPyeon.Battle;
using UnityEngine;

namespace BoolPyeon.Meta
{
    // F-28: 선제 타격 — 웨이브 전투가 시작될 때(OnBattleStart) 살아 있는 첫 적에게
    // character.attackSkill 기본 공격 1회를 에너지 소모 없이 적용한다.
    public class FirstStrikeEffect : IModuleEffect
    {
        private BattleEvents events;
        private BattleState state;
        private CharacterDefinition character;

        public void Attach(BattleEvents events, BattleState state, CharacterDefinition character)
        {
            this.events = events;
            this.state = state;
            this.character = character;
            if (events != null) events.OnBattleStart += OnBattleStart;
        }

        public void Detach()
        {
            if (events != null) events.OnBattleStart -= OnBattleStart;
            events = null;
            state = null;
            character = null;
        }

        private void OnBattleStart()
        {
            if (state == null || character == null || character.attackSkill == null) return;
            Combatant target = null;
            foreach (var e in state.Enemies)
            {
                if (!e.IsDead) { target = e; break; }
            }
            if (target == null) return;
            SkillResolver.TryUseFree(character.attackSkill, state.Player, state.Enemies, target, events);
            Debug.Log("[Module] 선제 타격!");
        }
    }
}
