using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon.CDK;

namespace Cythral.CloudFormation.LambdaNetworking
{
    /// <inheritdoc />
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly INetworkInterfaceService networkInterfaceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateRenderer"/> class.
        /// </summary>
        /// <param name="networkInterfaceService">Service for looking up details about network interfaces.</param>
        public TemplateRenderer(
            INetworkInterfaceService networkInterfaceService
        )
        {
            this.networkInterfaceService = networkInterfaceService;
        }

        /// <inheritdoc />
        public async Task<string> Render(CancellationToken cancellationToken)
        {
            var lookupDetails = await networkInterfaceService.GetLookupDetails(cancellationToken);
            var query = from subnet in lookupDetails.SubnetIds
                        select networkInterfaceService.GetPrivateIpAddress(lookupDetails.SecurityGroupId, subnet, cancellationToken);

            var networkInterfaces = await Task.WhenAll(query);
            var app = new App();
            var stack = new LambdaNetworkingStack(app, new LambdaNetworkingStackProps
            {
                NetworkInterfaceIds = networkInterfaces,
                Synthesizer = new BootstraplessSynthesizer(new BootstraplessSynthesizerProps()),
            });

            var result = app.Synth();
            var templatePath = result.Stacks.First().TemplateFullPath;
            return await File.ReadAllTextAsync(templatePath, cancellationToken);
        }
    }
}
