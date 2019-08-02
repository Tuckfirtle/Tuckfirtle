using System;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsThread = Tuckfirtle.Miner.Threading.Windows.Thread;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal sealed class TuckfirtlePowMiner
    {
        private Thread MiningThread { get; }

        private bool IsRunning { get; set; }

        private TuckfirtlePowMinerInformation MinerInformation { get; }

        private TuckfirtlePowInformation PowInformation { get; }

        private Action<TuckfirtlePowSubmitInformation> SubmitAction { get; }

        public TuckfirtlePowMiner(TuckfirtlePowMinerInformation minerInformation, TuckfirtlePowInformation powInformation, Action<TuckfirtlePowSubmitInformation> submitAction)
        {
            MinerInformation = minerInformation;
            PowInformation = powInformation;
            SubmitAction = submitAction;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsThread = new WindowsThread(InternalStartMining) { ProcessorAffinity = minerInformation.ThreadAffinity };
                windowsThread.ManagedThread.IsBackground = true;

                MiningThread = windowsThread.ManagedThread;
            }
            else
                MiningThread = new Thread(InternalStartMining) { IsBackground = true };
        }

        public void StartMining()
        {
            MiningThread.Start();
            IsRunning = true;
        }

        public void StopMining()
        {
            IsRunning = false;
            MiningThread.Join();
        }

        private void InternalStartMining()
        {
            var powInformation = PowInformation;
            var currentNonce = MinerInformation.StartingNonce;

            while (IsRunning)
            {
                var blockHeaderTemplate = powInformation.BlockHeaderTemplate;
                var powValue = Core.Pow.TuckfirtlePow.GetPowValueUnsafe($"{blockHeaderTemplate}{currentNonce++}");

                if (powValue >= powInformation.TargetPowValue)
                    continue;

                // Found a block!
                SubmitAction(new TuckfirtlePowSubmitInformation { Height = powInformation.Height, BlockHeaderTemplate = blockHeaderTemplate, Nonce = currentNonce - 1, PowValue = powValue });
                currentNonce = MinerInformation.StartingNonce;
            }
        }
    }
}