using AElfIndexer.Client.GraphQL;

namespace BingoGame.Indexer.CA.GraphQL;

public class PortKeyIndexerCASchema : AElfIndexerClientSchema<Query>
{
    public PortKeyIndexerCASchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}