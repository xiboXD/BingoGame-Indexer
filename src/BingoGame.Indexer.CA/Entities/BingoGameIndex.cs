using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;
using BingoGame.Indexer.CA.GraphQL;

namespace BingoGame.Indexer.CA.Entities;

public class BingoGameIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]public long PlayBlockHeight { get; set; }
    [Keyword]public long BingoBlockHeight { get; set; }
    [Keyword]public string PlayBlockHash { get; set; }
    [Keyword]public string BingoBlockHash { get; set; }
    [Keyword]public long Amount { get; set; }
    [Keyword]public long Award { get; set; }
    [Keyword]public bool IsComplete { get; set; }
    [Keyword]public string PlayId { get; set; }
    [Keyword]public string BingoId { get; set; }
    [Keyword]public int BingoType { get; set; }
    public List<int> Dices { get; set; }
    [Keyword]public string PlayerAddress { get; set; }
    [Keyword]public long PlayTime { get; set; }
    [Keyword]public long BingoTime { get; set; }
    public List<TransactionFee> PlayTransactionFee { get; set; }
    public List<TransactionFee> BingoTransactionFee { get; set; }
}
