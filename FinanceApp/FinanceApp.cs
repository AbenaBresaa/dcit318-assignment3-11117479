using System;
using System.Collections.Generic;
using System.IO;

// Record for transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for transaction processors
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Implementation of different processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}
// Base Account class
public class Account
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New Balance: {Balance:C}");
    }
}

// Sealed class: Specialized savings account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction of {transaction.Amount:C} for {transaction.Category} applied. New Balance: {Balance:C}");
        }
    }
}
// Main app class
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var savingsAccount = new SavingsAccount("ACC246", 1500m);

        var t1 = new Transaction(1, DateTime.Now, 180m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 520m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 900m, "Entertainment");

        var mobileMoney = new MobileMoneyProcessor();
        var bankTransfer = new BankTransferProcessor();
        var cryptoWallet = new CryptoWalletProcessor();

        mobileMoney.Process(t1);
        savingsAccount.ApplyTransaction(t1);
        _transactions.Add(t1);

        bankTransfer.Process(t2);
        savingsAccount.ApplyTransaction(t2);
        _transactions.Add(t2);

        cryptoWallet.Process(t3);
        savingsAccount.ApplyTransaction(t3);
        _transactions.Add(t3);
    }
}

// Entry point
public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}

