using System.Collections.Generic;
using System.Linq;

using Amazon.CDK.Assertions;

using Constructs;

using FluentAssertions;

using NUnit.Framework;

namespace Cythral.CloudFormation.LambdaNetworking;

[Category("Unit")]
public class LambdaNetworkingStackTests
{
    [Test, Auto]
    public void ShouldCreateAnElasticIPForEachInterface(
        string networkInterface1,
        string networkInterface2,
        string networkInterface3,
        string networkInterface4,
        Construct construct,
        LambdaNetworkingStackProps props
    )
    {
        props.NetworkInterfaceIds = new[] { networkInterface1, networkInterface2, networkInterface3, networkInterface4 };
        var stack = new LambdaNetworkingStack(construct, props);
        var template = Template.FromStack(stack);
        template.ResourceCountIs("AWS::EC2::EIP", 4);
    }

    [Test, Auto]
    public void ShouldCreateAnEIPAssociationForEachInterface(
        string networkInterface1,
        string networkInterface2,
        string networkInterface3,
        string networkInterface4,
        Construct construct,
        LambdaNetworkingStackProps props
    )
    {
        props.NetworkInterfaceIds = new[] { networkInterface1, networkInterface2, networkInterface3, networkInterface4 };
        var stack = new LambdaNetworkingStack(construct, props);
        var template = Template.FromStack(stack);
        template.ResourceCountIs("AWS::EC2::EIPAssociation", 4);
    }

    [Test, Auto]
    public void EIPAssociationsShouldReferenceTheirRespectiveEIP(
        string networkInterface1,
        string networkInterface2,
        Construct construct,
        LambdaNetworkingStackProps props
    )
    {
        static (object, object) GetAssociationEip(IDictionary<string, IDictionary<string, object>> resources, int index)
        {
            var firstAssociation = resources.ElementAt(index).Value;
            dynamic firstAssociationProps = firstAssociation["Properties"];
            var att = firstAssociationProps["AllocationId"]["Fn::GetAtt"];
            return (att[0], att[1]);
        }

        props.NetworkInterfaceIds = new[] { networkInterface1, networkInterface2 };
        var stack = new LambdaNetworkingStack(construct, props);
        var template = Template.FromStack(stack);
        var associations = template.FindResources("AWS::EC2::EIPAssociation");

        var (eip1, att1) = GetAssociationEip(associations, 0);
        eip1.Should().Be("LambdaElasticIP0");
        att1.Should().Be("AllocationId");

        var (eip2, att2) = GetAssociationEip(associations, 1);
        eip2.Should().Be("LambdaElasticIP1");
        att2.Should().Be("AllocationId");
    }

    [Test, Auto]
    public void EIPAssociationsShouldReferenceTheirNetworkInterfaceId(
        string networkInterface1,
        string networkInterface2,
        Construct construct,
        LambdaNetworkingStackProps props
    )
    {
        static object GetNetworkInterfaceId(IDictionary<string, IDictionary<string, object>> resources, int index)
        {
            var firstAssociation = resources.ElementAt(index).Value;
            dynamic firstAssociationProps = firstAssociation["Properties"];
            return firstAssociationProps["NetworkInterfaceId"];
        }

        props.NetworkInterfaceIds = new[] { networkInterface1, networkInterface2 };
        var stack = new LambdaNetworkingStack(construct, props);
        var template = Template.FromStack(stack);
        var associations = template.FindResources("AWS::EC2::EIPAssociation");

        var eip1 = GetNetworkInterfaceId(associations, 0);
        eip1.Should().Be(networkInterface1);

        var eip2 = GetNetworkInterfaceId(associations, 1);
        eip2.Should().Be(networkInterface2);
    }
}
