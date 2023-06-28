﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using MelissandreDepartment.DAO;
using MelissandreDepartment.Model; // Assuming the Account class is in the MelissandreDepartment.Model namespace
using MelissandreDepartment.Tool;
using MelissandreServiceLibrary.Enum;

namespace MelissandreDepartment.ViewModel
{
    public abstract class AccountManagementViewModel : INotifyPropertyChanged
    {
        protected HttpClientUserDAO userDAO;
        public RelayCommand SendNewPasswordCommand { get; set; }
        public RelayCommand ActivateAccountsCommand { get; set; }
        public RelayCommand DeactivateAccountsCommand { get; set; }
        public RelayCommand AddAccountCommand { get; set; }
        public RelayCommand GetClientCommand { get; set; }
        public RelayCommand DeleteAccountCommand { get; set; }

        private String message;
        public String Message
        {
            get { return String.IsNullOrEmpty(message) ? String.Empty : message; }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private String emailFormParameter;
        public String EmailFormParameter
        {
            get { return String.IsNullOrEmpty(emailFormParameter)? String.Empty : emailFormParameter; }
            set
            {
                emailFormParameter = value;
                OnPropertyChanged(nameof(EmailFormParameter));
            }
        }

        private String fullNameFormParameter;
        public String FullNameFormParameter
        {
            get { return String.IsNullOrEmpty(fullNameFormParameter) ? String.Empty : fullNameFormParameter; }
            set
            {
                fullNameFormParameter = value;
                OnPropertyChanged(nameof(FullNameFormParameter));
            }
        }

        private ObservableCollection<Account> accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged(nameof(Accounts));
            }
        }

        private ICollectionView accountsView;
        public ICollectionView AccountsView
        {
            get { return accountsView; }
            set
            {
                accountsView = value;
                OnPropertyChanged(nameof(AccountsView));
            }
        }

        protected AccountManagementViewModel()
        {
            // Initialize the accounts collection and view
            userDAO = HttpClientUserDAO.Instance;
            Accounts = new ObservableCollection<Account>();
            AccountsView = CollectionViewSource.GetDefaultView(Accounts);
            SendNewPasswordCommand = new RelayCommand((o) => SendNewPassword(o));
            ActivateAccountsCommand = new RelayCommand(o => ActivateAccounts(), o => CanActivateOrDeactivateAccounts());
            DeactivateAccountsCommand = new RelayCommand(o => DeactivateAccounts(), o => CanActivateOrDeactivateAccounts());
        }

        protected bool IsValidEmail(string email)
        {
            String pattern = @"^[\w\.-]+@[\w\.-]+\.\w+$";
            return Regex.IsMatch(email, pattern);
        }


        protected async void SendNewPassword(object parameter)
        {
            Account account = parameter as Account;
            if (account != null)
            {
                if (account is ClientAccount clientAccount)
                {
                    // Cast to ClientAccount and implement DAO logic for sending a new password
                    string email = clientAccount.Email;
                    ClientAccountType accountType = clientAccount.Role;

                    // Convert accountType to string
                    string accountTypeString = accountType.ToString();

                    try
                    {
                        (bool success, string message) = await userDAO.SendNewPassword(email, accountTypeString);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message; 
                    }
                }
                else if (account is DepartmentAccount departmentAccount)
                {
                    // Cast to DepartmentAccount and implement DAO logic for sending a new password
                    string email = departmentAccount.Email;
                    DepartmentAccountType accountType = departmentAccount.Role;

                    // Convert accountType to string
                    string accountTypeString = accountType.ToString();

                    try
                    {
                        (bool success, string message) = await userDAO.SendNewPassword(email, accountTypeString);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected async void ActivateAccounts()
        {
            List<Account> selectedAccounts = GetSelectedAccounts();
            // Implement DAO logic
            foreach (Account account in selectedAccounts)
            {
                account.Status = AccountStatus.Active;

                if (account is ClientAccount clientAccount)
                {
                    try
                    {
                        (bool success, string message) = await userDAO.ActivateAccount(clientAccount.Email, clientAccount.Role.ToString(), true);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message;
                    }
                }
                else if (account is DepartmentAccount departmentAccount)
                {

                    try
                    {
                        (bool success, string message) = await userDAO.ActivateAccount(departmentAccount.Email, departmentAccount.Role.ToString(), true);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            AccountsView.Refresh();
        }

        protected async void DeactivateAccounts()
        {
            List<Account> selectedAccounts = GetSelectedAccounts();

            // Implement DAO logic
            foreach (Account account in selectedAccounts)
            {
                account.Status = AccountStatus.Inactive;

                if (account is ClientAccount clientAccount)
                {
                    try
                    {
                        (bool success, string message) = await userDAO.DeactivateAccount(clientAccount.Email, clientAccount.Role.ToString(), false);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message;
                    }
                }
                else if (account is DepartmentAccount departmentAccount)
                {
                    try
                    {
                        (bool success, string message) = await userDAO.DeactivateAccount(departmentAccount.Email, departmentAccount.Role.ToString(), false);
                        Message = message;
                    }
                    catch (Exception ex)
                    {
                        Message = message;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            AccountsView.Refresh();
        }

        protected bool CanActivateOrDeactivateAccounts()
        {
            return GetSelectedAccounts().Any();
        }

        protected List<Account> GetSelectedAccounts()
        {
            List<Account> selectedAccounts = new List<Account>();

            foreach (Account account in Accounts)
            {
                if (account.IsSelected)
                {
                    selectedAccounts.Add(account);
                }
            }

            return selectedAccounts;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected string GeneratePassword()
        {
            // Implement your password generation logic here
            // Example: Generate a random alphanumeric password of length 8
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
    }
}