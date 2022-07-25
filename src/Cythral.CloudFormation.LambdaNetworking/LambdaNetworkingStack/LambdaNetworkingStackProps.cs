using System;
using System.Collections.Generic;

using Amazon.CDK;

namespace Cythral.CloudFormation.LambdaNetworking
{
    /// <summary>
    /// Properties that can be passed to the lambda networking stack.
    /// </summary>
    public class LambdaNetworkingStackProps : StackProps
    {
        /// <summary>
        /// Gets or sets a list of lambda network interface IDs.
        /// </summary>
        public IEnumerable<string> NetworkInterfaceIds { get; set; } = Array.Empty<string>();
    }
}
