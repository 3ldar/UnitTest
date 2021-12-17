using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class BankService : IBankService, IDisposable
    {
        private IBankRepository repo;
        private readonly IReportingService reportingService;
        private bool disposed = false;

        public BankService(IBankRepository repo, IReportingService reportingService)
        {
            this.repo = repo;
            this.reportingService = reportingService;
        }

        public Account GetAccount(string accountNumber)
        {
            return repo.GetAccount(accountNumber);
        }

        public void UpdateAccountBalance(Account account, decimal amount)
        {
            //if (account == null)
            //{
            //    throw new ArgumentNullException(nameof(account));
            //}

            repo.SetBalance(account.ID, account.Balance += amount);

            repo.AddTransaction(account.ID, amount, account.Balance);

            if (account.Balance < 0)
            {

                reportingService.AccountIsOverdrawn(account.ID);
            }
        }

        public void TransferMoney(string sourceAccountNumber,
                                  string destinationAccountNumber, decimal transferAmount)
        {
            if (transferAmount <= 0)
            {
                throw new InvalidAmountException();
            }

            Account sourceAccount = repo.GetAccount(sourceAccountNumber);

            if (sourceAccount == null)
            {
                throw new AccountNotFoundException(sourceAccountNumber);
            }

            Account destinationAccount = repo.GetAccount(destinationAccountNumber);

            if (destinationAccount == null)
            {
                throw new AccountNotFoundException(destinationAccountNumber);
            }

            if (sourceAccount.Balance < transferAmount)
            {
                throw new InsufficientFundsException();
            }

            // remove transferAmount from destination account
            repo.SetBalance(sourceAccount.ID, sourceAccount.Balance - transferAmount);

            // record the transaction
            repo.AddTransaction(sourceAccount.ID, -transferAmount, sourceAccount.Balance);

            // add transferAmount to source account
            repo.SetBalance
                 (destinationAccount.ID, destinationAccount.Balance + transferAmount);

            // record the transaction
            repo.AddTransaction
                 (destinationAccount.ID, transferAmount, destinationAccount.Balance);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.repo.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int[] GetNegativeBalancedAccountIds()
        {
            return this.reportingService.GetOverdrawnAccounts();
        }
    }

    public interface IReportingService
    {
        void AccountIsOverdrawn(int ıD);

        int[] GetOverdrawnAccounts();
    }

    public class ReportingService
    {
        public ReportingService()
        {
        }

        public void AccountIsOverdrawn(int ıD)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
        {
        }

        public AccountNotFoundException(string? message) : base(message)
        {
        }

        public AccountNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AccountNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class Account
    {
        public decimal Balance { get; set; }
        public int ID { get; set; }
    }

    [Serializable]
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {
        }

        public InsufficientFundsException(string? message) : base(message)
        {
        }

        public InsufficientFundsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InsufficientFundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException()
        {
        }

        public InvalidAmountException(string? message) : base(message)
        {
        }

        public InvalidAmountException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidAmountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public interface IBankRepository : IDisposable
    {
        Account GetAccount(string accountNumber);
        Account GetAccount(int id);
        void SetBalance(int id, decimal balance);
        void AddTransaction(int id, decimal amount, decimal newBalance);
    }
}