using AElfIndexer.Client.GraphQL;

namespace BingoGame.Indexer.CA.GraphQL;

public class BingoGameIndexerCASchema : AElfIndexerClientSchema<Query>
{
    public BingoGameIndexerCASchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}