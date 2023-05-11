

using System;
using System.Collections.Generic;
using BankingService.Models;

namespace BankingService
{
    public static class BankingOperations
    {
        private static List<User> _users = new List<User>();
        private static List<Account> _accounts = new List<Account>();

        public static User CreateUser(User user)
        {
            //Assign a user ID and initialize Accounts list.
            user.Id = _users.Count + 1;
            user.Accounts = new List<Account>();

            //Add user to the users list.
            _users.Add(user);

            return user;
        }

        public static List<User> GetUsers()
        {
            return _users;
        }

        public static void DeleteUser(int userId)
        {
            var user = GetUser(userId);
            if (user == null) throw new ArgumentException("User not found.");
            if (user.Accounts.Count > 0) throw new InvalidOperationException("User still has accounts.");
            _users.Remove(user);
        }

        public static Account CreateAccount(int userId, AccountType accountType)
        {
            var user = GetUser(userId);
            if (user == null) throw new ArgumentException("User not found.");
            var account = new Account
            {
                Id = _accounts.Count + 1,
                Balance = 0,
                UserId = userId,
                Type = accountType
            };
            user.Accounts.Add(account);
            _accounts.Add(account);
            return account;
        }

        public static void DeleteAccount(int accountId)
        {
            var account = GetAccount(accountId);
            if (account == null) throw new ArgumentException("Account not found.");
            if (account.Balance < 100) throw new InvalidOperationException("Account balance cannot be less than $100.");
            var user = GetUser(account.UserId);
            user.Accounts.Remove(account);
            _accounts.Remove(account);
        }

        public static decimal Deposit(int accountId, decimal amount)
        {
            if (amount > 10000) throw new ArgumentException("Deposit amount cannot be greater than $10,000.");
            var account = GetAccount(accountId);
            if (account == null) throw new ArgumentException("Account not found.");
            account.Balance += amount;
            return account.Balance;
        }

        public static decimal Withdraw(int accountId, decimal amount)
        {
            var account = GetAccount(accountId);
            if (account == null) throw new ArgumentException("Account not found.");
            if (amount > account.Balance * 0.9m) throw new InvalidOperationException("Withdrawal amount cannot be greater than 90% of the account balance.");
            if (account.Balance - amount < 100) throw new InvalidOperationException("Account balance cannot be less than $100.");
            account.Balance -= amount;
            return account.Balance;
        }

        public static User GetUser(int userId)
        {
            return _users.Find(u => u.Id == userId);
        }

        public static Account GetAccount(int accountId)
        {
            return _accounts.Find(a => a.Id == accountId);
        }
    }
}
