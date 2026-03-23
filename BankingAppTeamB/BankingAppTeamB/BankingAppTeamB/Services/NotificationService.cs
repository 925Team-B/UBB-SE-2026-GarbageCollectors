using BankingAppTeamB.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BankingAppTeamB.Services
{
    public class NotificationService
    {
        public void NotifyTransferCompleted(Transfer transfer)
        {
            string message = $"[NOTIFICATION] Transfer completed: " +
                             $"€{transfer.Amount} {transfer.Currency} " +
                             $"to {transfer.RecipientName} ({transfer.RecipientIBAN}). " +
                             $"Status: {transfer.Status}. " +
                             $"Ref: {transfer.Id} at {transfer.CreatedAt:yyyy-MM-dd HH:mm:ss}";

            Log(message);

            // TODO: replace with real email/toast delivery
            // EmailService.Send(transfer.UserId, "Transfer Completed", message);
            // ToastService.Push(transfer.UserId, message);
        }

        public void NotifyBeneficiaryStatsUpdated(Beneficiary beneficiary, decimal amountSent)
        {
            string message = $"[NOTIFICATION] Beneficiary stats updated: " +
                             $"{beneficiary.Name} ({beneficiary.IBAN}). " +
                             $"Amount sent: €{amountSent}. " +
                             $"Total sent: €{beneficiary.TotalAmountSent}. " +
                             $"Transfer count: {beneficiary.TransferCount}.";

            Log(message);

            // TODO: replace with real email/toast delivery
        }

        public void NotifyRateAlertTriggered(RateAlert alert, decimal currentRate)
        {
            string message = $"[NOTIFICATION] Rate alert triggered: " +
                             $"{alert.BaseCurrency}/{alert.TargetCurrency} " +
                             $"has reached {currentRate} " +
                             $"(your target was {alert.TargetRate}).";

            Log(message);

            // TODO: replace with real email/toast delivery
        }

        public void NotifyRecurringPaymentDue(RecurringPayment payment)
        {
            string message = $"[NOTIFICATION] Recurring payment due soon: " +
                             $"€{payment.Amount} scheduled for " +
                             $"{payment.NextExecutionDate:yyyy-MM-dd}. " +
                             $"Status: {payment.Status}.";

            Log(message);

            // TODO: replace with real email/toast delivery
        }

        public void CheckAndNotifyDuePayments(List<RecurringPayment> payments, TimeSpan warningWindow)
        {
            var now = DateTime.Now;
            foreach (var payment in payments)
            {
                if (payment.NextExecutionDate <= now + warningWindow)
                {
                    Debug.WriteLine($"[SCHEDULER] Due payment detected for payment Id={payment.Id}, " +
                                    $"due {payment.NextExecutionDate:yyyy-MM-dd}");
                    NotifyRecurringPaymentDue(payment);
                }
            }
        }

        public void CheckAndNotifyRateAlerts(List<RateAlert> alerts, Dictionary<string, decimal> liveRates)
        {
            foreach (var alert in alerts)
            {
                string pair = $"{alert.BaseCurrency}/{alert.TargetCurrency}";
                if (!liveRates.ContainsKey(pair)) continue;

                decimal currentRate = liveRates[pair];
                if (currentRate >= alert.TargetRate && !alert.IsTriggered)
                {
                    Debug.WriteLine($"[SCHEDULER] Rate alert triggered for pair {pair}: " +
                                    $"current={currentRate}, target={alert.TargetRate}");
                    NotifyRateAlertTriggered(alert, currentRate);
                }
            }
        }

        private void Log(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
