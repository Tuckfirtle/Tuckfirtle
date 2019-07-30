using Tuckfirtle.Core.Pow;
using Xunit;

namespace Tuckfirtle.Core.Test.Pow
{
    public class TuckfirtlePowTest
    {
        [Theory]
        [InlineData("", new byte[] { 247, 113, 27, 246, 94, 59, 76, 221, 1, 48, 189, 181, 84, 164, 165, 185, 244, 34, 237, 187, 64, 116, 220, 176, 9, 250, 241, 26, 64, 39, 22, 33 })]
        public void GetPowValueTest(string jsonData, byte[] expectedResult)
        {
            var powValue = TuckfirtlePow.GetPowValue(jsonData);
            Assert.Equal(powValue, expectedResult);
        }
    }
}