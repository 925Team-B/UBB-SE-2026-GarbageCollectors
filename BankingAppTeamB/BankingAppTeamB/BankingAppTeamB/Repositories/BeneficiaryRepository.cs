using BankingAppTeamB.Data;
using BankingAppTeamB.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BankingAppTeamB.Repositories
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        public void Add(Beneficiary b)
        {
            string sql = @"
                INSERT INTO Beneficiaries
                    (UserId, Name, IBAN, BankName, LastTransferDate,
                     TotalAmountSent, TransferCount, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES
                    (@UserId, @Name, @IBAN, @BankName, @LastTransferDate,
                     @TotalAmountSent, @TransferCount, @CreatedAt)";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", b.UserId));
                    command.Parameters.Add(new SqlParameter("@Name", b.Name));
                    command.Parameters.Add(new SqlParameter("@IBAN", b.IBAN));
                    command.Parameters.Add(new SqlParameter("@BankName", (object?)b.BankName ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@LastTransferDate", (object?)b.LastTransferDate ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@TotalAmountSent", b.TotalAmountSent));
                    command.Parameters.Add(new SqlParameter("@TransferCount", b.TransferCount));
                    command.Parameters.Add(new SqlParameter("@CreatedAt", b.CreatedAt));

                    b.Id = (int)command.ExecuteScalar();
                }
            }
        }

        public Beneficiary GetById(int id)
        {
            string sql = "SELECT * FROM Beneficiaries WHERE Id = @Id";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapBeneficiary(reader);
                    }
                }
            }
            return null;
        }

        public List<Beneficiary> GetByUserId(int uid)
        {
            string sql = "SELECT * FROM Beneficiaries WHERE UserId = @UserId ORDER BY Name ASC";
            var results = new List<Beneficiary>();

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", uid));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            results.Add(MapBeneficiary(reader));
                    }
                }
            }
            return results;
        }

        public void Update(Beneficiary b)
        {
            string sql = @"
                UPDATE Beneficiaries SET
                    Name                = @Name,
                    IBAN                = @IBAN,
                    BankName            = @BankName,
                    LastTransferDate    = @LastTransferDate,
                    TotalAmountSent     = @TotalAmountSent,
                    TransferCount       = @TransferCount
                WHERE Id = @Id";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Name", b.Name));
                    command.Parameters.Add(new SqlParameter("@IBAN", b.IBAN));
                    command.Parameters.Add(new SqlParameter("@BankName", (object?)b.BankName ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@LastTransferDate", (object?)b.LastTransferDate ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@TotalAmountSent", b.TotalAmountSent));
                    command.Parameters.Add(new SqlParameter("@TransferCount", b.TransferCount));
                    command.Parameters.Add(new SqlParameter("@Id", b.Id));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Beneficiaries WHERE Id = @Id";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", id));
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ExistsByIBAN(int uid, string iban)
        {
            string sql = "SELECT COUNT(1) FROM Beneficiaries WHERE UserId = @UserId AND IBAN = @IBAN";

            using (var connection = AppDatabase.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserId", uid));
                    command.Parameters.Add(new SqlParameter("@IBAN", iban));
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        private Beneficiary MapBeneficiary(SqlDataReader reader)
        {
            return new Beneficiary
            {
                Id                  = (int)reader["Id"],
                UserId              = (int)reader["UserId"],
                Name                = (string)reader["Name"],
                IBAN                = (string)reader["IBAN"],
                BankName            = reader["BankName"] == DBNull.Value ? null : (string)reader["BankName"],
                LastTransferDate    = reader["LastTransferDate"] == DBNull.Value ? null : (DateTime?)reader["LastTransferDate"],
                TotalAmountSent     = (decimal)reader["TotalAmountSent"],
                TransferCount       = (int)reader["TransferCount"],
                CreatedAt           = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
