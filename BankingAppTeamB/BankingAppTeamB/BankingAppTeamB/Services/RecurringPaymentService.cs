using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BankingAppTeamB.Services
{
    public class RecurringPaymentService
    {
        private readonly IRecurringPaymentRepository recurringPaymentRepository;
        private readonly BillPaymentService billPaymentService;

        public RecurringPaymentService(IRecurringPaymentRepository recurringPaymentRepository, BillPaymentService billPaymentService)
        {
            this.recurringPaymentRepository = recurringPaymentRepository;
            this.billPaymentService = billPaymentService;
        }

        public DateTime ComputeNextRunDate(RecurringFrequency frequency, DateTime from)
        {
            return frequency switch
            {
                RecurringFrequency.Weekly => from.AddDays(7),
                RecurringFrequency.Monthly => from.AddMonths(1),
                RecurringFrequency.BiWeekly => from.AddMonths(3),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), $"Unknown frequency: {frequency}")
            };
        }

        public RecurringPayment Create(RecurringPaymentDto dto)
        {
            var recurringPayment = new RecurringPayment
            {
                UserId = dto.UserId,
                BillerId = dto.BillerId,
                SourceAccountId = dto.SourceAccountId,
                Amount = dto.Amount,
                IsPayInFull = dto.IsPayInFull,
                Frequency = dto.Frequency,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                NextExecutionDate = dto.StartDate,
                Status = PaymentStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            return recurringPaymentRepository.Add(recurringPayment);
        }

        public List<RecurringPayment> GetByUser(int userId)
        {
            return recurringPaymentRepository.GetByUserId(userId);
        }

        public void Pause(int id)
        {
            var payment = recurringPaymentRepository.GetById(id);
            if (payment == null)
                throw new InvalidOperationException($"Recurring payment with ID {id} does not exist.");

            payment.Status = PaymentStatus.Paused;
            recurringPaymentRepository.Update(payment);
        }

        public void Resume(int id)
        {
            var payment = recurringPaymentRepository.GetById(id);
            if (payment == null)
                throw new InvalidOperationException($"Recurring payment with ID {id} does not exist.");

            payment.Status = PaymentStatus.Active;
            recurringPaymentRepository.Update(payment);
        }

        public void Cancel(int id)
        {
            var payment = recurringPaymentRepository.GetById(id);
            if (payment == null)
                throw new InvalidOperationException($"Recurring payment with ID {id} does not exist.");

            payment.Status = PaymentStatus.Cancelled;
            recurringPaymentRepository.Update(payment);
        }

        public void ProcessDuePayments()
        {
            var duePayments = recurringPaymentRepository.GetDueBefore(DateTime.UtcNow);

            foreach (var payment in duePayments)
            {
                try
                {
                    var dto = new BillPaymentDto
                    {
                        UserId = payment.UserId,
                        SourceAccountId = payment.SourceAccountId,
                        BillerId = payment.BillerId,
                        BillerReference = string.Empty,
                        Amount = payment.Amount,
                        IsPayInFull = payment.IsPayInFull
                    };

                    billPaymentService.PayBill(dto);

                    payment.NextExecutionDate = ComputeNextRunDate(payment.Frequency, payment.NextExecutionDate);
                    recurringPaymentRepository.Update(payment);
                }
                catch (Exception ex)
                {
                    payment.Status = PaymentStatus.Failed;
                    recurringPaymentRepository.Update(payment);
                    Debug.WriteLine($"[RecurringPaymentService] Payment ID {payment.Id} failed: {ex.Message}");
                }
            }
        }

        public List<RecurringPayment> GetDueSoon()
        {
            var dueSoon = recurringPaymentRepository.GetDueBefore(DateTime.UtcNow.AddHours(24));

            foreach (var payment in dueSoon)
            {
                Debug.WriteLine($"[RecurringPaymentService] NOTIFICATION STUB � Payment ID {payment.Id} for UserId {payment.UserId} is due on {payment.NextExecutionDate:yyyy-MM-dd HH:mm}");
            }

            return dueSoon;
        }
    }
}