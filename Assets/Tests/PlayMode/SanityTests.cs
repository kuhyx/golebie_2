using System.Collections;
using NUnit.Framework;
using PigeonRacingManager.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace PigeonRacingManager.Tests.PlayMode
{
    [TestFixture]
    public class GameManagerPlayTests
    {
        private GameObject _gameManagerGo;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (GameManager.Instance != null)
            {
                Object.Destroy(GameManager.Instance.gameObject);
                yield return null;
            }

            _gameManagerGo = new GameObject("TestGameManager");
            _gameManagerGo.AddComponent<GameManager>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_gameManagerGo != null)
            {
                Object.Destroy(_gameManagerGo);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Singleton_IsSet_AfterAwake()
        {
            yield return null;
            Assert.That(GameManager.Instance, Is.Not.Null);
            Assert.That(GameManager.Instance.gameObject, Is.EqualTo(_gameManagerGo));
        }

        [UnityTest]
        public IEnumerator Singleton_Duplicate_IsDestroyed()
        {
            yield return null;
            GameObject duplicate = new GameObject("DuplicateGM");
            duplicate.AddComponent<GameManager>();
            yield return null;

            Assert.That(GameManager.Instance.gameObject, Is.EqualTo(_gameManagerGo));
            yield return null;
            Assert.That(duplicate == null, Is.True);
        }

        [UnityTest]
        public IEnumerator StartNewGame_CreatesState()
        {
            yield return null;
            GameManager.Instance.StartNewGame("TestPlayer");
            Assert.That(GameManager.Instance.State, Is.Not.Null);
            Assert.That(GameManager.Instance.State.Player.PlayerName, Is.EqualTo("TestPlayer"));
        }

        [UnityTest]
        public IEnumerator StartNewGame_EmptyName_UsesDefault()
        {
            yield return null;
            GameManager.Instance.StartNewGame("");
            Assert.That(GameManager.Instance.State, Is.Not.Null);
            Assert.That(GameManager.Instance.State.Player.PlayerName, Is.EqualTo("Player"));
        }

        [UnityTest]
        public IEnumerator AdvanceWeek_FiresEvent()
        {
            yield return null;
            GameManager.Instance.StartNewGame("Tester");

            bool eventFired = false;
            GameManager.Instance.OnWeekAdvanced += () => eventFired = true;
            GameManager.Instance.AdvanceWeek();

            Assert.That(eventFired, Is.True);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AdvanceWeek_NoState_DoesNothing()
        {
            yield return null;
            Assert.DoesNotThrow(() => GameManager.Instance.AdvanceWeek());
            yield return null;
        }

        [UnityTest]
        public IEnumerator NotifyMoneyChanged_FiresEvent()
        {
            yield return null;
            bool eventFired = false;
            GameManager.Instance.OnMoneyChanged += () => eventFired = true;
            GameManager.Instance.NotifyMoneyChanged();

            Assert.That(eventFired, Is.True);
            yield return null;
        }
    }

    [TestFixture]
    public class ScreenManagerPlayTests
    {
        private GameObject _managerGo;
        private GameObject _screenA;
        private GameObject _screenB;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (ScreenManager.Instance != null)
            {
                Object.Destroy(ScreenManager.Instance.gameObject);
                yield return null;
            }

            _screenA = new GameObject("ScreenA");
            _screenB = new GameObject("ScreenB");

            _managerGo = new GameObject("TestScreenManager");
            _managerGo.AddComponent<ScreenManager>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_managerGo != null)
            {
                Object.Destroy(_managerGo);
            }

            if (_screenA != null)
            {
                Object.Destroy(_screenA);
            }

            if (_screenB != null)
            {
                Object.Destroy(_screenB);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Singleton_IsSet_AfterAwake()
        {
            yield return null;
            Assert.That(ScreenManager.Instance, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator OnScreenChanged_EventFires()
        {
            yield return null;
            ScreenType receivedType = ScreenType.MainMenu;
            bool fired = false;
            ScreenManager.Instance.OnScreenChanged += (t) =>
            {
                receivedType = t;
                fired = true;
            };

            ScreenManager.Instance.ShowScreen(ScreenType.Loft);

            Assert.That(fired, Is.True);
            Assert.That(receivedType, Is.EqualTo(ScreenType.Loft));
            Assert.That(ScreenManager.Instance.CurrentScreen, Is.EqualTo(ScreenType.Loft));
            yield return null;
        }
    }
}
