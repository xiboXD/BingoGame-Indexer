using AElf.CSharp.Core;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Portkey.Contracts.CA;
using BingoGame.Indexer.CA.Entities;
using Volo.Abp.ObjectMapping;

namespace BingoGame.Indexer.CA.Processors;

public abstract class CAHolderTransactionProcessorBase<TEvent> : AElfLogEventProcessorBase<TEvent, TransactionInfo>
    where TEvent : IEvent<TEvent>, new()
{
    protected readonly IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo> CAHolderIndexRepository;

    protected readonly IAElfIndexerClientEntityRepository<CAHolderManagerIndex, LogEventInfo>
        CAHolderManagerIndexRepository;

    protected readonly IAElfIndexerClientEntityRepository<CAHolderTransactionIndex, TransactionInfo>
        CAHolderTransactionIndexRepository;

    protected readonly IAElfIndexerClientEntityRepository<CAHolderTransactionAddressIndex, TransactionInfo>
        CAHolderTransactionAddressIndexRepository;

    protected readonly ContractInfoOptions ContractInfoOptions;
    protected readonly CAHolderTransactionInfoOptions CAHolderTransactionInfoOptions;
    protected readonly IObjectMapper ObjectMapper;

    protected CAHolderTransactionProcessorBase(ILogger<CAHolderTransactionProcessorBase<TEvent>> logger,
        IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo> caHolderIndexRepository,
        IAElfIndexerClientEntityRepository<CAHolderManagerIndex, LogEventInfo> caHolderManagerIndexRepository,
        IAElfIndexerClientEntityRepository<CAHolderTransactionIndex, TransactionInfo>
            caHolderTransactionIndexRepository,
        IAElfIndexerClientEntityRepository<CAHolderTransactionAddressIndex, TransactionInfo>
            caHolderTransactionAddressIndexRepository,
        IOptionsSnapshot<ContractInfoOptions> contractInfoOptions,
        IOptionsSnapshot<CAHolderTransactionInfoOptions> caHolderTransactionInfoOptions,
        IObjectMapper objectMapper) : base(logger)
    {
        CAHolderIndexRepository = caHolderIndexRepository;
        CAHolderManagerIndexRepository = caHolderManagerIndexRepository;
        CAHolderTransactionIndexRepository = caHolderTransactionIndexRepository;
        ContractInfoOptions = contractInfoOptions.Value;
        CAHolderTransactionInfoOptions = caHolderTransactionInfoOptions.Value;
        ObjectMapper = objectMapper;
        CAHolderTransactionAddressIndexRepository = caHolderTransactionAddressIndexRepository;
    }

    protected bool IsValidTransaction(string chainId, string to, string methodName, string parameter)
    {
        if (!CAHolderTransactionInfoOptions.CAHolderTransactionInfos.Where(t => t.ChainId == chainId).Any(t =>
                t.ContractAddress == to && t.MethodName == methodName &&
                t.EventNames.Contains(GetEventName()))) return false;
        if (methodName == "ManagerForwardCall" &&
            !IsValidManagerForwardCallTransaction(chainId, to, methodName, parameter)) return false;
        return true;
    }

    private bool IsValidManagerForwardCallTransaction(string chainId, string to, string methodName, string parameter)
    {
        if (methodName != "ManagerForwardCall") return false;
        if (to != ContractInfoOptions.ContractInfos.First(c => c.ChainId == chainId).CAContractAddress) return false;
        var managerForwardCallInput = ManagerForwardCallInput.Parser.ParseFrom(ByteString.FromBase64(parameter));
        return IsValidTransaction(chainId, managerForwardCallInput.ContractAddress.ToBase58(),
            managerForwardCallInput.MethodName, managerForwardCallInput.Args.ToBase64());
    }

    protected string GetMethodName(string methodName, string parameter)
    {
        if (methodName == "ManagerTransfer") return "Transfer";
        if (methodName != "ManagerForwardCall") return methodName;
        var managerForwardCallInput = ManagerForwardCallInput.Parser.ParseFrom(ByteString.FromBase64(parameter));
        return GetMethodName(managerForwardCallInput.MethodName, managerForwardCallInput.Args.ToBase64());
    }

    protected Dictionary<string, long> GetTransactionFee(Dictionary<string, string> extraProperties)
    {
        var feeMap = new Dictionary<string, long>();
        if (extraProperties.TryGetValue("TransactionFee", out var transactionFee))
        {
            feeMap = JsonConvert.DeserializeObject<Dictionary<string, long>>(transactionFee) ??
                     new Dictionary<string, long>();
        }

        if (extraProperties.TryGetValue("ResourceFee", out var resourceFee))
        {
            var resourceFeeMap = JsonConvert.DeserializeObject<Dictionary<string, long>>(resourceFee) ??
                                 new Dictionary<string, long>();
            foreach (var (symbol, fee) in resourceFeeMap)
            {
                if (feeMap.TryGetValue(symbol, out _))
                {
                    feeMap[symbol] += fee;
                }
                else
                {
                    feeMap[symbol] = fee;
                }
            }
        }

        return feeMap;
    }

    protected async Task AddCAHolderTransactionAddressAsync(string caAddress, string address, string addressChainId,
        LogEventContext context)
    {
        var id = IdGenerateHelper.GetId(context.ChainId, caAddress, address, addressChainId);
        var caHolderTransactionAddressIndex =
            await CAHolderTransactionAddressIndexRepository.GetFromBlockStateSetAsync(id, context.ChainId);
        if (caHolderTransactionAddressIndex == null)
        {
            caHolderTransactionAddressIndex = new CAHolderTransactionAddressIndex
            {
                Id = id,
                CAAddress = caAddress,
                Address = address,
                AddressChainId = addressChainId
            };
        }

        var transactionTime = context.BlockTime.ToTimestamp().Seconds;
        if (caHolderTransactionAddressIndex.TransactionTime >= transactionTime) return;
        caHolderTransactionAddressIndex.TransactionTime = transactionTime;
        ObjectMapper.Map(context, caHolderTransactionAddressIndex);
        await CAHolderTransactionAddressIndexRepository.AddOrUpdateAsync(caHolderTransactionAddressIndex);
    }

    protected async Task<string> ProcessCAHolderTransactionAsync(LogEventContext context, string caAddress)
    {
        if (!IsValidTransaction(context.ChainId, context.To, context.MethodName, context.Params)) return null;
        var holder = await CAHolderIndexRepository.GetFromBlockStateSetAsync(IdGenerateHelper.GetId(context.ChainId,
            caAddress), context.ChainId);
        if (holder == null) return null;

        var index = new CAHolderTransactionIndex
        {
            Id = IdGenerateHelper.GetId(context.BlockHash, context.TransactionId),
            Timestamp = context.BlockTime.ToTimestamp().Seconds,
            FromAddress = caAddress,
            TransactionFee = GetTransactionFee(context.ExtraProperties)
        };
        ObjectMapper.Map(context, index);
        index.MethodName = GetMethodName(context.MethodName, context.Params);

        await CAHolderTransactionIndexRepository.AddOrUpdateAsync(index);

        return holder.CAAddress;
    }
}