public class Operation
{
    public Account Sender { get; }
    public Account Receiver { get; }
    public int Amount { get; }

    public Operation(Account sender, Account receiver, int amount)
    {
        Sender = sender;
        Receiver = receiver;
        Amount = amount;
    }

    public bool Verify()
    {
        if (Amount <= 0) return false;
        if (Sender.Balance < Amount) return false;

        return true;
    }

    public override string ToString()
    {
        return $"{Sender.Name} -> {Receiver.Name}: {Amount}";
    }
}