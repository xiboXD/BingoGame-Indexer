using GraphQL;

namespace BingoGame.Indexer.CA.GraphQL;

public class CAAddressInfo
{
    [Name("caAddress")] public string CAAddress { get; set; }
    public string ChainId { get; set; }
}