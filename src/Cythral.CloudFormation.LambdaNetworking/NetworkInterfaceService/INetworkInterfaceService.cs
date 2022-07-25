using System.Threading;
using System.Threading.Tasks;

namespace Cythral.CloudFormation.LambdaNetworking;

/// <summary>
/// Service for interacting with network interfaces.
/// </summary>
public interface INetworkInterfaceService
{
    /// <summary>
    /// Retrieves network interface lookup details.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The resulting lookup details.</returns>
    Task<NetworkInterfaceLookupDetails> GetLookupDetails(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a network interface ID by its security group id and subnet id.
    /// </summary>
    /// <param name="securityGroupId">The security group id of the network interface.</param>
    /// <param name="subnetId">The subnet id of the network interface.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The id of the subnet that is in the given <paramref name="subnetId" /> and has security group <paramref name="securityGroupId" />.</returns>
    Task<string> GetPrivateIpAddress(string securityGroupId, string subnetId, CancellationToken cancellationToken);
}
