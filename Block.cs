public class Block
{
    public string BlockId { get; }
    public string PrevHash { get; }
    public List<Transaction> Transactions { get; }

    public Block(List<Transaction> transactions, string prevHash)
    {
        Transactions = transactions;
        PrevHash = prevHash;
        BlockId = CalculateHash();
    }

    private string CalculateHash()
    {
        string data = PrevHash + string.Join("", Transactions.Select(t => t.TransactionId));
        return Hash.ToSHA1(data);
    }

    public override string ToString()
    {
        return $"Block: {BlockId}\nPrev: {PrevHash}\nTx count: {Transactions.Count}\n";
    }
}