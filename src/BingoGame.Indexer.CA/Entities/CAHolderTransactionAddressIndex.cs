using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class CAHolderTransactionAddressIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]
    public string CAAddress { get; set; }
    
    [Keyword]
    public string Address { get; set; }
    
    [Keyword]
    public string AddressChainId { get; set; }
    
    /// <summary>
    /// Latest transaction time
    /// </summary>
    public long TransactionTime { get; set; }
}