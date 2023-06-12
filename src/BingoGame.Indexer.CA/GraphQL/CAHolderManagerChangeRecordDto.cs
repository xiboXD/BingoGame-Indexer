using GraphQL;

namespace BingoGame.Indexer.CA.GraphQL;

public class CAHolderManagerChangeRecordDto
{
    [Name("caAddress")]
    public string CAAddress { get; set; }
    
    [Name("caHash")]
    public string CAHash { get; set; }
    
    public string Manager { get; set; }
    
    public string ChangeType { get; set; }
    
    public long BlockHeight { get; set; }
    public string BlockHash { get; set; }
}