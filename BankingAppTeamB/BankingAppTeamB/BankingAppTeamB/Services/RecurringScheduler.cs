using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using BankingAppTeamB.Services;

namespace BankingAppTeamB.Services
{
    public class RecurringScheduler
    {
        private readonly RecurringPaymentService recurringPaymentService;
        private readonly ExchangeService exchangeService;
        private readonly System.Timers.Timer timer;

        public RecurringScheduler(RecurringPaymentService recurringPaymentService, ExchangeService exchangeService, System.Timers.Timer timer)
        {
            this.recurringPaymentService = recurringPaymentService;
            this.exchangeService = exchangeService;
            this.timer = timer;
            this.timer.Elapsed += OnTick;
        }

        public void SetExchangeService(ExchangeService exchangeService) { }

        public void Start()
        {
            timer.Start();
            Debug.WriteLine("[RecurringScheduler] Scheduler started.");
        }

        public void Stop()
        {
            timer.Stop();
            Debug.WriteLine("[RecurringScheduler] Scheduler stopped.");
        }

        private void OnTick(object? sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"[RecurringScheduler] Tick at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

            try
            {
                recurringPaymentService.ProcessDuePayments();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RecurringScheduler] ProcessDuePayments failed: {ex.Message}");
            }

            try
            {
                exchangeService.CheckRateAlerts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RecurringScheduler] CheckRateAlerts failed: {ex.Message}");
            }
        }
    }
}