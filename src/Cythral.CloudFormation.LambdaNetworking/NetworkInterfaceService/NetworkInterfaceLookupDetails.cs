using System;
using System.Collections.Generic;

namespace Cythral.CloudFormation.LambdaNetworking;

/// <summary>
/// Details to be used to lookup a lambda network interface.
/// </summary>
public class NetworkInterfaceLookupDetails
{
    /// <summary>
    /// Gets or sets the security group id that will be used to lookup network interfaces.
    /// </summary>
    public string SecurityGroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a list of subnet ids that will be used to lookup network interfaces.
    /// </summary>
    public IEnumerable<string> SubnetIds { get; set; } = Array.Empty<string>();
}
