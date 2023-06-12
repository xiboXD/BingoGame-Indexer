using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class BingoGameStaticsIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]public long Amount { get; set; }
    [Keyword]public long Award { get; set; }
    [Keyword]public string PlayerAddress { get; set; }
    [Keyword]public long TotalWins { get; set; }
    [Keyword]public long TotalPlays { get; set; }
}