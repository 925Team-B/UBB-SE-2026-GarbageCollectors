using System;
using System.Diagnostics;

namespace BankingAppTeamB.Services
{
    public class TwoFAService
    {
        public bool Requires2FA(decimal amount)
        {
            return amount >= 1000;
        }

        // TODO: replace with real OTP validation (e.g. TOTP via Google Authenticator)
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Debug.WriteLine("[2FA] Validation failed: token was empty.");
                return false;
            }

            Debug.WriteLine($"[2FA] Token accepted (placeholder): '{token}'");
            return true;
        }

        // TODO: replace with real OTP generation (e.g. TOTP, SMS, email code)
        public string GenerateToken(int userId)
        {
            string placeholder = "123456";
            Debug.WriteLine($"[2FA] Placeholder token generated for userId={userId}: {placeholder}");
            return placeholder;
        }
    }
}
