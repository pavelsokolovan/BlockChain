using System;
using System.Threading.Tasks;
using Blockchain;
using P2P;

namespace Test
{
    class Program
    {
        static void Main()
        {
            using (var messanger = new Messager())
            {
                messanger.FindPeers();
                messanger.SendMessageToPeer("hello");
                Console.ReadKey();
                //Console.WriteLine("Enter several messages:");
                //while(true)
                //{
                //    messanger.SendMessageToPeer(Console.ReadLine());
                //}
            }


            //Wallet walletA = new Wallet(new User("walletA"));
            //Wallet walletB = new Wallet(new User("walletB"));
            //Wallet walletCoinsBase = new Wallet(new User("walletCoinsBase"));
            //BlockChain blockChain = new BlockChain();

            //Transaction genesisTransaction = Transaction.GetTransaction(walletCoinsBase.PublicKey,
            //                                                            walletA.PublicKey,
            //                                                            100,
            //                                                            null);
            //genesisTransaction.GenerateSignature(walletCoinsBase.PrivateKey);
            //genesisTransaction.Outputs.Add(
            //    new TransactionOutput(genesisTransaction.ReciepientPublicKey,
            //                        genesisTransaction.Value,
            //                        genesisTransaction.TransactionId)
            //);

            //BlockChain.Utxos.Add(genesisTransaction.Outputs[0].Id,
            //                    genesisTransaction.Outputs[0]);

            //Console.WriteLine("Creating and Mining Genesis block... ");
            //Block genesisBlock = blockChain.Chain[0];
            //genesisBlock.AddTransaction(genesisTransaction);

            ////testing
            //Block block1 = new Block();
            //block1.PreviousHash = genesisBlock.Hash;
            //Console.WriteLine("\nWalletA's balance is: " + walletA.Balance);
            //Console.WriteLine("\nWalletA is Attempting to send funds (40) to WalletB...");
            //block1.AddTransaction(walletA.SendFunds(walletB.PublicKey, 40));
            //blockChain.AddBlock(block1);
            //Console.WriteLine("\nWalletA's balance is: " + walletA.Balance);
            //Console.WriteLine("WalletB's balance is: " + walletB.Balance);

            //Block block2 = new Block();
            //block2.PreviousHash = block1.Hash;
            //Console.WriteLine("\nWalletA Attempting to send more funds (1000) than it has...");
            //block2.AddTransaction(walletA.SendFunds(walletB.PublicKey, 1000));
            //blockChain.AddBlock(block2);
            //Console.WriteLine("\nWalletA's balance is: " + walletA.Balance);
            //Console.WriteLine("WalletB's balance is: " + walletB.Balance);

            //Block block3 = new Block();
            //block3.PreviousHash = block2.Hash;
            //Console.WriteLine("\nWalletB is Attempting to send funds (20) to WalletA...");
            //block3.AddTransaction(walletB.SendFunds(walletA.PublicKey, 20));
            //Console.WriteLine("\nWalletA's balance is: " + walletA.Balance);
            //Console.WriteLine("WalletB's balance is: " + walletB.Balance);

            //Console.ReadKey();
        }

        private static void OnFindPeersProcessFinished(object sender, EventArgs e)
        {
            Messager messanger = sender as Messager;
            Console.Write("Enter message: ");
            string message = Console.ReadLine();
            messanger.SendMessageToPeer(message);
        }
    }
}
