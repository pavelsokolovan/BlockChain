using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockchain
{
    public class BlockChain
    {
        public const decimal MinimumTransaction = 0.1M;

        private const int difficulty = 2;
        private readonly List<Block> chain;

        public List<Block> Chain
        {
            get { return chain; }
        }
        public static Dictionary<string, TransactionOutput> Utxos { get; } =
            new Dictionary<string, TransactionOutput>();
        private Block LastBlok => chain.Last();

        public BlockChain()
        {
            chain = new List<Block>()
            {
                new Block()
            };
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = LastBlok.Hash;
            newBlock.MineBlock(difficulty);
            chain.Add(newBlock);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < chain.Count; i++)
            {
                Block currentBlock = chain[i];
                Block previousBlock = chain[i - 1];

                if (!currentBlock.Hash.Equals(currentBlock.CalculateHash()))
                {
                    Console.WriteLine("Current Hashes not equal");
                    return false;
                }

                if (!previousBlock.Hash.Equals(currentBlock.PreviousHash))
                {
                    Console.WriteLine("Previous Hashes not equal");
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            foreach (Block block in chain)
            {
                output.Append(
                    value: block.Hash +
                    " >>   previous: " + block.PreviousHash + "\n"
                );
            }

            return output.ToString();
        }
    }

}
