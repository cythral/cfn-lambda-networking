using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Cythral.CloudFormation.LambdaNetworking
{
    /// <inheritdoc />
    public class NetworkInterfaceService : INetworkInterfaceService
    {
        private readonly IAmazonEC2 ec2;
        private readonly IAmazonCloudFormation cloudFormation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInterfaceService"/> class.
        /// </summary>
        /// <param name="ec2">Service for interacting with the EC2 API.</param>
        /// <param name="cloudFormation">Service for interacting with the CloudFormation APi.</param>
        public NetworkInterfaceService(
            IAmazonEC2 ec2,
            IAmazonCloudFormation cloudFormation
        )
        {
            this.ec2 = ec2;
            this.cloudFormation = cloudFormation;
        }

        /// <inheritdoc />
        public async Task<NetworkInterfaceLookupDetails> GetLookupDetails(CancellationToken cancellationToken)
        {
            var request = new DescribeStacksRequest { StackName = "cfn-utilities" };
            var result = await cloudFormation.DescribeStacksAsync(request, cancellationToken);
            var outputs = result.Stacks.First().Outputs;
            var subnetIds = GetOutputValue(outputs, "SubnetIds");

            return new NetworkInterfaceLookupDetails
            {
                SubnetIds = subnetIds.Split(','),
                SecurityGroupId = GetOutputValue(outputs, "LambdaSecurityGroupId"),
            };
        }

        /// <inheritdoc />
        public async Task<string> GetPrivateIpAddress(string securityGroupId, string subnetId, CancellationToken cancellationToken)
        {
            var request = new DescribeNetworkInterfacesRequest
            {
                Filters = new List<Filter>()
                {
                    new Filter { Name = "group-id", Values = new List<string> { securityGroupId } },
                    new Filter { Name = "subnet-id", Values = new List<string> { subnetId } },
                },
            };

            var response = await ec2.DescribeNetworkInterfacesAsync(request, cancellationToken);
            return response.NetworkInterfaces.First().NetworkInterfaceId;
        }

        private static string GetOutputValue(List<Output> outputs, string key)
        {
            var query = from output in outputs
                        where output.OutputKey == key
                        select output.OutputValue;

            return query.FirstOrDefault(string.Empty);
        }
    }
}
