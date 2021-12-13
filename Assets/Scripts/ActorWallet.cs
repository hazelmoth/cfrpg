﻿using System;

public class ActorWallet : IWallet
{
    public int Balance { get; set; }

    public ActorWallet(int balance)
    {
        Balance = balance;
    }

    public void AddBalance(int amount)
    {
        Balance += amount;
        Balance = Math.Max(Balance, 0);
    }
}
