using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoolPyeon.MiniGames.Tests
{
    public class MiniGamePoolFilterTests
    {
        private readonly List<MiniGameDefinition> _created = new List<MiniGameDefinition>();

        private MiniGameDefinition Make(MiniGameUsage usage, MiniGameCategory category)
        {
            var d = ScriptableObject.CreateInstance<MiniGameDefinition>();
            d.usage = usage;
            d.category = category;
            _created.Add(d);
            return d;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var d in _created)
            {
                if (d != null) Object.DestroyImmediate(d);
            }
            _created.Clear();
        }

        [Test]
        public void Filter_UsageAndCategory_ReturnsOnlyMatching()
        {
            var food = Make(MiniGameUsage.Energy, MiniGameCategory.Food);
            var wear = Make(MiniGameUsage.Energy, MiniGameCategory.Wear);
            var pool = new List<MiniGameDefinition> { food, wear };

            var result = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Energy, MiniGameCategory.Food);

            Assert.AreEqual(1, result.Count);
            Assert.AreSame(food, result[0]);
        }

        [Test]
        public void Filter_NoCategory_FiltersByUsageOnly()
        {
            var food = Make(MiniGameUsage.Energy, MiniGameCategory.Food);
            var wear = Make(MiniGameUsage.Energy, MiniGameCategory.Wear);
            var pool = new List<MiniGameDefinition> { food, wear };

            var result = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Energy);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Filter_MultiUsageFlag_IncludedInBothQueries()
        {
            var both = Make(MiniGameUsage.Energy | MiniGameUsage.Evasion, MiniGameCategory.Food);
            var pool = new List<MiniGameDefinition> { both };

            var energyResult = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Energy);
            var evasionResult = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Evasion);

            Assert.AreEqual(1, energyResult.Count);
            Assert.AreEqual(1, evasionResult.Count);
        }

        [Test]
        public void Filter_NullPool_ReturnsEmptyList()
        {
            var result = MiniGamePoolFilter.Filter(null, MiniGameUsage.Energy);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Filter_NullElement_IsSkipped()
        {
            var food = Make(MiniGameUsage.Energy, MiniGameCategory.Food);
            var pool = new List<MiniGameDefinition> { food, null };

            var result = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Energy);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Filter_DuplicateElements_ReturnedOnce()
        {
            var food = Make(MiniGameUsage.Energy, MiniGameCategory.Food);
            var pool = new List<MiniGameDefinition> { food, food };

            var result = MiniGamePoolFilter.Filter(pool, MiniGameUsage.Energy);

            Assert.AreEqual(1, result.Count);
        }
    }
}
