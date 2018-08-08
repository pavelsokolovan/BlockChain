namespace Blockchain
{
    public class TransactionInput
    {
        public string TransactionOutputId { get; private set; }
        public TransactionOutput Utxo { get; private set; }

        public TransactionInput(string transactionOutputId)
        {
            this.TransactionOutputId = transactionOutputId;
        }

        public void UpdateTransactionOutput()
        {
            Utxo = BlockChain.Utxos[TransactionOutputId];
        }
    }
}
