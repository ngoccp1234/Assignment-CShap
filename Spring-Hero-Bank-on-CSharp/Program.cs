using System;
using ConsoleApp1;
using Spring_Hero_Bank_on_CSharp.entity;

namespace Spring_Hero_Bank_on_CSharp
{
    class Program
    {
        public static SHBAccount currentLoggedInAccount;
        public static BlockchainAddress currentLoggedInAddress;

        static void Main(string[] args)
        {

            while (true)
            {
                Console.Clear();
                GiaoDich giaoDich = null;
                Console.WriteLine("Please select one of the following trading types.");
                Console.WriteLine("========================================================");
                Console.WriteLine("1. Transaction by Spring Hero Bank.");
                Console.WriteLine("2. Transaction by Blockchain e-wallet.");
                Console.WriteLine("3. Exit.");
                Console.WriteLine("========================================================");
                Console.WriteLine("Please enter your choice: ");
                var choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        giaoDich = new GiaoDichSHB();
                        break;
                    case 2:
                        giaoDich = new GiaoDichBlockchain();
                        break;
                    case 3:
                      
                        break;
                    default:
                        Console.WriteLine("Wrong choice, please try again!");
                        break;
                }

                if (choice == 3)
                {
                    Console.WriteLine("Bye! see you again!");
                    break;
                }

                giaoDich.Login();
                if (currentLoggedInAccount != null)
                {
                    
                    Console.WriteLine("Logged in successfully!");
                    Console.WriteLine("=========================");
                    Console.WriteLine($"Account: {currentLoggedInAccount.Username}");
                    Console.WriteLine($"Balance: {currentLoggedInAccount.Balance}");
                    Console.WriteLine("==========================");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadLine();
                    GenerateTransactionMenu(giaoDich);
                }

                if (currentLoggedInAddress != null)
                {
                    
                     Console.WriteLine("Logged in successfully!");
                     Console.WriteLine("==========================");
                     Console.WriteLine($"Address: {currentLoggedInAddress.Address}");
                     Console.WriteLine($"Balance: {currentLoggedInAddress.Balance}");
                     Console.WriteLine("==========================");
                     Console.WriteLine("Press any key to continue.");
                     Console.ReadLine();
                     GenerateTransactionMenu(giaoDich);
                }
            }
        }

        private static void GenerateTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                Console.WriteLine("Please select the transaction type.");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Withdraw.");
                Console.WriteLine("2. Deposit.");
                Console.WriteLine("3. Transfer.");
                Console.WriteLine("4. Exit.");
                Console.WriteLine("===================================");
                Console.WriteLine("Please enter your choice: ");
                var choice1 = int.Parse(Console.ReadLine());
                switch (choice1)
                {
                    case 1:
                        giaoDich.Withdraw();
                        break;
                    case 2:
                        giaoDich.Deposit();
                        break;
                    case 3:
                        giaoDich.Transfer();
                        break;
                    case 4:
                        break;
                    default:
                        Console.WriteLine("Wrong choice, please try again!");
                        break;

                }

                if (choice1 == 4)
                {
                    
                    Console.WriteLine("Bye! see you again!");
                    currentLoggedInAccount = null;
                    currentLoggedInAddress = null;
                    break;
                }
            }
        }
    }
   
}