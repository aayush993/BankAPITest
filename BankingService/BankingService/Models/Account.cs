namespace BankingService.Models
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public AccountType Type { get; set; }
    }

    public enum AccountType
    {
        Savings,
        Chequing,
        Corporate
    }

    public class TransactionRequest
    {
        public decimal Amount { get; set; }
    }

    public class CreateAccountRequest
    {
       
        public int UserId { get; set; }
        public string AccountType { get; set; }
    }
}
