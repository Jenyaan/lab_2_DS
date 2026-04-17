using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Blockchain blockchain = new Blockchain();

        Account Andrii = new Account("Andrii"); 
        Account Roma = new Account("Roma");
        Account Oleg = new Account("Oleg");

        blockchain.GetTokenFromFaucet(Andrii, 100);

        Console.WriteLine("Initial Balances:");
        Console.WriteLine(Andrii);
        Console.WriteLine(Roma);
        Console.WriteLine(Oleg);
        Console.WriteLine();

        Operation op1 = new Operation(Andrii, Roma, 50);
        Transaction tx1 = new Transaction(new List<Operation> { op1 }, 1); 

        Block block1 = new Block(
            new List<Transaction> { tx1 },
            blockchain.BlockHistory[^1].BlockId 
        );

        blockchain.AddBlock(block1);

        Operation op2 = new Operation(Roma, Oleg, 20);
        Transaction tx2 = new Transaction(new List<Operation> { op2 }, 2);

        Operation op3 = new Operation(Andrii, Oleg, 10);
        Transaction tx3 = new Transaction(new List<Operation> { op3 }, 3); 

        Block block2 = new Block(
            new List<Transaction> { tx2, tx3 }, 
            blockchain.BlockHistory[^1].BlockId 
        );

        blockchain.AddBlock(block2);

        Console.WriteLine("\nBalances after all transactions:");
        Console.WriteLine(Andrii); 
        Console.WriteLine(Roma);   
        Console.WriteLine(Oleg);   
        
        Console.WriteLine("\n-Global Coin Database:");
        foreach(var item in blockchain.CoinDatabase)
        {
            Console.WriteLine($"Acc: {item.Key}, Bal: {item.Value}");
        }

        blockchain.PrintBlockchain();
    }
}