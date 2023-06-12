using AElf.Indexing.Elasticsearch;
using AElfIndexer.Client;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class CAHolderIndex : AElfIndexerClientEntity<string>, IIndexBuild
{
    [Keyword]public override string Id { get; set; }
    
    /// <summary>
    /// CA holder hash(Id)
    /// </summary>
    [Keyword]public string CAHash { get; set; }
    
    /// <summary>
    /// CA holder address
    /// </summary>
    [Keyword]public string CAAddress { get; set; }
    
    /// <summary>
    /// CA holder creator address
    /// </summary>
    [Keyword]public string Creator { get; set; }
    
    /// <summary>
    /// CA Holder manager address list
    /// </summary>
    [Nested(Name = "ManagerInfos",Enabled = true,IncludeInParent = true,IncludeInRoot = true)]
    public List<ManagerInfo> ManagerInfos { get; set; }
    
    [Nested(Name = "Guardians",Enabled = true,IncludeInParent = true,IncludeInRoot = true)]
    public List<Guardian> Guardians { get; set; }
    
    /// <summary>
    /// ChainId where CAHolder created
    /// </summary>
    [Keyword]public string OriginChainId { get; set; }
}

public class ManagerInfo
{
    [Keyword]public string Address { get; set; }
    
    [Keyword]public string ExtraData { get; set; }
}

