using UnityEngine;
using UnityEngine.SceneManagement;
using BoolPyeon.Battle;

namespace BoolPyeon.BattleFlow
{
    /// <summary>
    /// 디버그 키: 1 = 1마리 웨이브로 재시작, 3 = 3마리 웨이브로 재시작, E = 차례인 멤버 에너지 +5, K = 적 전멸,
    /// F = 차례인 멤버 한 명 사망, D = 편성 전원 사망(패배 검증용),
    /// I = 다음 행동 예고 종류를 순서대로(Attack→Special→Guard→Magic→Buff→Heal→Wait) 강제 표시(T5 검증용).
    /// </summary>
    public class BattleDebug : MonoBehaviour
    {
        public BattleController controller;

        private static readonly EnemyIntentKind[] PreviewOrder =
        {
            EnemyIntentKind.Attack, EnemyIntentKind.Special, EnemyIntentKind.Guard,
            EnemyIntentKind.Magic, EnemyIntentKind.Buff, EnemyIntentKind.Heal, EnemyIntentKind.Wait
        };
        private int previewIndex;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) RestartWithWave(1);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) RestartWithWave(3);
            if (UnityEngine.Input.GetKeyDown(KeyCode.E)) AddEnergy();
            if (UnityEngine.Input.GetKeyDown(KeyCode.K)) KillAllEnemies();
            if (UnityEngine.Input.GetKeyDown(KeyCode.D)) KillPlayer();
            if (UnityEngine.Input.GetKeyDown(KeyCode.F)) KillActiveMember();
            if (UnityEngine.Input.GetKeyDown(KeyCode.I)) PreviewNextIntent();
        }

        // T5: 키 I 를 누를 때마다 PreviewOrder 의 다음 종류를 살아 있는 몬스터 전부에 강제 표시한다
        [ContextMenu("Preview Next Intent")]
        public void PreviewNextIntent()
        {
            EnemyIntentKind kind = PreviewOrder[previewIndex % PreviewOrder.Length];
            previewIndex++;
            PreviewIntents(kind);
        }

        public void PreviewIntents(EnemyIntentKind kind)
        {
            if (controller == null || controller.State == null) return;
            var enemies = controller.State.Enemies;
            var views = FindObjectsByType<BattleUnitView>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsDead) continue;
                var intent = new EnemyIntent(i, kind, Mathf.Max(1, enemies[i].Attack));
                foreach (var v in views)
                {
                    if (v.Unit == enemies[i])
                    {
                        v.Refresh(intent);
                        if (v.intentIcon != null && v.intentText != null)
                        {
                            Debug.Log($"[IntentPreview] kind={kind} text={v.intentText.text} color={v.intentIcon.color}");
                        }
                        else
                        {
                            Debug.Log($"[IntentPreview] kind={kind} (icon/text 미연결)");
                        }
                        break;
                    }
                }
            }
        }

        [ContextMenu("Restart 1 Monster")]
        public void RestartOne() => RestartWithWave(1);

        [ContextMenu("Restart 3 Monsters")]
        public void RestartThree() => RestartWithWave(3);

        public void RestartWithWave(int count)
        {
            BattleController.DebugWaveCount = count;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex >= 0
                ? SceneManager.GetActiveScene().name
                : SceneManager.GetActiveScene().path);
        }

        [ContextMenu("Energy +5 (Active Member)")]
        public void AddEnergyToActive() => AddEnergy();

        // T8: memberIndex 가 -1 이면 지금 차례인 멤버에게 에너지를 더한다.
        public void AddEnergy(int memberIndex = -1)
        {
            if (controller != null && controller.State != null)
            {
                int idx = memberIndex >= 0 ? memberIndex : controller.State.ActiveMemberIndex;
                controller.State.AddEnergyTo(idx, 5);
                string id = idx >= 0 && idx < controller.State.Members.Count ? controller.State.Members[idx].Definition?.id : null;
                Debug.Log($"[Debug] energy member={id} +5");
                controller.RefreshAll();
            }
        }

        [ContextMenu("Kill All Enemies")]
        public void KillAllEnemies()
        {
            if (controller != null && controller.State != null)
            {
                controller.State.DebugDefeatAllEnemies();
                controller.RefreshAll();
            }
        }

        [ContextMenu("Kill Player (All Members)")]
        public void KillPlayer()
        {
            if (controller != null && controller.State != null)
            {
                controller.State.DebugKillPlayer();
                controller.RefreshAll();
            }
        }

        [ContextMenu("Kill Active Member")]
        public void KillActiveMember() => KillMember();

        // T8: memberIndex 가 -1 이면 지금 차례인 멤버를 죽인다. 한 명이 죽어도 전투가 이어지는지 확인용.
        public void KillMember(int memberIndex = -1)
        {
            if (controller != null && controller.State != null)
            {
                int idx = memberIndex >= 0 ? memberIndex : controller.State.ActiveMemberIndex;
                controller.State.DebugKillMember(idx);
                controller.RefreshAll();
            }
        }
    }
}
