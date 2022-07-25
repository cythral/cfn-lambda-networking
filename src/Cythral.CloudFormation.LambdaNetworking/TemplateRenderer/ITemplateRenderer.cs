using System.Threading;
using System.Threading.Tasks;

namespace Cythral.CloudFormation.LambdaNetworking
{
    /// <summary>
    /// Service for rendering a template.
    /// </summary>
    public interface ITemplateRenderer
    {
        /// <summary>
        /// Renders a CloudFormation template for Lambda Networking.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The template body.</returns>
        Task<string> Render(CancellationToken cancellationToken);
    }
}
