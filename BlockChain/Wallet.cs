using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Blockchain
{
    public class Wallet
    {
        private Dictionary<string, TransactionOutput> utxos;

        public User Owner { get; private set; }
        public Key PrivateKey { get; private set; }
        public Key PublicKey { get; private set; }

        public Wallet(User owner)
        {
            Owner = owner;
            GenerateKeyPair();
            utxos = new Dictionary<string, TransactionOutput>();
        }

        private void GenerateKeyPair()
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(); 
            
            PublicKey = new Key(rsaProvider.ExportParameters(false), Owner);
            PrivateKey = new Key(rsaProvider.ExportParameters(true), Owner);
        }

        //returns balance and stores the UTXO's owned by this wallet in this.UTXOs
        public decimal Balance
        {
            get
            {
                decimal total = 0;
                foreach (var item in BlockChain.Utxos)
                {
                    TransactionOutput utxo = item.Value;
                    if (utxo.IsMine(PublicKey))
                    {
                        total += utxo.Value;
                    }
                }
                return total;
            }
        }

        public void UpdateUtxos()
        {
            foreach (var item in BlockChain.Utxos)
            {
                TransactionOutput utxo = item.Value;
                if (utxo.IsMine(PublicKey))
                {
                    utxos.Add(utxo.Id, utxo);
                }
            }
        }

        //Generates and returns a new transaction from this wallet.
        public Transaction SendFunds(Key recipient, decimal value)
        {
            if (Balance < value)
            { //gather balance and check funds.
                Console.WriteLine("#Not Enough funds to send transaction. Transaction Discarded.");
                return null;
            }

            UpdateUtxos();

            List<TransactionInput> inputs = new List<TransactionInput>();
            decimal total = 0;
            foreach (var item in utxos)
            {
                TransactionOutput utxo = item.Value;
                total += utxo.Value;
                inputs.Add(new TransactionInput(utxo.Id));
                if (total > value) break;
            }

            Transaction newTransaction = Transaction.GetTransaction(PublicKey, recipient, value, inputs);
            newTransaction.GenerateSignature(PrivateKey);

            foreach (TransactionInput input in inputs)
            {
                utxos.Remove(input.TransactionOutputId);
            }

            return newTransaction;
        }
    }
}
