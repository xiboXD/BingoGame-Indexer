using GraphQL;
using BingoGame.Indexer.CA.Entities;

namespace BingoGame.Indexer.CA.GraphQL;

public class CAHolderTransactionDto
{
    public string Id { get; set; }
    
    public string ChainId { get; set; }

    public string BlockHash { get; set; }

    public long BlockHeight { get; set; }

    public string PreviousBlockHash { get; set; }
    
    public string TransactionId { get; set; }
    /// <summary>
    /// Method name
    /// </summary>
    public string MethodName { get; set; }

    // [Name("nftInfo")]
    // public NFTInfo NFTInfo { get; set; }
      
    public TransactionStatus Status { get; set; }

    public long Timestamp { get; set; }

    public TransferInfo TransferInfo { get; set; }
    
    public string FromAddress { get; set; }
    
    public List<TransactionFee> TransactionFees { get; set; }
}

public class TransactionFee
{
    public string Symbol { get; set; }
    
    public long Amount { get; set; }
}