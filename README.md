# Tuckfirtle
This is the main tuckfirtle cryptocurrency repository.

#### Table of contents:
- [About](#About)
- [Basic Information](#Basic-Information)
- [Donations](#Donations)
- [Developers](#Developers)

## About
Tuckfirtle is an open source cryptocurrency coin written from scratch in C#.
It is branded as an educational project as it is designed to be simple for any developer to follow and learn together.
The aim of this project is to equip new entry level developers into cryptocurrency and make an original coin together.
As a result, we also build a healthy relationship with the community around us.

The reason not to fork an existing coin and use their code as base is due to complexity and the dependencies needed to build a coin is extremely ridiculous.
Forking existing coin tend to not do well in long run and thus the reason to write from scratch. I find that forking a coin is like copy and paste and never had the uniqueness to it.
We need more original coins!

This coin will aim to be fully decentralized with variable block time and supply to create incentive to mine.
Transaction may not be instant but it can happen in less than a minute to few minutes long with variable block time.
In terms of privacy, it does not equip with CN technology with mixins.
This may mean that transactions could be tracable by wallet address but we will iron out this out when we have ideas to solve them.

The POW algorithm is brand new and written to look similar to [CryptoNight](https://cryptonote.org/cns/cns008.txt). You can refer to the links below to have an idea how it works.
It is not proven to be asic resistance but the POW can be changed to solve that.

More information about this coin will be written when we have iron out the idea.

## Basic Information
- Coin Name: Tuckfirtle (TF)
- Github: https://github.com/Tuckfirtle/Tuckfirtle
- Discord: http://discord.gg/eEXTB3T
- Pre-ANN: https://bitcointalk.org/index.php?topic=5172164
- Currency Mining Type: Proof of work (POW)
- POW Mining Algorithm: [TuckfirtlePow](https://github.com/Tuckfirtle/Tuckfirtle.Core/blob/master/src/Pow/TuckfirtlePow.cs)
- Block Time: Variable based on number of miners and their corresponding mining speed. The difficulty will drop every minute if it is not found.
- Block Reward: Capped at 1 TF/Minute. It will scale down if you found the block too early.
- Total Supply: TBD
- Premine: 0%
- CPU/ARM Minable: Yes
- GPU/FPGA Minable: Possible but it may not be as efficient as CPU with the memory hard loop. (This is not proven to be true as of now.)
- ASIC Minable: Could be possible but the efficiency should not be as fast as CPU with the memory hard loop. (This is not proven to be true as of now.)
- Pool Mining: Yes

## Donations
- BTC: `3Dc5jpiyuts136YhamcRbAeue7mi44gW8d`
- LTC: `LUU9Avuanafmq1vMp53AWS1mr3GCCc2X42`
- XMR: `42oj7eV68BK8Z8wcGzLMFEJgAQG22Z3ajGdtpmJx5p7iDqEgG91wNybWbwaVe4vUMveKAzAiA4j8xgUi29TpKXpm3zwfwWN`

## Developers
- Yong Jian Ming