using MarkAsPlayed.Api.Modules.Article.Models;

namespace MarkAsPlayed.Api.Tests.Modules.Article;

public class ArticleSharedTestData
{
    public class MalformedArticleData : TheoryData<ArticleCreationRequest>
    {
        public MalformedArticleData()
        {
            Add(new ArticleCreationRequest{ });
        }
    }
}
