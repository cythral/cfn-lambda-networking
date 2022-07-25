using System;
using System.Threading;
using System.Threading.Tasks;

using Cythral.CloudFormation.LambdaNetworking.Cicd.Utils;

using Microsoft.Extensions.Hosting;

namespace Cythral.CloudFormation.LambdaNetworking.Cicd.DeployDriver
{
    /// <inheritdoc />
    public class Host : IHost
    {
        private readonly StackDeployer deployer;
        private readonly ITemplateRenderer renderer;
        private readonly IHostApplicationLifetime lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host" /> class.
        /// </summary>
        /// <param name="deployer">Service for deploying cloudformation stacks.</param>
        /// <param name="renderer">Service for rendering the cloudformation template.</param>
        /// <param name="lifetime">Service that controls the application lifetime.</param>
        /// <param name="serviceProvider">Object that provides access to the program's services.</param>
        public Host(
            StackDeployer deployer,
            ITemplateRenderer renderer,
            IHostApplicationLifetime lifetime,
            IServiceProvider serviceProvider
        )
        {
            this.deployer = deployer;
            this.renderer = renderer;
            this.lifetime = lifetime;
            Services = serviceProvider;
        }

        /// <inheritdoc />
        public IServiceProvider Services { get; }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var template = string.Empty;

            await Step("Generate template", async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                template = await renderer.Render(cancellationToken);
            });

            await Step("Deploy template", async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var request = new DeployContext
                {
                    StackName = "cfn-lambda-networking",
                    TemplateBody = template,
                };

                await deployer.Deploy(request, cancellationToken);
            });

            lifetime.StopApplication();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private static async Task Step(string title, Func<Task> action)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{title} ==========\n");
            Console.ResetColor();

            await action();
        }
    }
}
