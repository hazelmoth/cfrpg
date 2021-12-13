
using System;

public interface IWallet
{
    public int Balance { get; set; }

    public void AddBalance(int amount)
    {
        Balance += amount;
        Balance = Math.Max(Balance, 0);
    }
}
