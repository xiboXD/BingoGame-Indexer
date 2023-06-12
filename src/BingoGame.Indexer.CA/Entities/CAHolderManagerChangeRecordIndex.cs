using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class CAHolderManagerChangeRecordIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword] 
    public override string Id { get; set; }
    
    [Keyword]
    public string CAAddress { get; set; }
    
    [Keyword]
    public string CAHash { get; set; }
    
    [Keyword]
    public string Manager { get; set; }
    
    [Keyword]
    public string ChangeType { get; set; }
}