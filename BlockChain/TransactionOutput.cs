using System.Security.Cryptography;
using EasyEncryption;

namespace Blockchain
{
    public class TransactionOutput
    {
        public string Id { get; }
        public Key Reciepient { get; }
        public decimal Value { get; }
        public string ParentTransactionId { get; }  //  TODO rename to TransactionId

        public TransactionOutput(Key reciepient, decimal value, string parentTransactionId)
        {
            Reciepient = reciepient;
            Value = value;
            ParentTransactionId = parentTransactionId;
            Id = SHA.ComputeSHA256Hash(reciepient.ToString() + Value + parentTransactionId);
        }

        public bool IsMine(Key publicKey)
        {
            return publicKey.Equals(Reciepient);    //TODO check for comparation
        }
    }
}