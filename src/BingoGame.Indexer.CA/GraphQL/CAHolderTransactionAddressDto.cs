using GraphQL;

namespace BingoGame.Indexer.CA.GraphQL;

public class CAHolderTransactionAddressDto
{
    public string ChainId { get; set; }
    
    [Name("caAddress")]
    public string CAAddress { get; set; }
    
    public string Address { get; set; }
    
    public string AddressChainId { get; set; }
    
    /// <summary>
    /// Latest transaction time
    /// </summary>
    public long TransactionTime { get; set; }
}