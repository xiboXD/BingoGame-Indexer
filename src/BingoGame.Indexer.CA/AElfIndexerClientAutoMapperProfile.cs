using AElfIndexer.Client.Handlers;
using AutoMapper;
using BingoGame.Indexer.CA.Entities;
using BingoGame.Indexer.CA.GraphQL;
using TransactionFee = BingoGame.Indexer.CA.GraphQL.TransactionFee;

namespace BingoGame.Indexer.CA;

public class TestGraphQLAutoMapperProfile : Profile
{
    public TestGraphQLAutoMapperProfile()
    {
        CreateMap<LogEventContext, CAHolderIndex>();
        CreateMap<LogEventContext, CAHolderManagerIndex>();
        CreateMap<LogEventContext, CAHolderTransactionIndex>();

        CreateMap<CAHolderTransactionIndex, CAHolderTransactionDto>()
            .ForMember(c => c.TransactionFees, opt => opt.MapFrom<TransactionFeeResolver>());
        CreateMap<LogEventContext, CAHolderManagerIndex>();
        CreateMap<CAHolderIndex, CAHolderManagerDto>();
        CreateMap<CAHolderTransactionAddressIndex, CAHolderTransactionAddressDto>();
        CreateMap<CAHolderManagerChangeRecordIndex, CAHolderManagerChangeRecordDto>();

        CreateMap<CAHolderTransactionIndex, CAHolderTransactionDto>()
            .ForMember(c => c.TransactionFees, opt => opt.MapFrom<TransactionFeeResolver>());
        CreateMap<CAHolderIndex, CAHolderManagerDto>();

        CreateMap<BingoGameIndex, BingoInfo>();
        CreateMap<BingoGameStaticsIndex, BingoStatics>();
        CreateMap<LogEventContext, BingoGameIndex>();
        CreateMap<LogEventContext, BingoGameStaticsIndex>();


        CreateMap<CAHolderIndex, CAHolderInfoDto>().ForMember(d => d.GuardianList,
            opt => opt.MapFrom(e => e.Guardians.IsNullOrEmpty() ? null : new GuardianList { Guardians = e.Guardians }));
        CreateMap<Portkey.Contracts.CA.Guardian, Guardian>()
            .ForMember(d => d.IdentifierHash, opt => opt.MapFrom(e => e.IdentifierHash.ToHex()))
            .ForMember(d => d.VerifierId, opt => opt.MapFrom(e => e.VerifierId.ToHex()))
            .ForMember(d => d.Type, opt => opt.MapFrom(e => (int)e.Type));
    }
}

public class
    TransactionFeeResolver : IValueResolver<CAHolderTransactionIndex, CAHolderTransactionDto, List<TransactionFee>>
{
    public List<TransactionFee> Resolve(CAHolderTransactionIndex source, CAHolderTransactionDto destination,
        List<TransactionFee> destMember,
        ResolutionContext context)
    {
        var list = new List<TransactionFee>();
        foreach (var (symbol, amount) in source.TransactionFee)
        {
            list.Add(new TransactionFee
            {
                Amount = amount,
                Symbol = symbol
            });
        }

        return list;
    }
}