using Microsoft.Extensions.DependencyInjection;
using Orleans;
using BingoGame.Indexer.TestBase;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace BingoGame.Indexer.Orleans.TestBase;

[DependsOn(typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(BingoGameIndexerTestBaseModule)
    )]
public class BingoGameIndexerOrleansTestBaseModule:AbpModule
{
    private ClusterFixture _fixture;
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        if(_fixture == null)
            _fixture = new ClusterFixture();
        // var fixture = new ClusterFixture();
        context.Services.AddSingleton<ClusterFixture>(_fixture);
        context.Services.AddSingleton<IClusterClient>(sp => _fixture.Cluster.Client);
    }
}