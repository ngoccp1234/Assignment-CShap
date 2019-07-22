using System;
using MySql.Data.MySqlClient;
using Spring_Hero_Bank_on_CSharp.entity;

namespace Spring_Hero_Bank_on_CSharp.model
{
    public class BlockchainAddressModel
    {
        public BlockchainAddress FindByAddressAndPrivateKey(string address, string privateKey)
        {
            // Tạo connection đến db, lấy ra trong bảng shb account những tài khoản có username, password trùng.            
            var cmd = new MySqlCommand(
                "select * from blockchain where address = @address and privateKey = @privateKey",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@privateKey", privateKey);
            // Tạo ra một đối tượng của lớp shbAccount.
            BlockchainAddress blockchainAddress = null;
            // Đóng Connection và trả về đối tượng này.  
            var dataReader = cmd.ExecuteReader();

            if (dataReader.Read())
            {
                blockchainAddress = new BlockchainAddress()
                {
                    Address = dataReader.GetString("address"),
                    PrivateKey = dataReader.GetString("privateKey"),
                    Balance = dataReader.GetDouble("balance")
                };

            }
            ConnectionHelper.CloseConnection();
            // Trong trường hợp không tìm thấy tài khoản thì trả về null.
            return blockchainAddress;
        }

        public BlockchainAddress GetAddress(string addrress)
        {
            ConnectionHelper.GetConnection();
            var queryString = "select * from `blockchain` where `address` = @address";
            var cmd = new MySqlCommand(queryString, ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@address", addrress);
            var dataReader = cmd.ExecuteReader();
            BlockchainAddress blockchainAddress = null;
            if (dataReader.Read())
            {
                blockchainAddress = new BlockchainAddress()
                {
                    Address = dataReader.GetString("address"),
                    PrivateKey = dataReader.GetString("privateKey"),
                    Balance = dataReader.GetDouble("balance"),
                  
                };
            }
            dataReader.Close();
            ConnectionHelper.CloseConnection();
            return blockchainAddress;
        }

        public bool UpdateBalanceBlockchain(BlockchainAddress currentLoggedInAddress,
            BlockchainTransaction blockchainTransaction)
        {
            // 4. Commit transaction.
            ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                var cmd = new MySqlCommand("select balance from blockchain where address = @address",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                BlockchainAddress blockchainAddress = null;
                var dataReader = cmd.ExecuteReader();
                double currentAddressBalance = 0;
                if (dataReader.Read())
                {
                    currentAddressBalance = dataReader.GetDouble("balance");
                }

           
               
                dataReader.Close();
                if (currentAddressBalance < 0)
                {
                    Console.WriteLine("Not enough money in the account.");
                    return false;
                }
                    
                if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.WITHDRAW &&
                    currentAddressBalance < blockchainTransaction.Amount)
                {
                    throw new Exception("Not enough money in the account.");

                }
                
                if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.WITHDRAW)
                {
                    currentAddressBalance -= blockchainTransaction.Amount;
                }
                
                else if (blockchainTransaction.Type == BlockchainTransaction.TransactionType.DEPOSIT)
                {
                    currentAddressBalance += blockchainTransaction.Amount;
                }
                else if(blockchainTransaction.Type == BlockchainTransaction.TransactionType.TRANSFER)
                {
                   

                }
                
                var updateQuery =
                    "update `blockchain` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAddressBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();
                
                var historyTransactionQuery =
                    "insert into `blockchaintransaction` (transactionId, type, senderAddress, receiverAddress, amount, message, createAtMLS, updateAtMLS, status) " +
                    "values (@transactionId, @type, @senderAddress, @receiverAddress, @amount, @message, @createAtMLS, @updateAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", blockchainTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", blockchainTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", blockchainTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", blockchainTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@createAtMLS", blockchainTransaction.CreateAtMlS);
                historyTransactionCmd.Parameters.AddWithValue("@updateAtMLS", blockchainTransaction.UpdateAtMlS);
                historyTransactionCmd.Parameters.AddWithValue("@status", blockchainTransaction.Status);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    blockchainTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    blockchainTransaction.ReceiverAddress);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("cannot add a transaction or update an account, please check again.");
                }

                transaction1.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction1.Rollback(); // lưu giao dịch vào.                
                return false;
            }
            ConnectionHelper.CloseConnection();
            return true;
        }
        
              public bool Transfer(BlockchainAddress currentLoggedInAddress, BlockchainTransaction blockchainTransaction)
        {
            ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                // Kiểm tra số dư tài khoản.
                var selectBalance =
                    "select balance from blockchain where address = @address";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var dataReader = cmdSelect.ExecuteReader();
                double currentAccountBalance = 0;
                if (dataReader.Read())
                {
                    currentAccountBalance = dataReader.GetDouble("balance");

                }

                dataReader.Close();

                if (currentAccountBalance < blockchainTransaction.Amount)
                {
                    throw new Exception("Not enough money in the account.");

                }

                currentAccountBalance -= blockchainTransaction.Amount;
                //Tiến hành trừ tiền tài khoản gửi.





                // Update tài khoản.

                var updateQuery =
                    "update `blockchain` set `balance` = @balance where address = @address";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateResult = sqlCmd.ExecuteNonQuery();

                // Kiểm tra số dư tài khoản.
                var selectBalanceReceiver =
                    "select balance from `blockchain` where address = @address";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@address", blockchainTransaction.ReceiverAddress);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                double receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close(); // important. 
                //Tiến hành cộng tiền tài khoản nhận.
                receiverBalance += blockchainTransaction.Amount;

                // Update tài khoản.
                var updateQueryReceiver =
                    "update `blockchain` set `balance` = @balance where address = @address";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@address", blockchainTransaction.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                // Lưu lịch sử giao dịch.
                var historyTransactionQuery =
                    "insert into `blockchaintransaction` (transactionId, amount, type, message, senderAddress, receiverAddress,createAtMlS, updateAtMlS, status) " +
                    "values (@transactionId, @amount, @type, @message, @senderAddress, @receiverAddress, @createAtMlS, @updateAtMlS, @status )";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", blockchainTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", blockchainTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", blockchainTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", blockchainTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    blockchainTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    blockchainTransaction.ReceiverAddress);
                historyTransactionCmd.Parameters.AddWithValue("@createAtMlS", blockchainTransaction.CreateAtMlS);
                historyTransactionCmd.Parameters.AddWithValue("@updateAtMlS", blockchainTransaction.UpdateAtMlS);
                historyTransactionCmd.Parameters.AddWithValue("@status", blockchainTransaction.Status);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("cannot add a transaction or update an account, please check again.");
                }

                transaction1.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction1.Rollback();
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.ToString());
                return false;
            }
            finally
            {
                ConnectionHelper.CloseConnection();        
            }
            
        }
    }
    
}