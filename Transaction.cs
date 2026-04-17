using System;
using System.Collections.Generic;
using System.Linq;

public class Transaction
{
    public string TransactionId { get; }
    public List<Operation> Operations { get; }
    public int Nonce { get; }

    public Transaction(List<Operation> operations, int nonce)
    {
        Operations = operations;
        Nonce = nonce;
        TransactionId = CalculateHash();
    }

    private string CalculateHash()
    {
        string data = string.Join("", Operations.Select(o => o.ToString())) + Nonce;
        return Hash.ToSHA1(data);
    }

    public override string ToString()
    {
        return TransactionId;
    }
}