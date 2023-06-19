using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portkey.Contracts.BingoGameContract;
using BingoGame.Indexer.CA.Entities;
using BingoGame.Indexer.CA.GraphQL;
using Volo.Abp.ObjectMapping;
using BingoGame.Indexer.CA.Processors;

namespace BingoGame.Indexer.CA.Processors;

public class BingoedProcessor : BingoGameProcessorBase<Bingoed>
{   

    private readonly IAElfIndexerClientEntityRepository<BingoGameIndex, TransactionInfo> _bingoIndexRepository;
    private readonly IAElfIndexerClientEntityRepository<BingoGameStaticsIndex, TransactionInfo> _bingoStaticsIndexRepository;
    private readonly IObjectMapper _objectMapper;
    public BingoedProcessor(ILogger<BingoedProcessor> logger,
        IAElfIndexerClientEntityRepository<BingoGameIndex, TransactionInfo> bingoIndexRepository,
        IAElfIndexerClientEntityRepository<BingoGameStaticsIndex, TransactionInfo> bingoStaticsIndexRepository,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IObjectMapper objectMapper) :
        base(logger,objectMapper,contractInfoOptions)
    {
        _bingoIndexRepository = bingoIndexRepository;
        _objectMapper = objectMapper;
        _bingoStaticsIndexRepository = bingoStaticsIndexRepository;
    }

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoOptions.ContractInfos.First(c=>c.ChainId == chainId).BingoGameContractAddress;
    }

    protected override async Task HandleEventAsync(Bingoed eventValue, LogEventContext context)
    {
        if (eventValue.PlayerAddress == null || eventValue.PlayerAddress.Value == null)
        {
            return;
        }
        //update bingoIndex
        var index = await _bingoIndexRepository.GetFromBlockStateSetAsync(eventValue.PlayId.ToHex(), context.ChainId);
        if (index == null)
        {
            return;
        }
        // _objectMapper.Map<LogEventContext, CAHolderIndex>(context, caHolderIndex);
        index.BingoBlockHeight = context.BlockHeight;
        index.BingoId = context.TransactionId;
        index.BingoTime = context.BlockTime.ToTimestamp().Seconds;
        var feeMap = GetTransactionFee(context.ExtraProperties);
        List<TransactionFee> feeList;
        if (!feeMap.IsNullOrEmpty())
        {
            feeList = feeMap.Select(pair => new TransactionFee
            {
                Symbol = pair.Key,
                Amount = pair.Value
            }).ToList();
        }
        else
        {
            feeList = new List<TransactionFee>
            {
                new ()
                {
                    Symbol = null,
                    Amount = 0
                }
            };
        }
        index.BingoTransactionFee = feeList;
        index.IsComplete = true;
        index.Dices = eventValue.Dices.Dices.ToList();
        index.Award = eventValue.Award;
        index.BingoBlockHash = context.BlockHash;
        _objectMapper.Map<LogEventContext, BingoGameIndex>(context, index);
        await _bingoIndexRepository.AddOrUpdateAsync(index);
        
        //update bingoStaticsIndex
        var staticsId= IdGenerateHelper.GetId(context.ChainId, eventValue.PlayerAddress.ToBase58());
        var bingoStaticsIndex = await _bingoStaticsIndexRepository.GetFromBlockStateSetAsync(staticsId, context.ChainId);
        if (bingoStaticsIndex == null)
        {
            bingoStaticsIndex = new BingoGameStaticsIndex
            {
                Id = staticsId,
                PlayerAddress = eventValue.PlayerAddress.ToBase58(),
                Amount = eventValue.Amount,
                Award = eventValue.Award,
                TotalWins = eventValue.Award > 0 ? 1 : 0,
                TotalPlays = 1
            };
        }
        else
        {
            bingoStaticsIndex.Amount += eventValue.Amount;
            bingoStaticsIndex.Award += eventValue.Award;
            bingoStaticsIndex.TotalPlays += 1;
            bingoStaticsIndex.TotalWins += eventValue.Award > 0 ? 1 : 0;
        }
        _objectMapper.Map<LogEventContext, BingoGameStaticsIndex>(context, bingoStaticsIndex);
        await _bingoStaticsIndexRepository.AddOrUpdateAsync(bingoStaticsIndex); 
    }
}
