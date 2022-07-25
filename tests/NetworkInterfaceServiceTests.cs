using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon.CloudFormation;
using Amazon.CloudFormation.Model;
using Amazon.EC2;
using Amazon.EC2.Model;

using AutoFixture.NUnit3;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using static NSubstitute.Arg;

namespace Cythral.CloudFormation.LambdaNetworking;

[Category("Unit")]
public class NetworkInterfaceServiceTests
{
    [TestFixture]
    [Category("Unit")]
    public class GetLookupDetailsTests
    {
        [Test, Auto]
        public async Task ShouldRetrieveTheLambdaSecurityGroupId(
            string securityGroupId,
            [Frozen] DescribeStacksResponse response,
            [Frozen] IAmazonCloudFormation cloudformation,
            [Target] NetworkInterfaceService service,
            CancellationToken cancellationToken
        )
        {
            response.Stacks = new List<Stack>
            {
                new Stack
                {
                    Outputs = new List<Output>
                    {
                        new Output { OutputKey = "LambdaSecurityGroupId", OutputValue = securityGroupId },
                    },
                },
            };

            var result = await service.GetLookupDetails(cancellationToken);
            result.SecurityGroupId.Should().Be(securityGroupId);
        }

        [Test, Auto]
        public async Task ShouldRetrieveTheSubnetIds(
           string subnetId1,
           string subnetId2,
           [Frozen] DescribeStacksResponse response,
           [Frozen] IAmazonCloudFormation cloudformation,
           [Target] NetworkInterfaceService service,
           CancellationToken cancellationToken
       )
        {
            response.Stacks = new List<Stack>
            {
                new Stack
                {
                    Outputs = new List<Output>
                    {
                        new Output { OutputKey = "SubnetIds", OutputValue = $"{subnetId1},{subnetId2}" },
                    },
                },
            };

            var result = await service.GetLookupDetails(cancellationToken);
            result.SubnetIds.Should().BeEquivalentTo(new[] { subnetId1, subnetId2 });
        }
    }

    [TestFixture]
    [Category("Unit")]
    public class GetNetworkInterfaceIdTests
    {
        [Test, Auto]
        public async Task ShouldRetrieveTheNetworkInterfaceId(
            string securityGroupId,
            string subnetId,
            [Frozen] DescribeNetworkInterfacesResponse response,
            [Frozen] IAmazonEC2 ec2,
            [Target] NetworkInterfaceService service,
            CancellationToken cancellationToken
        )
        {
            var result = await service.GetPrivateIpAddress(securityGroupId, subnetId, cancellationToken);

            await ec2.Received().DescribeNetworkInterfacesAsync(
                Is<DescribeNetworkInterfacesRequest>(req =>
                    req.Filters.Any(filter => filter.Name == "group-id" && filter.Values[0] == securityGroupId) &&
                    req.Filters.Any(filter => filter.Name == "subnet-id" && filter.Values[0] == subnetId)
                ),
                Is(cancellationToken)
            );
        }
    }
}
