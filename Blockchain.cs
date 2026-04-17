public class Blockchain
{
    public List<Block> BlockHistory { get; } = new List<Block>();
    public Dictionary<string, int> CoinDatabase { get; } = new Dictionary<string, int>();
    public HashSet<string> TxDatabase { get; } = new HashSet<string>();

    public Blockchain()
    {
        InitBlockchain();
    }

    private void InitBlockchain()
    {
        Block genesis = new Block(new List<Transaction>(), "0");
        BlockHistory.Add(genesis);
    }

    public void GetTokenFromFaucet(Account account, int amount)
    {
        account.UpdateBalance(amount);
        CoinDatabase[account.AccountId] = account.Balance;
    }

    public void AddBlock(Block block)
    {
        if (ValidateBlock(block))
        {
            BlockHistory.Add(block);
            Console.WriteLine("Block added!");
        }
        else
        {
            Console.WriteLine("Block invalid!");
        }
    }

    public void PrintBlockchain()
    {
        Console.WriteLine("\n--- History Blockchain ---");
        for (int i = 0; i < BlockHistory.Count; i++)
        {
            Block b = BlockHistory[i];
            Console.WriteLine($"Block #{i}");
            Console.WriteLine($"Hash:     {b.BlockId}");
            Console.WriteLine($"PrevHash: {b.PrevHash}");
            
            foreach (var tx in b.Transactions)
            {
                foreach (var op in tx.Operations)
                {
                    Console.WriteLine($"  [{op.ToString()}]");
                }
            }
            Console.WriteLine("--------------------------");
        }
    }

    private bool ValidateBlock(Block block)
    {
        Block lastBlock = BlockHistory.Last();

        if (block.PrevHash != lastBlock.BlockId)
            return false;

        foreach (var tx in block.Transactions)
        {
            if (TxDatabase.Contains(tx.TransactionId))
                return false;

            foreach (var op in tx.Operations)
            {
                if (!op.Verify())
                    return false;
            }
        }

        foreach (var tx in block.Transactions)
        {
            TxDatabase.Add(tx.TransactionId);
            
            foreach (var op in tx.Operations)
            {
                op.Sender.UpdateBalance(-op.Amount);
                op.Receiver.UpdateBalance(op.Amount);
                
                CoinDatabase[op.Sender.AccountId] = op.Sender.Balance;
                CoinDatabase[op.Receiver.AccountId] = op.Receiver.Balance;
            }
        }

        return true;
    }
}