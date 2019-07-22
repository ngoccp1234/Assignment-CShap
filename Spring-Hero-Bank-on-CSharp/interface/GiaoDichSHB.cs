using System;
using Spring_Hero_Bank_on_CSharp;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.model;

namespace ConsoleApp1
{
    public class GiaoDichSHB : GiaoDich
    {
        private static SHBAccountModel shbAccountModel;

        public GiaoDichSHB()
        {
            shbAccountModel = new SHBAccountModel();
        }


        public void Withdraw()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Withdraw money at the banking system SHB.");
                Console.WriteLine("Please enter the amount to withdraw: ");
                var amount = double.Parse(Console.ReadLine());
                
                if (amount > Program.currentLoggedInAccount.Balance)
                {
                    Console.WriteLine("Invalid amount, please check again.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    Message = "withdraw money at ATM SHB with money: " + amount,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount,transaction))
                {
                   Console.WriteLine("Successful transaction.");  
                }
//                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
              
            }
            else
            {
                Console.WriteLine("Please login to use this function.");
            }
        }

        public void Deposit()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Deposit money at the banking system SHB.");
                Console.WriteLine("Please enter the amount to deposit:");
                var amount = double.Parse(Console.ReadLine());
                
                if (amount <= 0)
                {
                    Console.WriteLine("Invalid amount, please check again.");
                    return;
                }
                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    Message = "Deposit money at ATM SHB with money: " + amount,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount,transaction))
                {
                    Console.WriteLine("Successful transaction.");  
                }
//                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
              
            }
            else
            {
                Console.WriteLine("Please login to use this function.");
            }
        }
        

        public void Transfer()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.WriteLine("Transfer money at the banking system SHB.");
                Console.WriteLine("Please enter the account number you want to transfer: ");
                var accountNumber = Console.ReadLine();
                var receiverAccount = shbAccountModel.GetAccountByAccountNumber(accountNumber);
                if (receiverAccount == null)
                {
                    Console.WriteLine("Money receiving account does not exist or has been locked, please check again.");
                    return;
                }
                Console.WriteLine("Money receiving account: " + accountNumber);
                Console.WriteLine("account holder: " + receiverAccount.Username);
                Console.WriteLine("Enter the amount you want to transfer: ");
                var amount = double.Parse(Console.ReadLine());
                Program.currentLoggedInAccount = shbAccountModel.GetAccountByUsername(Program.currentLoggedInAccount.Username);
                if (amount > Program.currentLoggedInAccount.Balance)
                {
                    Console.WriteLine("Account balance is not enough to make transactions.");
                    return;
                }
                Console.WriteLine("Enter transaction content: ");
                var message = Console.ReadLine();
                var shbTransaction  = new SHBTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = SHBTransaction.TransactionType.TRANSFER,
                    Amount = amount,    
                    Message = message,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE,
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = accountNumber
                };
                if (shbAccountModel.Transfer(Program.currentLoggedInAccount, shbTransaction))
                {
                    Console.WriteLine("Successful transaction.");
                }
                else
                {
                    Console.WriteLine("The transaction failed, please check again.");
                }
            }
        }

        public void Login()
        {
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Login SHB system.");
            // Yêu cầu nhập username, password.
            Console.WriteLine("Please enter your account name: ");
            var username = Console.ReadLine();
            Console.WriteLine("Please enter your password: ");
            var password = Console.ReadLine();
            // gọi đến model kiểm, nếu model trả về null thì báo đăng nhập sai.
            var shbAccount = shbAccountModel.FindByUsernameAndPassword(username, password);
            if (shbAccount == null)
            {
                Console.WriteLine("Wrong account information or password. Please try again.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến currentLoggedInAccount.
            Program.currentLoggedInAccount = shbAccount;
            
        }
    }
}