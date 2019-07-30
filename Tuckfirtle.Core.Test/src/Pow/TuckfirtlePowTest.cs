using Tuckfirtle.Core.Pow;
using Xunit;

namespace Tuckfirtle.Core.Test.Pow
{
    public class TuckfirtlePowTest : IClassFixture<TuckfirtlePow>
    {
        private TuckfirtlePow TuckfirtlePow { get; }

        public TuckfirtlePowTest(TuckfirtlePow tuckfirtlePow)
        {
            TuckfirtlePow = tuckfirtlePow;
        }

        [Theory]
        [InlineData("", new byte[] { 227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85, 56, 176, 96, 167, 81, 172, 150, 56, 76, 217, 50, 126, 177, 177, 227, 106, 33, 253, 183, 17, 20, 190, 7, 67, 76, 12, 199, 191, 99, 246, 225, 218, 39, 78, 222, 191, 231, 111, 101, 251, 213, 26, 210, 241, 72, 152, 185, 91, 207, 131, 225, 53, 126, 239, 184, 189, 241, 84, 40, 80, 214, 109, 128, 7, 214, 32, 228, 5, 11, 87, 21, 220, 131, 244, 169, 33, 211, 108, 233, 206, 71, 208, 209, 60, 93, 133, 242, 176, 255, 131, 24, 210, 135, 126, 236, 47, 99, 185, 49, 189, 71, 65, 122, 129, 165, 56, 50, 122, 249, 39, 218, 62 })]
        public void GetPowDataTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            Assert.Equal(powData, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 247, 113, 27, 246, 94, 59, 76, 221, 1, 48, 189, 181, 84, 164, 165, 185, 244, 34, 237, 187, 64, 116, 220, 176, 9, 250, 241, 26, 64, 39, 22, 33 })]
        public void GetPowValueTest(string jsonData, byte[] expectedResult)
        {
            var powValue = TuckfirtlePow.GetPowValue(jsonData);
            Assert.Equal(powValue, expectedResult);
        }
    }
}