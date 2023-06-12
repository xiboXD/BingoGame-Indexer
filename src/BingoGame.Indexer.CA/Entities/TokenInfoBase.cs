using AElfIndexer.Client;
using Google.Protobuf.Collections;
using Nest;

namespace BingoGame.Indexer.CA.Entities;

public class TokenInfoBase: AElfIndexerClientEntity<string>
{
    [Keyword] public override string Id { get; set; }
    
    [Keyword] public string Symbol { get; set; }

    public TokenType Type { get; set; }
    
    /// <summary>
    /// token contract address
    /// </summary>
    [Keyword] public string TokenContractAddress { get; set; }
    
    public int Decimals { get; set; }
    
    public long Supply { get; set; }
    
    public long TotalSupply { get; set; }

    [Keyword] public string TokenName { get; set; }

    [Keyword] public string Issuer { get; set; }

    public bool IsBurnable { get; set; }

    public int IssueChainId { get; set; }
    
    // public TokenExternalInfo TokenExternalInfo { get; set; }

    public Dictionary<string, string> ExternalInfoDictionary { get; set; }
}

public enum TokenType
{
    Token,
    NFTCollection,
    NFTItem
}

// public class TokenExternalInfo
// {
//     public string ImageUrl { get; set; }
//     
//     public long LastItemId { get; set; }
//     
//     public string BaseUrl { get; set; }
//     
//     public string Type { get; set; }
//
//     public bool IsItemIdReuse { get; set; }
//     
//     public bool IsBurned { get; set; }
// }