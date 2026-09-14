using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LumenRush.Tests
{
    public sealed class RunnerPlayModeTests
    {
        GameManager game;
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            game = Object.FindFirstObjectByType<GameManager>();
            if (game == null)
                game = new GameObject("Test session").AddComponent<GameManager>();
            game.StartRun();
            yield return null;
        }

        [UnityTest]
        public IEnumerator LaneJumpAndSlideRespond()
        {
            game.Player.MoveLane(1);
            yield return new WaitForSeconds(.3f);
            Assert.Greater(game.Player.transform.position.x, 2.5f);
            game.Player.Jump();
            yield return new WaitForSeconds(.2f);
            Assert.Greater(game.Player.Height, 1);
            game.Player.Slide();
            yield return new WaitForSeconds(.3f);
            Assert.IsTrue(game.Player.Sliding);
        }

        [UnityTest]
        public IEnumerator PauseFreezesRun()
        {
            yield return new WaitForSeconds(.1f);
            game.TogglePause();
            float distance = game.Distance;
            yield return new WaitForSecondsRealtime(.15f);
            Assert.AreEqual(distance, game.Distance);
            game.TogglePause();
            yield return null;
            Assert.Greater(game.Distance, distance);
        }

        [UnityTest]
        public IEnumerator FatalCollisionReviveAndRestart()
        {
            yield return new WaitForSeconds(2.2f);
            var obstacle = new GameObject("Test cargo").AddComponent<TrackItem>();
            obstacle.Kind = ItemKind.Cargo;
            obstacle.PreviousZ = 1;
            obstacle.transform.position = Vector3.zero;
            obstacle.Check(game);
            Assert.AreEqual(RunState.GameOver, game.State);
            game.Revive();
            Assert.AreEqual(RunState.Running, game.State);
            Assert.IsTrue(game.Revived);
            game.Retry();
            Assert.AreEqual(0, game.Score);
            Assert.AreEqual(0, game.Coins);
            Assert.IsFalse(game.Revived);
            Object.Destroy(obstacle.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShieldAndMagnetHaveDuration()
        {
            game.PowerUps.Activate(PowerKind.Shield);
            Assert.IsTrue(game.PowerUps.Absorb());
            Assert.IsFalse(game.PowerUps.Active(PowerKind.Shield));
            game.PowerUps.Activate(PowerKind.Magnet);
            game.PowerUps.Tick(100);
            Assert.IsFalse(game.PowerUps.Active(PowerKind.Magnet));
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            game.Home();
            Time.timeScale = 1;
            yield return null;
        }
    }
}
