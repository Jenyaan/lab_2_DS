using System;

public class Account
{
    public string AccountId { get; private set; }
    public string Name { get; private set; } 
    public int Balance { get; private set; }

    public Account(string name) 
    {
        Name = name;
        AccountId = Guid.NewGuid().ToString();
        Balance = 0;
    }

    public void UpdateBalance(int amount)
    {
        Balance += amount;
    }

    // Делаем вывод более читаемым
    public override string ToString()
    {
        return $"{Name} (Balance: {Balance})"; 
    }
}