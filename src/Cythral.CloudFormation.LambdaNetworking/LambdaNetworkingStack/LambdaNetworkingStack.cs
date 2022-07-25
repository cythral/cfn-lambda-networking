using System.Linq;

using Amazon.CDK;
using Amazon.CDK.AWS.EC2;

using Constructs;

namespace Cythral.CloudFormation.LambdaNetworking
{
    /// <summary>
    /// Stack that contains repositories for storing artifacts.
    /// </summary>
    public class LambdaNetworkingStack : Stack
    {
        private readonly LambdaNetworkingStackProps props;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaNetworkingStack"/> class.
        /// </summary>
        /// <param name="scope">CDK Scope to use for the stack.</param>
        /// <param name="props">Stack properties to pass.</param>
        public LambdaNetworkingStack(
            Construct scope,
            LambdaNetworkingStackProps props
        )
            : base(scope, "LambdaNetworkingSupport", props)
        {
            this.props = props;

            Generate();
        }

        private void Generate()
        {
            for (var index = 0; index < props.NetworkInterfaceIds.Count(); index++)
            {
                var interfaceId = props.NetworkInterfaceIds.ElementAt(index);
                AddElasticIP(index, interfaceId);
            }
        }

        private CfnEIP AddElasticIP(int index, string interfaceId)
        {
            var eip = new CfnEIP(this, $"LambdaElasticIP{index}", new CfnEIPProps());
            _ = new CfnEIPAssociation(this, $"LambdaElasticIPAssociation{index}", new CfnEIPAssociationProps
            {
                AllocationId = eip.GetAtt("AllocationId").ToString(),
                NetworkInterfaceId = interfaceId,
            });

            return eip;
        }
    }
}
