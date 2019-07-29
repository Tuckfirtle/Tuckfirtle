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
        [InlineData("", new byte[] { 219, 0, 164, 229, 201, 80, 138, 44, 214, 34, 198, 182, 40, 222, 90, 78, 31, 30, 33, 67, 53, 55, 5, 116, 232, 76, 171, 101, 201, 227, 91, 63 })]
        public void GetAesKeyTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var aesKey = TuckfirtlePow.GetAesKey(powData);
            Assert.Equal(aesKey, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 238, 126, 86, 36, 106, 81, 191, 254, 189, 88, 239, 239, 181, 155, 97, 221, 232, 205, 63, 138, 153, 128, 221, 70, 36, 78, 250, 161, 158, 245, 57, 92 })]
        public void GetXorKeyTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var xorKey = TuckfirtlePow.GetXorKey(powData);
            Assert.Equal(xorKey, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 181, 153, 213, 184, 76, 22, 111, 93, 38, 204, 155, 91, 42, 75, 51, 240, 36, 105, 224, 129, 26, 196, 136, 49, 90, 187, 42, 168, 126, 89, 54, 17 })]
        public void GetAesDataTest(string jsonData, byte[] expectedResult)
        {
            var powData = TuckfirtlePow.GetPowData(jsonData);
            var aesData = TuckfirtlePow.GetAesData(powData);
            Assert.Equal(aesData, expectedResult);
        }

        [Theory]
        [InlineData("", new byte[] { 105, 58, 78, 200, 18, 118, 150, 158, 16, 113, 126, 66, 19, 229, 51, 174, 44, 165, 74, 239, 111, 144, 152, 71, 175, 158, 146, 16, 115, 89, 179, 94, 249, 113, 161, 102, 144, 109, 87, 249, 141, 24, 243, 191, 112, 112, 34, 171, 125, 161, 235, 77, 72, 226, 91, 31, 16, 80, 155, 227, 63, 170, 189, 134, 54, 95, 207, 174, 246, 126, 116, 234, 196, 11, 195, 224, 89, 137, 168, 74, 148, 216, 186, 110, 37, 180, 227, 230, 170, 15, 115, 11, 141, 54, 219, 92, 101, 147, 87, 182, 184, 228, 166, 111, 48, 71, 26, 146, 96, 223, 90, 125, 61, 170, 171, 70, 39, 255, 136, 202, 133, 249, 98, 168, 253, 4, 150, 85, 65, 155, 19, 159, 101, 99, 88, 163, 135, 26, 16, 88, 219, 5, 248, 28 })]
        public void GetPowValueTest(string jsonData, byte[] expectedResult)
        {
            var powValue = TuckfirtlePow.GetPowValue(jsonData);
            Assert.Equal(powValue, expectedResult);
        }
    }
}