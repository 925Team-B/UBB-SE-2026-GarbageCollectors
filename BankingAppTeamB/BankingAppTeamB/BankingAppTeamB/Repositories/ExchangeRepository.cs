using System;
using System.Collections.Generic;
using BankingAppTeamB.Configuration;
using BankingAppTeamB.Models;
using Microsoft.Data.SqlClient;

namespace BankingAppTeamB.Repositories;

public class ExchangeRepository  : IExchangeRepository
{
    public ExchangeTransaction Add(ExchangeTransaction t)
    {
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var query = @"
                INSERT INTO ExchangeTransaction
                (UserId, SourceAccountId, TargetAccountId, SourceCurrency, TargetCurrency,
                 SourceAmount, TargetAmount, ExchangeRate, Commission, Status, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES
                (@UserId, @SourceAccountId, @TargetAccountId, @SourceCurrency, @TargetCurrency,
                 @SourceAmount, @TargetAmount, @Rate, @Commission, @Status, @CreatedAt)";
        
        using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@UserId", t.UserId);
        cmd.Parameters.AddWithValue("@SourceAccountId", t.SourceAccountId);
        cmd.Parameters.AddWithValue("@TargetAccountId", t.TargetAccountId);
        cmd.Parameters.AddWithValue("@SourceCurrency", t.SourceCurrency);
        cmd.Parameters.AddWithValue("@TargetCurrency", t.TargetCurrency);
        cmd.Parameters.AddWithValue("@SourceAmount", t.SourceAmount);
        cmd.Parameters.AddWithValue("@TargetAmount", t.TargetAmount);
        cmd.Parameters.AddWithValue("@Rate", t.ExchangeRate);
        cmd.Parameters.AddWithValue("@Commission", t.Commission);
        cmd.Parameters.AddWithValue("@Status", t.Status);
        cmd.Parameters.AddWithValue("@CreatedAt", t.CreatedAt);

        t.Id = (int)cmd.ExecuteScalar();

        return t;
    }

    public List<ExchangeTransaction> GetByUserId(int userId)
    {
        var list = new List<ExchangeTransaction>();
        
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var query = @"
        SELECT * FROM ExchangeTransaction
        WHERE UserId = @UserId
        ORDER BY CreatedAt DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new ExchangeTransaction
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                SourceCurrency = reader["SourceCurrency"].ToString(),
                TargetCurrency = reader["TargetCurrency"].ToString(),
                SourceAmount = (decimal)reader["SourceAmount"],
                TargetAmount = (decimal)reader["TargetAmount"],
                ExchangeRate = (decimal)reader["ExchangeRate"],
                Commission = (decimal)reader["Commission"],
                Status = Enum.Parse<TransferStatus>(reader["Status"].ToString()),
                CreatedAt = (DateTime)reader["CreatedAt"]
            });
        }
        
        return list;
    }

    public List<RateAlert> GetUserActiveAlerts(int userId)
    {
        var list = new List<RateAlert>();
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var query = @"
        SELECT * FROM RateAlert
        WHERE UserId = @UserId AND IsTriggered = 0";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapAlert(reader));
        }

        return list;
    }
    
    private RateAlert MapAlert(SqlDataReader reader)
    {
        return new RateAlert
        {
            Id = (int)reader["Id"],
            UserId = (int)reader["UserId"],
            BaseCurrency = reader["BaseCurrency"].ToString(),
            TargetCurrency = reader["TargetCurrency"].ToString(),
            TargetRate = (decimal)reader["TargetRate"],
            IsTriggered = (bool)reader["IsTriggered"],
            IsBuyAlert = (bool)reader["IsBuyAlert"],
            CreatedAt = (DateTime)reader["CreatedAt"]
        };
    }

    public List<RateAlert> GetAllActiveAlerts()
    {
        var list = new List<RateAlert>();
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var query = "SELECT * FROM RateAlert WHERE IsTriggered = 0";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(MapAlert(reader));
        }
        
        return list;
    }

    public RateAlert AddAlert(RateAlert alert)
    {
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var query = @"
        INSERT INTO RateAlert
        (UserId, BaseCurrency, TargetCurrency, TargetRate, isTriggered, isBuyAlert, CreatedAt)
        OUTPUT INSERTED.Id
        VALUES (@UserId, @Base, @Target, @Rate, 0, @IsBuyAlert, @CreatedAt)";
        
        using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@UserId", alert.UserId);
        cmd.Parameters.AddWithValue("@Base", alert.BaseCurrency);
        cmd.Parameters.AddWithValue("@Target", alert.TargetCurrency);
        cmd.Parameters.AddWithValue("@Rate", alert.TargetRate);
        cmd.Parameters.AddWithValue("@CreatedAt", alert.CreatedAt);
        cmd.Parameters.AddWithValue("@IsBuyAlert", alert.IsBuyAlert);
        
        alert.Id = (int)cmd.ExecuteScalar();

        return alert;
    }

    public void DeleteAlert(int id)
    {
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var cmd = new SqlCommand("DELETE FROM RateAlert WHERE Id = @id", conn);
        
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void MarkAlertTriggered(int id)
    {
        using var conn = new SqlConnection(ConnectionConfigHelper.GetConnectionString());
        conn.Open();
        
        var cmd = new SqlCommand(
            "UPDATE RateAlert SET IsTriggered = 1 WHERE Id = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", id);

        cmd.ExecuteNonQuery();
    }
    
    
}