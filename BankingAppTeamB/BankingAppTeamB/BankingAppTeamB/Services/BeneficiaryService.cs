using BankingAppTeamB.Models;
using BankingAppTeamB.Models.DTOs;
using BankingAppTeamB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using BankingAppTeamB.Models.DTOs;

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
        public bool ValidateIBAN(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban)) return false;
            if (iban.Length < 15 || iban.Length > 34) return false;
            if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1])) return false;
            if (!char.IsDigit(iban[2]) || !char.IsDigit(iban[3])) return false;
            return true;
        }
        public Beneficiary Add(string name, string iban, int uid)
        {
            if (ValidateIBAN(iban) == false)
            {
                throw new ArgumentException("Invalid IBAN format.");
            }
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

        internal object Add(string name, string iban, string newBankName, int uid)
        {
            if (ValidateIBAN(iban) == false)
            {
                throw new ArgumentException("Invalid IBAN format.");
            }
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
                BankName = newBankName,
                CreatedAt = DateTime.UtcNow
            };

            repo.Add(beneficiary);
            return beneficiary;
        }
    }
}
