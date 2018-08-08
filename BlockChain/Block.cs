using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyEncryption;

namespace Blockchain
{
    public class Block
    {
        private readonly DateTime createdOn;
        private int nonce;

        public string Hash { get; private set; }
        public string PreviousHash { get; set; }
        public List<Transaction> Transactions { get; private set; }
        public string TransactionsHash { get; private set; }

        public Block()
        {
            createdOn = DateTime.Now;
            Hash = CalculateHash();
            Transactions = new List<Transaction>();
        }

        public string CalculateHash()
        {
            return SHA.ComputeSHA256Hash(PreviousHash + TransactionsHash + createdOn + nonce.ToString());
        }

        public void MineBlock(int difficulty)
        {
            string target = "".PadRight(difficulty, '0');
            TransactionsHash = Transaction.CalculateHash(Transactions);

            while (!Hash.Substring(0, difficulty).Equals(target))
            {
                nonce++;
                Hash = CalculateHash();
            }

            Console.WriteLine("Block Mined!!! : " + Hash);
        }

        //Add transactions to this block
        public bool AddTransaction(Transaction transaction)
        {
            //process transaction and check if valid, unless block is genesis block then ignore.
            if (transaction == null)
            {
                return false;
            }

            if (PreviousHash != null)
            {
                if (transaction.ProcessTransaction() != true)
                {
                    Console.WriteLine("Transaction failed to process. Discarded.");
                    return false;
                }
            }
            Transactions.Add(transaction);
            Console.WriteLine("Transaction Successfully added to Block");
            return true;
        }
    }
}
