using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using Amazon.CDK;

using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

using Constructs;

using RichardSzalay.MockHttp;

#pragma warning disable EF1001

internal class AutoAttribute : AutoDataAttribute
{
    public AutoAttribute()
        : base(Create)
    {
    }

    public static IFixture Create()
    {
        var fixture = new Fixture();
        var messageHandler = new MockHttpMessageHandler();
        var httpClient = new System.Net.Http.HttpClient(messageHandler) { Timeout = TimeSpan.FromSeconds(1) };
        fixture.Inject(messageHandler);
        fixture.Inject(httpClient);
        fixture.Inject<Construct>(new App());
        fixture.Inject(new CancellationToken(false));
        fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
        fixture.Customize(new SupportMutableValueTypesCustomization());
        fixture.Customizations.Add(new OptionsRelay());
        fixture.Customizations.Add(new TypeOmitter<IDictionary<string, JsonElement>>());
        fixture.Customizations.Add(new TypeOmitter<JsonConverter>());
        fixture.Customizations.Insert(-1, new TargetRelay());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}
