using System.Numerics;
using System.Threading.Tasks;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core.Pow;

namespace Tuckfirtle.Node
{
    public class Test
    {
        private IConsoleLogger ConsoleLogger { get; }

        public Test(IConsoleLogger consoleLogger)
        {
            ConsoleLogger = consoleLogger;
        }

        public void Start()
        {
            var pow = new TuckfirtlePow();

            var biggestNumber = new byte[97];
            
            for (var i = 0; i < biggestNumber.Length; i++)
            {
                if (i != 96)
                    biggestNumber[i] = byte.MaxValue;
                else
                    biggestNumber[i] = 0;
            }

            var bigNumber = new BigInteger(biggestNumber);
            var smallestNumber = bigNumber;

            for (var i = 0; i < 10000000; i++)
            {
                var powValue = pow.GetPowValue(i.ToString());
                var powNumber = new byte[powValue.Length + 1];
                powNumber[powValue.Length] = 0;
                powValue.CopyTo(powNumber, 0);

                var bigInteger = new BigInteger(powNumber);

                if (bigInteger >= smallestNumber)
                    continue;

                smallestNumber = bigInteger;
                ConsoleLogger.LogMessage($"NOnce {i}: {bigInteger.ToString().PadLeft(bigNumber.ToString().Length, '0')}");
            }
        }
    }
}