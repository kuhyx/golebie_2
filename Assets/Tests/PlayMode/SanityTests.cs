using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace PigeonRacingManager.Tests.PlayMode
{
    [TestFixture]
    public class SanityTests
    {
        [UnityTest]
        public IEnumerator PlaceholderTest_Passes()
        {
            Assert.That(true, Is.True);
            yield return null;
        }
    }
}
