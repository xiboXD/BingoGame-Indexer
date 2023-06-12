using AElf;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Nethereum.Hex.HexConvertors.Extensions;
using Portkey.Contracts.BingoGameContract;
using Portkey.Contracts.CA;
using BingoGame.Indexer.CA.Entities;
using BingoGame.Indexer.CA.Processors;
using BingoGame.Indexer.CA.Tests.Helper;
using Shouldly;
using Xunit;

namespace BingoGame.Indexer.CA.Tests.Processors;

public class PlayedLogEventProcessorTests: BingoGameIndexerCATestBase
{
    private readonly IAElfIndexerClientEntityRepository<BingoGameIndex, LogEventInfo> _bingoGameIndexRepository;
    private readonly IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo> _caHolderIndexRepository;

    private readonly IAElfIndexerClientEntityRepository<CAHolderTransactionIndex, LogEventInfo>
        _caHolderTransactionRepository;
    private readonly IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo> _repository;
    
    public PlayedLogEventProcessorTests()
    {
        _bingoGameIndexRepository = GetRequiredService<IAElfIndexerClientEntityRepository<BingoGameIndex, LogEventInfo>>();
        _caHolderTransactionRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<CAHolderTransactionIndex, LogEventInfo>>();
        _repository = GetRequiredService<IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo>>();
        _caHolderIndexRepository = GetRequiredService<IAElfIndexerClientEntityRepository<CAHolderIndex, LogEventInfo>>();
    }
    [Fact]
    public async Task HandlePlayedLogEventAsync_Test(){
        await CreateHolder();
        //step1: create blockStateSet
        const string chainId = "AELF";
        const string blockHash = "3c7c267341e9f097b0886c8a1661bef73d6bb4c30464ad73be714fdf22b09bdd";
        const string previousBlockHash = "9a6ef475e4c4b6f15c37559033bcfdbed34ca666c67b2ae6be22751a3ae171de";
        const string transactionId = "c09b8c142dd5e07acbc1028e5f59adca5b5be93a0680eb3609b773044a852c43";
        const long blockHeight = 200;
        var blockStateSetAdded = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash
        };
        
        var blockStateSetTransaction = new BlockStateSet<TransactionInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash
        };

        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSetAdded, chainId);
        var blockStateSetKeyTransaction = await InitializeBlockStateSetAsync(blockStateSetTransaction, chainId);
                //step2: create logEventInfo
        var bingoed = new Played
        {
            PlayBlockHeight = blockHeight,
            PlayerAddress = Address.FromPublicKey("AAA".HexToByteArray()),       
            Amount = 100000000,
            Type = BingoType.Large,
            PlayId = HashHelper.ComputeFrom("PlayId"),
            Symbol = "ELF",
        };
        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(bingoed.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;
        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            Params = "{ \"to\": \"ca\", \"symbol\": \"ELF\", \"amount\": \"100000000000\" }",
            To = "CAAddress",
            MethodName = "Played",
            ExtraProperties = new Dictionary<string, string>
            {
                { "TransactionFee", "{\"ELF\":\"30000000\"}" },
                { "ResourceFee", "{\"ELF\":\"30000000\"}" }
            },
            BlockTime = DateTime.UtcNow
        };
        var bingoedLogEventProcessor = GetRequiredService<PlayedProcessor>();
        
        await bingoedLogEventProcessor.HandleEventAsync(logEventInfo, logEventContext);
        
        bingoedLogEventProcessor.GetContractAddress(chainId);
        
        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await BlockStateSetSaveDataAsync<TransactionInfo>(blockStateSetKeyTransaction);
        await Task.Delay(2000);

        var bingoGameIndexData = await _bingoGameIndexRepository.GetAsync(HashHelper.ComputeFrom("PlayId").ToHex());
        bingoGameIndexData.ShouldNotBeNull();
        bingoGameIndexData.Amount.ShouldBe(100000000);
        bingoGameIndexData.PlayerAddress.ShouldBe(bingoed.PlayerAddress.ToBase58());
        bingoGameIndexData.PlayTransactionFee[0].Amount.ShouldBe(60000000);
        bingoGameIndexData.PlayTransactionFee[0].Symbol.ShouldBe("ELF");
        bingoGameIndexData.PlayBlockHash.ShouldBe(blockHash);
        bingoGameIndexData.PlayBlockHeight.ShouldBe(blockHeight);
        bingoGameIndexData.ChainId.ShouldBe(chainId);
    }
    private async Task CreateHolder()
    {
        const string chainId = "AELF";
        const string blockHash = "dac5cd67a2783d0a3d843426c2d45f1178f4d052235a907a0d796ae4659103b1";
        const string previousBlockHash = "e38c4fb1cf6af05878657cb3f7b5fc8a5fcfb2eec19cd76b73abb831973fbf4e";
        const string transactionId = "c1e625d135171c766999274a00a7003abed24cfe59a7215aabf1472ef20a2da2";
        const long blockHeight = 100;

        var caHolderCreatedProcessor = GetRequiredService<CAHolderCreatedLogEventProcessor>();

        //step1: create blockStateSet
        var blockStateSet = new BlockStateSet<LogEventInfo>
        {
            BlockHash = blockHash,
            BlockHeight = blockHeight,
            Confirmed = true,
            PreviousBlockHash = previousBlockHash,
        };
        var blockStateSetKey = await InitializeBlockStateSetAsync(blockStateSet, chainId);

        //step2: create logEventInfo
        var caHolderCreated = new CAHolderCreated
        {
            CaHash = HashHelper.ComputeFrom("test@google.com"),
            CaAddress = Address.FromPublicKey("AAA".HexToByteArray()),
            Creator = Address.FromPublicKey("BBB".HexToByteArray()),
            Manager = Address.FromPublicKey("CCC".HexToByteArray()),
            ExtraData = "ExtraData"
        };
        var logEventInfo = LogEventHelper.ConvertAElfLogEventToLogEventInfo(caHolderCreated.ToLogEvent());
        logEventInfo.BlockHeight = blockHeight;
        logEventInfo.ChainId = chainId;
        logEventInfo.BlockHash = blockHash;
        logEventInfo.TransactionId = transactionId;
        var logEventContext = new LogEventContext
        {
            ChainId = chainId,
            BlockHeight = blockHeight,
            BlockHash = blockHash,
            PreviousBlockHash = previousBlockHash,
            TransactionId = transactionId,
            Params = "{ \"to\": \"ca\", \"symbol\": \"ELF\", \"amount\": \"100000000000\" }",
            To = "CAAddress",
            MethodName = "CreatHolderInfo",
            ExtraProperties = new Dictionary<string, string>
            {
                { "TransactionFee", "{\"ELF\":\"30000000\"}" },
                { "ResourceFee", "{\"ELF\":\"30000000\"}" }
            },
            BlockTime = DateTime.UtcNow
        };

        //step3: handle event and write result to blockStateSet
        await caHolderCreatedProcessor.HandleEventAsync(logEventInfo, logEventContext);

        //step4: save blockStateSet into es
        await BlockStateSetSaveDataAsync<LogEventInfo>(blockStateSetKey);
        await Task.Delay(2000);
    }
}