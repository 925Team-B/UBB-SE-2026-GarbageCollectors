using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BankingAppTeamB.Configuration;

public static class ConnectionConfigHelper
{
    public static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        return config.GetConnectionString("BankingApp");
    }
}