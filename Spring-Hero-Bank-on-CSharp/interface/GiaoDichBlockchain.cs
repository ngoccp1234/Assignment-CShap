using System;
using Spring_Hero_Bank_on_CSharp;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.model;

namespace ConsoleApp1
{
    public class GiaoDichBlockchain : GiaoDich
    {
        private static BlockchainAddressModel blockchainAddressModel;
        
        public GiaoDichBlockchain()
        {
            blockchainAddressModel = new BlockchainAddressModel();
        }
        public void Withdraw()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Withdraw money at blockchain e-wallet.");
                Console.WriteLine("Please enter the amount to withdraw: ");
                var amount = double.Parse(Console.ReadLine());
                if (amount > Program.currentLoggedInAddress.Balance)
                {
                    Console.WriteLine("Invalid amount, please check again!");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    Message = "Withdraw money at blockchain e-wallet with money: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    Console.WriteLine("Successful transaction.");  
                }
            }
            else
            {
                Console.WriteLine("Please login to use this function.");
            }
        }

        public void Deposit()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Deposit at Blockchain e-wallet.");
                Console.WriteLine("Please enter the amount to deposit: ");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Invalid amount, please check again.");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    Message = "Deposit at Blockchain e-wallet with amount: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    
                    Console.WriteLine("Successful transaction.");  
                }
            }
            else
            {
                Console.WriteLine("Please login to use this function.");
            }
        }

        public void Transfer()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.WriteLine("Transfer at Blockchain e-wallet.");
                Console.WriteLine("Please enter the address you want to transfer: ");
                var address = Console.ReadLine();
                var receiverAddress = blockchainAddressModel.GetAddress(address);
                if (receiverAddress == null)
                {
                    Console.WriteLine("Money receiving account does not exist or has been locked.");
                    return;
                }
                Console.WriteLine("Address to receive money: " + address);
                    Console.WriteLine("Enter the amount you want to transfer: ");
                var amount = double.Parse(Console.ReadLine());
                Program.currentLoggedInAddress = blockchainAddressModel.GetAddress(Program.currentLoggedInAddress.Address);
                if (amount > Program.currentLoggedInAddress.Balance)
                {
                    Console.WriteLine("Account balance is not enough to make transactions.");
                    return;
                }
                Console.WriteLine("Enter transaction content: ");
                var message = Console.ReadLine();
                var blockchainTransaction  = new BlockchainTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = BlockchainTransaction.TransactionType.TRANSFER,
                    Amount = amount,    
                    Message = message,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE,
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = address
                };
                if (blockchainAddressModel.Transfer(Program.currentLoggedInAddress, blockchainTransaction))
                {
                    Console.WriteLine("Successful transaction!");
                }
                else
                {
                    Console.WriteLine("The transaction failed, please check again.");
                }
            }
        }

        public void Login()
        {
            Program.currentLoggedInAddress = null;
            Console.Clear();
            Console.WriteLine("Login of Blockchain e-wallet system.");
            Console.WriteLine("Please enter the login address: ");
            var address = Console.ReadLine();
            Console.WriteLine("Please enter the private key: ");
            var privateKey = Console.ReadLine();
            var blockchainAddress = blockchainAddressModel.FindByAddressAndPrivateKey(address, privateKey);
            if (blockchainAddress == null)
            {
                Console.WriteLine("Incorrect account address, please check and login again.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến CurrentLoggedInAddress.
            Program.currentLoggedInAddress = blockchainAddress;

        }
    }
}