using FluentAssertions;
using Flurl.Http;
using MarkAsPlayed.Api.Modules.Article.Models;
using System.Net;

namespace MarkAsPlayed.Api.Tests.Modules.Article;

public class ArticleCreationEndpointTests : IClassFixture<IntegrationTest>, IAsyncLifetime
{
    private readonly IntegrationTest _suite;

    public ArticleCreationEndpointTests(IntegrationTest suite)
    {
        _suite = suite;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [ClassData(typeof(ArticleSharedTestData.MalformedArticleData))]
    public async Task ShouldSuccessWithMalformedRequest(
        ArticleCreationRequest request)
    {
        var response = await _suite.Client.Request("article").
                                    SetQueryParam("transactionId", Guid.NewGuid()).
                                    PostJsonAsync(request);
        response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }
}
