// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MegaTestApp.HttpClientTest;
using Microsoft.AspNetCore.BrowserTesting;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Testing;
using PlaywrightSharp;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.Tests
{
    public class BinaryHttpClientTest : ComponentBrowserTestBase,
        IClassFixture<BasicTestAppServerSiteFixture<CorsStartup>>,
        IClassFixture<BlazorWasmTestAppFixture<MegaTestApp.Program>>
    {
        private readonly BlazorWasmTestAppFixture<MegaTestApp.Program> _devHostServerFixture;
        readonly ServerFixture _apiServerFixture;

        protected override Type TestComponent { get; } = typeof(BinaryHttpRequestsComponent);

        public BinaryHttpClientTest(
            BlazorWasmTestAppFixture<MegaTestApp.Program> devHostServerFixture,
            BasicTestAppServerSiteFixture<CorsStartup> apiServerFixture,
            ITestOutputHelper output)
            : base(output)
        {
            _devHostServerFixture = devHostServerFixture;
            _devHostServerFixture.PathBase = "/subdir";
            _apiServerFixture = apiServerFixture;
            MountUri = _devHostServerFixture.RootUri + "subdir";
        }

        [Theory]
        [InlineData(BrowserKind.Chromium)]
        [InlineData(BrowserKind.Firefox)]
        [InlineData(BrowserKind.Webkit)]
        // NOTE: BrowserKind argument must be first
        public async Task CanSendAndReceiveBytes(BrowserKind browserKind)
        {
            if (ShouldSkip(browserKind)) 
            {
                Assert.Equal("Skipped", browserKind.ToString());
                return;
            }

            var targetUri = new Uri(_apiServerFixture.RootUri, "/subdir/api/data");
            await TestPage.TypeAsync("#request-uri", targetUri.AbsoluteUri);
            await TestPage.ClickAsync("#send-request");

            var status = await TestPage.GetTextContentAsync("#response-status");
            var statusText = await TestPage.GetTextContentAsync("#response-status-text");
            var testOutcome = await TestPage.GetTextContentAsync("#test-outcome");

            Assert.Equal("OK", status);
            Assert.Equal("OK", statusText);
            Assert.Equal("", testOutcome);
        }
    }
}
