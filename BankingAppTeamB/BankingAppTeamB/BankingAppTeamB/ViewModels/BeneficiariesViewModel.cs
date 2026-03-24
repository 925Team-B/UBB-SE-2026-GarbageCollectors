using BankingAppTeamB.Commands;
using BankingAppTeamB.Models;
using BankingAppTeamB.Services;
using BankingAppTeamB.Repositories;
using BankingAppTeamB.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAppTeamB.Models.DTOs;

namespace BankingAppTeamB.ViewModels
{
    public class BeneficiariesViewModel : ViewModelBase
    {
        private readonly BeneficiaryService beneficiaryService;

        // Hardcoded for this example.
        private readonly int currentUserId = 1;

        //Backing Fields
        private ObservableCollection<Beneficiary> beneficiaries = new ObservableCollection<Beneficiary>();
        private Beneficiary? selectedBeneficiary;
        private string newName = string.Empty;
        private string newIBAN = string.Empty;
        private string newBankName = string.Empty;
        private string errorMessage = string.Empty;
        private bool isAddFormVisible;

        //Properties
        public ObservableCollection<Beneficiary> Beneficiaries
        {
            get => beneficiaries;
            set => SetProperty(ref beneficiaries, value);
        }

        public Beneficiary? SelectedBeneficiary
        {
            get => selectedBeneficiary;
            set => SetProperty(ref selectedBeneficiary, value);
        }

        public string NewName
        {
            get => newName;
            set => SetProperty(ref newName, value);
        }

        public string NewIBAN
        {
            get => newIBAN;
            set => SetProperty(ref newIBAN, value);
        }

        public string NewBankName
        {
            get => newBankName;
            set => SetProperty(ref newBankName, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public bool IsAddFormVisible
        {
            get => isAddFormVisible;
            set => SetProperty(ref isAddFormVisible, value);
        }

        //Commands
        public IAsyncRelayCommand AddCommand { get; }
        public IRelayCommand<Beneficiary> DeleteCommand { get; }
        public IRelayCommand ShowAddFormCommand { get; }
        public IRelayCommand<Beneficiary> UseForTransferCommand { get; }

        //Constructor
        public BeneficiariesViewModel(BeneficiaryService beneficiaryService)
        {
            this.beneficiaryService = beneficiaryService;

            //Initialize Commands
            AddCommand = new AsyncRelayCommand(AddBeneficiaryAsync);
            DeleteCommand = new RelayCommand<Beneficiary>(DeleteBeneficiary!);
            ShowAddFormCommand = new RelayCommand(ShowAddForm);
            UseForTransferCommand = new RelayCommand<Beneficiary>(UseForTransfer!);
        }
        //Methods
        public async Task LoadAsync()
        {
            var data = beneficiaryService.GetByUser(currentUserId);

            Beneficiaries.Clear();
            foreach (var item in data)
            {
                Beneficiaries.Add(item);
            }

            await Task.CompletedTask;
        }

        private async Task AddBeneficiaryAsync()
        {
            ErrorMessage = string.Empty;

            try
            {
                var newBeneficiary = beneficiaryService.Add(NewName, NewIBAN, NewBankName, currentUserId);

                NewName = string.Empty;
                NewIBAN = string.Empty;
                NewBankName = string.Empty;
                IsAddFormVisible = false;

                await LoadAsync();
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred while saving the beneficiary.";
            }
        }

        private void DeleteBeneficiary(Beneficiary beneficiary)
        {
            if (beneficiary == null) return;

            beneficiaryService.Delete(beneficiary.Id);
            Beneficiaries.Remove(beneficiary);
        }

        private void ShowAddForm()
        {
            NewName = string.Empty;
            NewIBAN = string.Empty;
            NewBankName = string.Empty;
            ErrorMessage = string.Empty;
            IsAddFormVisible = true;
        }

        private void UseForTransfer(Beneficiary beneficiary)
        {
            if (beneficiary == null) return;

            //(0 is the placeholder for SourceAccountId)
            TransferDto transferDto = beneficiaryService.BuildTransferDtoFrom(beneficiary, 0, currentUserId);

            NavigationService.NavigateTo<TransferPage>(transferDto);
        }
    }
}
