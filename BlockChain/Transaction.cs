using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using EasyEncryption;

namespace Blockchain
{
    public class Transaction
    {
        private static int sequence = 0;

        public String TransactionId { get; private set; }
        public Key SenderPublicKey { get; private set; }
        public Key ReciepientPublicKey { get; private set; }
        public decimal Value { get; private set; }
        public byte[] Signature { get; private set; }
        public List<TransactionInput> Inputs { get; private set; }
        public List<TransactionOutput> Outputs { get; private set; }

        private byte[] VerificationData
        {
            get
            {
                var a = new List<byte>(SenderPublicKey.Value.Modulus);
                a.AddRange(ReciepientPublicKey.Value.Modulus);
                a.AddRange(Encoding.ASCII.GetBytes(Value.ToString()));
                return a.ToArray();
            }
        }

        //returns sum of inputs(UTXOs) values
        private decimal InputsValue
        {
            get
            {
                decimal total = 0;
                foreach (TransactionInput input in Inputs)
                {
                    if (input.Utxo == null) continue;
                    total += input.Utxo.Value;
                }
                return total;
            }
        }

        //returns sum of outputs:
        public decimal OutputsValue
        {
            get
            {
                decimal total = 0;
                foreach (TransactionOutput output in Outputs)
                {
                    total += output.Value;
                }
                return total;
            }

        }

        private Transaction(Key fromPublicKey,
                            Key toPublicKey,
                            decimal value,
                            List<TransactionInput> inputs)
        {
            TransactionId = "0";
            SenderPublicKey = fromPublicKey;
            ReciepientPublicKey = toPublicKey;
            Value = value;
            Inputs = inputs;
            Outputs = new List<TransactionOutput>();
        }


        public static Transaction GetTransaction(Key fromPublicKey,
                                                Key toPublicKey,
                                                decimal value,
                                                List<TransactionInput> inputs)
        {
            sequence++;
            return new Transaction(fromPublicKey, toPublicKey, value, inputs);
        }

        private String CalulateHash() // TODO replace to GetHashCode()
        {
            return SHA.ComputeSHA256Hash
            (
                SenderPublicKey.ToString() + ReciepientPublicKey.ToString() + Value
            );
        }


        public void GenerateSignature(Key privateKey)
        {
            Signature = HashAndSignBytes(VerificationData, privateKey);
        }

        private byte[] HashAndSignBytes(byte[] dataToSign, Key key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(key.Value);

                return RSAalg.SignData(dataToSign, new SHA256CryptoServiceProvider());
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        private bool IsSignatureValid()
        {
            return VerifySignedHash(VerificationData, Signature, SenderPublicKey);
        }

        private bool VerifySignedHash(byte[] dataToVerify, byte[] signedData, Key key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(key.Value);

                return RSAalg.VerifyData(dataToVerify, new SHA256CryptoServiceProvider(), signedData);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }


        public bool ProcessTransaction()
        {
            if (!IsSignatureValid())
            {
                Console.WriteLine("#Transaction Signature failed to verify");
                return false;
            }

            //gather transaction inputs (Make sure they are unspent):
            foreach (TransactionInput input in Inputs)
            {
                input.UpdateTransactionOutput();
            }

            decimal inputsValue = InputsValue;

            //check if transaction is valid:
            if (inputsValue < BlockChain.MinimumTransaction)
            {
                Console.WriteLine
                (
                    "#Transaction Input is: " + inputsValue
                    + ". Minimum transaction is " + BlockChain.MinimumTransaction
                );
                return false;
            }

            //generate transaction outputs:
            decimal leftOver = inputsValue - Value;
            TransactionId = CalulateHash();
            Outputs.Add(new TransactionOutput(ReciepientPublicKey, Value, TransactionId)); //send value to recipient
            Outputs.Add(new TransactionOutput(SenderPublicKey, leftOver, TransactionId)); //send the left over 'change' back to sender		

            //add outputs to Unspent list
            foreach (TransactionOutput output in Outputs)
            {
                BlockChain.Utxos.Add(output.Id, output);
            }

            //remove transaction inputs from UTXO lists as spent:
            foreach (TransactionInput input in Inputs)
            {
                if (input.Utxo == null) continue; //if Transaction can't be found skip it 
                BlockChain.Utxos.Remove(input.Utxo.Id);
            }

            return true;
        }


        public static string CalculateHash(ICollection<Transaction> transactions)
        {
            int count = transactions.Count;
            List<string> previousTreeLayer = new List<string>();

            foreach (Transaction transaction in transactions)
            {
                previousTreeLayer.Add(transaction.TransactionId);
            }

            List<string> treeLayer = previousTreeLayer;
            while (count > 1)
            {
                treeLayer = new List<string>();
                for (int i = 1; i < previousTreeLayer.Count; i++)
                {
                    treeLayer.Add(SHA.ComputeSHA256Hash
                    (
                        previousTreeLayer[i - 1] + previousTreeLayer[i]
                    ));
                }
                count = treeLayer.Count;
                previousTreeLayer = treeLayer;
            }

            string merkleRoot = treeLayer.Count == 1 ? treeLayer[0] : "";

            return merkleRoot;
        }

    }

}
