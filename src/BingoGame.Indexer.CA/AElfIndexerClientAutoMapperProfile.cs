using AElfIndexer.Client.Handlers;
using AutoMapper;
using BingoGame.Indexer.CA.Entities;
using BingoGame.Indexer.CA.GraphQL;
using TransactionFee = BingoGame.Indexer.CA.Entities.TransactionFee;

namespace BingoGame.Indexer.CA;

public class TestGraphQLAutoMapperProfile : Profile
{
    public TestGraphQLAutoMapperProfile()
    {
        CreateMap<BingoGameIndex, BingoInfo>();
        CreateMap<BingoGameStaticsIndex, BingoStatics>();
        CreateMap<LogEventContext, BingoGameIndex>();
        CreateMap<LogEventContext, BingoGameStaticsIndex>();
    }
}