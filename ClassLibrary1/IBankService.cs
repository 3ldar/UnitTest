namespace ClassLibrary1
{
    public interface IBankService
    {
        Account GetAccount(string accountNumber);
        void TransferMoney(string sourceAccountNumber, string destinationAccountNumber, decimal transferAmount);
        void UpdateAccountBalance(Account account, decimal amount);
        int[] GetNegativeBalancedAccountIds();
    }
}