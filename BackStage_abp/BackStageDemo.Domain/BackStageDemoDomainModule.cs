using System;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace BackStageDemo.Domain
{
    [DependsOn(
        typeof(AbpDddDomainModule)
        )]
    public class BackStageDemoDomainModule:AbpModule
    {
    }
}
