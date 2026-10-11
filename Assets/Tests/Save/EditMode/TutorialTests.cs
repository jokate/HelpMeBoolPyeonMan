using System;
using System.IO;
using NUnit.Framework;
using BoolPyeon.Save;

namespace BoolPyeon.Save.Tests
{
    // F-23: 새 저장은 tutorialDone=false 로 시작하고, CompleteTutorial 뒤에는 새 인스턴스로 다시 읽어도 true 로 남는다.
    public class TutorialTests
    {
        private string dir;

        [SetUp]
        public void SetUp()
        {
            dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            try { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
            catch { /* best effort */ }
        }

        [Test]
        public void NewSave_TutorialNotDone()
        {
            var svc = new SaveService(dir);
            Assert.IsFalse(svc.Data.tutorialDone);
        }

        [Test]
        public void CompleteTutorial_PersistsAcrossInstances()
        {
            var svc = new SaveService(dir);
            Assert.IsFalse(svc.Data.tutorialDone);
            svc.CompleteTutorial();

            var reloaded = new SaveService(dir).Load();
            Assert.IsTrue(reloaded.tutorialDone);
        }

        [Test]
        public void LegacySave_WithoutTutorialFlag_IsMarkedDone()
        {
            var svc = new SaveService(dir);
            svc.Data.runsStarted = 1;
            svc.Save(svc.Data);

            var reloaded = new SaveService(dir).Load();
            Assert.IsTrue(reloaded.tutorialDone);
        }
    }
}
