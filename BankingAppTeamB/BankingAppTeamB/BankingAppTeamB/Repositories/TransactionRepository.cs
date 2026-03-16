using BankingAppTeamB.Data;
using BankingAppTeamB.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BankingAppTeamB.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public void Add(Transaction t)
        {
            string insertSql = @"
                INSERT INTO Transactions
                    (AccountId, CardId, TransactionRef, Type, Direction,
                     Amount, Currency, BalanceAfter, CounterpartyName, CounterpartyIBAN,
                     Fee, ExchangeRate, Status, RelatedEntityType, RelatedEntityId, CreatedAt)
                OUTPUT INSERTED.Id 
                VALUES
                    (@AccountId, @CardId, @TempRef, @Type, @Direction,
                     @Amount, @Currency, @BalanceAfter, @CounterpartyName, @CounterpartyIBAN,
                     @Fee, @ExchangeRate, @Status, @RelatedEntityType, @RelatedEntityId, @CreatedAt)";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();

                int newId;
                using (var command = new SqlCommand(insertSql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@AccountId", t.AccountId));
                    command.Parameters.Add(new SqlParameter("@CardId", (object?)t.CardId ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@TempRef", "PLACEHOLDER"));
                    command.Parameters.Add(new SqlParameter("@Type", t.Type));
                    command.Parameters.Add(new SqlParameter("@Direction", t.Direction));
                    command.Parameters.Add(new SqlParameter("@Amount", t.Amount));
                    command.Parameters.Add(new SqlParameter("@Currency", t.Currency));
                    command.Parameters.Add(new SqlParameter("@BalanceAfter", t.BalanceAfter));
                    command.Parameters.Add(new SqlParameter("@CounterpartyName", t.CounterpartyName));
                    command.Parameters.Add(new SqlParameter("@CounterpartyIBAN", (object?)t.CounterpartyIBAN ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Fee", t.Fee));
                    command.Parameters.Add(new SqlParameter("@ExchangeRate", (object?)t.ExchangeRate ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Status", t.Status));
                    command.Parameters.Add(new SqlParameter("@RelatedEntityType", (object?)t.RelatedEntityType ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@RelatedEntityId", (object?)t.RelatedEntityId ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CreatedAt", t.CreatedAt));

                    newId = (int)command.ExecuteScalar();
                }

                string transactionRef = $"TXN-{t.CreatedAt:yyyyMMdd}-{newId:D4}";

                string updateRefSql = "UPDATE Transactions SET TransactionRef = @Ref WHERE Id = @Id";
                using (var updateCommand = new SqlCommand(updateRefSql, connection))
                {
                    updateCommand.Parameters.Add(new SqlParameter("@Ref", transactionRef));
                    updateCommand.Parameters.Add(new SqlParameter("@Id", newId));
                    updateCommand.ExecuteNonQuery();
                }

                t.Id = newId;
                t.TransactionRef = transactionRef;
            }
        }

        public Transaction GetById(int id)
        {
            string sql = "SELECT * FROM Transactions WHERE Id = @Id";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapTransaction(reader);
                    }
                }
            }
            return null;
        }

        public List<Transaction> GetByUserId(int userId)
        {
            string sql = "SELECT * FROM Transactions WHERE AccountId = @UserId";
            var results = new List<Transaction>();

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            results.Add(MapTransaction(reader));
                    }
                }
            }
            return results;
        }

        private Transaction MapTransaction(SqlDataReader reader)
        {
            return new Transaction
            {
                Id                  = (int)reader["Id"],
                AccountId           = (int)reader["AccountId"],
                CardId              = reader["CardId"] == DBNull.Value ? null : (int?)reader["CardId"],
                TransactionRef      = (string)reader["TransactionRef"],
                Type                = (string)reader["Type"],
                Direction           = (string)reader["Direction"],
                Amount              = (decimal)reader["Amount"],
                Currency            = (string)reader["Currency"],
                BalanceAfter        = (decimal)reader["BalanceAfter"],
                CounterpartyName    = (string)reader["CounterpartyName"],
                CounterpartyIBAN    = reader["CounterpartyIBAN"] == DBNull.Value ? null : (string)reader["CounterpartyIBAN"],
                Fee                 = (decimal)reader["Fee"],
                ExchangeRate        = reader["ExchangeRate"] == DBNull.Value ? null : (decimal?)reader["ExchangeRate"],
                Status              = (string)reader["Status"],
                RelatedEntityType   = reader["RelatedEntityType"] == DBNull.Value ? null : (string)reader["RelatedEntityType"],
                RelatedEntityId     = reader["RelatedEntityId"] == DBNull.Value ? null : (int?)reader["RelatedEntityId"],
                CreatedAt           = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
