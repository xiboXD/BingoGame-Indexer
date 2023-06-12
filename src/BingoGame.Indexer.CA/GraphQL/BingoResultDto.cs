using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using GraphQL;
using Nest;
using BingoGame.Indexer.CA.GraphQL;

namespace BingoGame.Indexer.CA.Entities;

public class BingoResultDto
{
    public long TotalRecordCount { get; set; }
    public List<BingoInfo> Data { get; set; }
    public List<BingoStatics> Statics { get; set; }
}

public class BingoInfo 
{
    public long Amount { get; set; }
    public long Award { get; set; }
    public bool IsComplete { get; set; }
    public string PlayId { get; set; }
    public string BingoId { get; set; }
    public int BingoType { get; set; }
    public List<int> Dices { get; set; }
    public string PlayerAddress { get; set; }
    public long PlayTime { get; set; }
    public long BingoTime { get; set; }
    public List<TransactionFee> PlayTransactionFee { get; set; }
    public List<TransactionFee> BingoTransactionFee { get; set; }
    public long PlayBlockHeight { get; set; }
    public long BingoBlockHeight { get; set; }
    public string PlayBlockHash { get; set; }
    public string BingoBlockHash { get; set; }
}

public class BingoStatics
{
    public long TotalWins { get; set; }
    public long TotalPlays { get; set; }
    public long Award { get; set; }
    public long Amount { get; set; }
    public string PlayerAddress { get; set; }
}