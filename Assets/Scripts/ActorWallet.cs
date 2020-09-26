using System;

public class ActorWallet
{
    public int Balance { get; private set; }
    public ActorWallet(int balance)
    {
        Balance = balance;
    }
    public void SetBalance(int amount)
    {
        Balance = amount;
    }
    public void AddBalance(int amount)
    {
        Balance += amount;
        Balance = Math.Max(Balance, 0);
    }
}