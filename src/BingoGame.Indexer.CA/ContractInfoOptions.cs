namespace BingoGame.Indexer.CA;

public class ContractInfoOptions
{
    public List<ContractInfo> ContractInfos { get; set; }
}

public class ContractInfo
{
    public string ChainId { get; set; }
    public string GenesisContractAddress { get; set; }
    public string CAContractAddress { get; set; }
    public string BingoGameContractAddress { get; set; }
    public string TokenContractAddress { get; set; }
    
    public string NFTContractAddress { get; set; }
}