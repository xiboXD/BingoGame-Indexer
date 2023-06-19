using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Portkey.Contracts.BingoGameContract;
using Portkey.Contracts.CA;
using BingoGame.Indexer.CA.Entities;
using BingoGame.Indexer.CA.GraphQL;
using Volo.Abp.ObjectMapping;

namespace BingoGame.Indexer.CA.Processors;

public class PlayedProcessor : BingoGameProcessorBase<Played>
{
    private readonly IAElfIndexerClientEntityRepository<BingoGameIndex, TransactionInfo> _bingoIndexRepository;
    private readonly IAElfIndexerClientEntityRepository<BingoGameStaticsIndex, TransactionInfo> _bingoStaticsIndexRepository;
    private readonly IObjectMapper _objectMapper;
    public PlayedProcessor(ILogger<PlayedProcessor> logger,
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

    protected override async Task HandleEventAsync(Played eventValue, LogEventContext context)
    {   
        if (eventValue.PlayerAddress == null || eventValue.PlayerAddress.Value == null)
        {
            return;
        }

        var index = await _bingoIndexRepository.GetFromBlockStateSetAsync(eventValue.PlayId.ToHex(), context.ChainId);
        if (index != null)
        {
            return;
        }
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
        // _objectMapper.Map<LogEventContext, CAHolderIndex>(context, caHolderIndex);
        var bingoIndex = new BingoGameIndex
        {
            Id = eventValue.PlayId.ToHex(),
            PlayBlockHeight = eventValue.PlayBlockHeight,
            Amount = eventValue.Amount,
            IsComplete = false,
            PlayId = context.TransactionId,
            BingoType = (int)eventValue.Type,
            Dices = new List<int>{},
            PlayerAddress = eventValue.PlayerAddress.ToBase58(),
            PlayTime = context.BlockTime.ToTimestamp().Seconds,
            PlayTransactionFee = feeList,
            PlayBlockHash = context.BlockHash
        };
        _objectMapper.Map<LogEventContext, BingoGameIndex>(context, bingoIndex);
        await _bingoIndexRepository.AddOrUpdateAsync(bingoIndex);
    }
}