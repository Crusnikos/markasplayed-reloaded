using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MarkAsPlayed.Api.Tests;

public class IntegrationTest : WebApplicationFactory<Program>
{
    private readonly TestConfiguration _configuration;

    public IntegrationTest()
    {
        _configuration = TestConfiguration.Create();
    }
}
