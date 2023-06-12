using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class CAHolderManagerIndex: AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]public override string Id { get; set; }
    
    [Keyword]public string Manager { get; set; }
    
    public List<string> CAAddresses { get; set; }
}