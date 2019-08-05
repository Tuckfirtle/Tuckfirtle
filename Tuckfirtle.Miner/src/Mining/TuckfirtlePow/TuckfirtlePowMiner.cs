// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using Tuckfirtle.Miner.Threading;

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
            MiningThread = new Thread(InternalStartMining) { ProcessorAffinity = minerInformation.ThreadAffinity, IsBackground = true };
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