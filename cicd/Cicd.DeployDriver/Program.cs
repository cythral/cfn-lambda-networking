using Amazon.CloudFormation;
using Amazon.EC2;

using Cythral.CloudFormation.LambdaNetworking;
using Cythral.CloudFormation.LambdaNetworking.Cicd.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable SA1516

await Host
.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
    services.AddSingleton<IHost, Cythral.CloudFormation.LambdaNetworking.Cicd.DeployDriver.Host>();
    services.AddSingleton<StackDeployer>();
    services.AddSingleton<IAmazonCloudFormation, AmazonCloudFormationClient>();
    services.AddSingleton<IAmazonEC2, AmazonEC2Client>();
    services.AddSingleton<INetworkInterfaceService, NetworkInterfaceService>();
    services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
})
.UseConsoleLifetime()
.Build()
.RunAsync();
