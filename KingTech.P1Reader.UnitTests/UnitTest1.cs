using System.Text.RegularExpressions;

namespace KingTech.P1Reader.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //assign
            var modbusClientKey = "0-1:24.2.1";
            
            //act
            var match = new Regex("0-(.*):24\\.2\\.1").Match(modbusClientKey);

            //assert
            Assert.IsTrue(match.Success);
        }
    }
}