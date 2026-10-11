using NUnit.Framework;

namespace BoolPyeon.Battle.Tests
{
    // F-14 검증: BattleScenarioSimulator 의 S1~S6 시나리오가 모두 PASS 하는지 EditMode 에서 확인한다.
    public class BattleScenarioTests
    {
        [Test]
        public void S1_OneMonster_NormalAttackHits()
        {
            var r = BattleScenarioSimulator.S1_OneMonster_NormalAttackHits();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S2_ThreeMonsters_AlwaysFailEvade()
        {
            var r = BattleScenarioSimulator.S2_ThreeMonsters_AlwaysFailEvade();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S3_GuardCommand_HalvesDamage()
        {
            var r = BattleScenarioSimulator.S3_GuardCommand_HalvesDamage();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S4_EnergyCostsAndCapAndCarryOver()
        {
            var r = BattleScenarioSimulator.S4_EnergyCostsAndCapAndCarryOver();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S5_SpecialInterval_PenaltyReserved()
        {
            var r = BattleScenarioSimulator.S5_SpecialInterval_PenaltyReserved();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S6_PlayerDeath_Defeat()
        {
            var r = BattleScenarioSimulator.S6_PlayerDeath_Defeat();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S8_Stage1Boss()
        {
            var r = BattleScenarioSimulator.S8_Stage1Boss();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S12_Squad_Stage1Trash()
        {
            var r = BattleScenarioSimulator.S12_Squad_Stage1Trash();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S13_Squad_Stage1Elite()
        {
            var r = BattleScenarioSimulator.S13_Squad_Stage1Elite();
            Assert.IsTrue(r.Pass, r.ToString());
        }

        [Test]
        public void S14_Squad_Stage1Boss()
        {
            var r = BattleScenarioSimulator.S14_Squad_Stage1Boss();
            Assert.IsTrue(r.Pass, r.ToString());
        }

#if UNITY_EDITOR
        // T10: 실제 Hero/Char_Cook/Char_Fashion 에셋 스탯·trait 과 실제 스킬 코스트로 1스테이지 첫 잡몹 웨이브를 돌린다.
        [Test]
        public void S15_Squad_Stage1AssetTrash()
        {
            var hero = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterDefinition>("Assets/Data/Battle/Hero.asset");
            var cook = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterDefinition>("Assets/Data/Battle/Char_Cook.asset");
            var fashion = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterDefinition>("Assets/Data/Battle/Char_Fashion.asset");
            var attack = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillDefinition>("Assets/Data/Battle/AttackSkill.asset");
            var heavy = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillDefinition>("Assets/Data/Battle/HeavyBlowSkill.asset");
            Assert.IsNotNull(hero); Assert.IsNotNull(cook); Assert.IsNotNull(fashion);
            Assert.IsNotNull(attack); Assert.IsNotNull(heavy);

            var members = new System.Collections.Generic.List<BattleScenarioSimulator.SquadMemberSpec>
            {
                new BattleScenarioSimulator.SquadMemberSpec { Name = hero.id, Hp = hero.maxHp, Atk = hero.attack, Def = hero.defense, Trait = hero.trait, Skills = new[] { attack, heavy } },
                new BattleScenarioSimulator.SquadMemberSpec { Name = cook.id, Hp = cook.maxHp, Atk = cook.attack, Def = cook.defense, Trait = cook.trait, Skills = new[] { attack, heavy } },
                new BattleScenarioSimulator.SquadMemberSpec { Name = fashion.id, Hp = fashion.maxHp, Atk = fashion.attack, Def = fashion.defense, Trait = fashion.trait, Skills = new[] { attack, heavy } },
            };

            var stage = LoadStage(Stage1);
            var wave = LoadWave("Assets/Data/Battle/Stage1/Wave_S1_TrashGrunt.asset");
            // T10: 3인 스쿼드는 RunWaveBalance 의 단일 플레이어 모델보다 화력이 3배 가까워 Stage_1.monsterHpMultiplier(1.0)
            // 을 그대로 쓰면 2턴 안에 끝난다. Stage_1 배율은 S9~S11(단일 모델)이 쓰므로 바꾸지 않고, 스쿼드 전용 배율(2.2)을 로컬로 둔다.
            const float squadHpMul = 2.2f;
            var r = BattleScenarioSimulator.RunSquadBalance("S15", members, wave.monsters, squadHpMul, stage.monsterAttackMultiplier, 3, 5);
            UnityEngine.Debug.Log(r.ToString());
            Assert.IsTrue(r.Pass, r.ToString());
        }

        // T8(캠페인): 방어·마법·지원 조합 웨이브와 Boss_Stage2 를 실제 에셋·스테이지 배율로 돌려
        // D-12(예고 피해 ≤ 기준HP 30 의 25%, 행동 ≤2)와 N턴 안 처치(방어형이 끝없이 버티지 않음)를 확인한다.
        const int NormalTurnCap = 15;
        const int EliteTurnCap = 18;
        const int BossTurnCap = 25;

        static WaveDefinition LoadWave(string path)
        {
            var w = UnityEditor.AssetDatabase.LoadAssetAtPath<WaveDefinition>(path);
            Assert.IsNotNull(w, path + " missing");
            return w;
        }

        static StageDefinition LoadStage(string path)
        {
            var s = UnityEditor.AssetDatabase.LoadAssetAtPath<StageDefinition>(path);
            Assert.IsNotNull(s, path + " missing");
            return s;
        }

        static void AssertWave(string stagePath, string wavePath, int turnCap, params EnemyIntentKind[] require)
        {
            var stage = LoadStage(stagePath);
            var wave = LoadWave(wavePath);
            var r = BattleScenarioSimulator.RunWaveBalance(wave.name, wave.monsters,
                stage.monsterHpMultiplier, stage.monsterAttackMultiplier, turnCap, 10, 30f, require);
            UnityEngine.Debug.Log(r.ToString());
            Assert.IsTrue(r.Pass, r.ToString());
        }

        const string Stage1 = "Assets/Data/Battle/Stage1/Stage_1.asset";
        const string Stage2 = "Assets/Data/Battle/Stage2/Stage_2.asset";

        [TestCase("Wave_S1_FairyGlove", NormalTurnCap, EnemyIntentKind.Guard, EnemyIntentKind.Magic)]
        [TestCase("Wave_S1_GloveTrash", NormalTurnCap, EnemyIntentKind.Guard)]
        [TestCase("Wave_S1_MixerMosq", NormalTurnCap)]
        [TestCase("Wave_S1_Trio", EliteTurnCap, EnemyIntentKind.Guard, EnemyIntentKind.Magic)]
        public void S9_Stage1_NewMonsterWaves(string wave, int cap, params EnemyIntentKind[] require)
        {
            AssertWave(Stage1, "Assets/Data/Battle/Stage1/" + wave + ".asset", cap, require);
        }

        [TestCase("Wave_S2_Cable2", NormalTurnCap, EnemyIntentKind.Guard)]
        [TestCase("Wave_S2_AdAd", NormalTurnCap, EnemyIntentKind.Magic)]
        [TestCase("Wave_S2_CablePrinter", NormalTurnCap, EnemyIntentKind.Guard)]
        [TestCase("Wave_S2_PrinterAd", NormalTurnCap, EnemyIntentKind.Magic)]
        [TestCase("Wave_S2_EliteRush", EliteTurnCap, EnemyIntentKind.Magic)]
        [TestCase("Wave_S2_EliteWall", EliteTurnCap, EnemyIntentKind.Guard, EnemyIntentKind.Magic)]
        public void S10_Stage2_Waves(string wave, int cap, params EnemyIntentKind[] require)
        {
            AssertWave(Stage2, "Assets/Data/Battle/Stage2/" + wave + ".asset", cap, require);
        }

        [Test]
        public void S11_Stage2Boss()
        {
            AssertWave(Stage2, "Assets/Data/Battle/Stage2/Wave_Stage2Boss.asset", BossTurnCap,
                EnemyIntentKind.Guard, EnemyIntentKind.Magic, EnemyIntentKind.Special);
        }
#endif
    }
}
