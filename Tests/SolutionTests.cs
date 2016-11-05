using System;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Solutions;

namespace Tests
{
    [TestClass]
    public class SolutionTests
    {
        [TestMethod]
        public void MultiplySolutionSerialize()
        {
            var solution = new MultiplySolutionData(69);
            var serialized = solution.Serialize();
            var deserialized = MultiplySolutionData.Deserialize(serialized);

            Assert.AreEqual(solution.result, deserialized.result);
        }
    }
}
