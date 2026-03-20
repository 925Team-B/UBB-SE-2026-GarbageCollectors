using BankingAppTeamB.Models;
using BankingAppTeamB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingAppTeamB.Services
{
    public class BeneficiaryService
    {
        private readonly IBeneficiaryRepository repo;

        public BeneficiaryService(IBeneficiaryRepository InputRepo)
        {
            repo = InputRepo;
        }

        public List<Beneficiary> GetByUser(int userId)
        {
            return repo.GetByUserId(userId);
        }

        public Beneficiary Add(string name, string iban, int uid)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty");
            }
            List<Beneficiary> existingBeneficiaries = repo.GetByUserId(uid);
            if (existingBeneficiaries.Any(b => b.IBAN.Equals(iban, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("A beneficiary with this IBAN already exists for this user.");
            }
            Beneficiary beneficiary = new Beneficiary
            {
                UserId = uid,
                Name = name,
                IBAN = iban,
                CreatedAt = DateTime.UtcNow
            };

            repo.Add(beneficiary);
            return beneficiary;
        }
        public void Update(Beneficiary beneficiary)
        {
            if (string.IsNullOrWhiteSpace(beneficiary.Name))
            {
                throw new ArgumentException("Beneficiary name cannot be empty.");
            }

            repo.Update(beneficiary);
        }
        public void Delete(int id)
        {
            repo.Delete(id);
        }
        public TransferDto BuildTransferDtoFrom(Beneficiary beneficiary, int sourceAccountId, int userId)
        {
            return new TransferDto
            {
                UserId = userId,
                SourceAccountId = sourceAccountId,
                RecipientName = beneficiary.Name,
                RecipientIBAN = beneficiary.IBAN
            };
        }
    }
}
