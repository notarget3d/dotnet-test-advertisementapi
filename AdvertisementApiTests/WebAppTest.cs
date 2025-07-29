//using AdvertisementApi.Core;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace AdvertisementApiTests
{
    internal class WebAppTest : WebApplicationFactory<Program>
    {
    }
}
