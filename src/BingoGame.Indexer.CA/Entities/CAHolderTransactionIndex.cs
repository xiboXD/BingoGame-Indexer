using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using GraphQL;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class CAHolderTransactionIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]public override string Id { get; set; }
    [Keyword]public string TransactionId { get; set; }
    /// <summary>
    /// Method name
    /// </summary>
    [Keyword] public string MethodName { get; set; }

    // public NFTInfo NFTInfo { get; set; }
      
    public TransactionStatus Status { get; set; }

    public long Timestamp { get; set; }

    public TransferInfo TransferInfo { get; set; }
    
    [Keyword]
    public string FromAddress { get; set; }
    
    public Dictionary<string,long> TransactionFee { get; set; }
}

// public class TokenInfo
// {
//     [Keyword]
//     public string Symbol { get; set; }
//     
//     public int Decimals { get; set; }
// }
//
// public class NFTInfo
// {
//     [Keyword]
//     public string Url { get; set; }
//     
//     [Keyword]
//     public string Alias { get; set; }
//     
//     [Name("nftId")]
//     public long NFTId { get; set; }
// }

public class TransferInfo
{
    [Keyword]
    public string FromAddress { get; set; }
    [Keyword]
    public string FromCAAddress { get; set; }
    [Keyword]
    public string ToAddress { get; set; }
    public long Amount { get; set; }
    [Keyword]
    public string FromChainId { get; set; }
    [Keyword]
    public string ToChainId { get; set; }
    [Keyword]
    public string TransferTransactionId { get; set; }
}

public enum TransactionStatus
{
    NotExisted,
    Pending,
    Failed,
    Mined,
    Conflict,
    PendingValidation,
    NodeValidationFailed,
}