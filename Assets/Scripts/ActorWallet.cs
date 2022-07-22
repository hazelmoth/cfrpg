using System;
using ActorComponents;
using Newtonsoft.Json;

[Serializable]
public class ActorWallet : IWallet, IActorComponent
{
    public int Balance { get; set; }
    public int Debt { get; set; }

    [JsonConstructor]
    public ActorWallet() { }

    public ActorWallet(int balance)
    {
        Balance = balance;
    }
    
    public ActorWallet(int balance, int debt)
    {
        Balance = balance;
        Debt = debt;
    }

    public void AddBalance(int amount)
    {
        Balance += amount;
        Balance = Math.Max(Balance, 0);
    }
}
