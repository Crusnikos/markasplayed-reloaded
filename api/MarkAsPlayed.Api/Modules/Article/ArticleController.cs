using MarkAsPlayed.Api.Modules;
using MarkAsPlayed.Api.Modules.Article.Models;
using MarkAsPlayed.Api.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MarkAsPlayed.Api.Models.Article;

[ApiController]
[Route("article")]
public class ArticleController : ControllerBase
{
    /// <summary>
    ///     Retrieves an articles listing
    /// </summary>
    [HttpGet("listing")]
    [ProducesResponseType(typeof(IReadOnlyList<ArticleListingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticleListingAsync(
        [Range(1, int.MaxValue)]
        int? page)
    {
        //insert method
        var response = new Paginated<ArticleListingResponse>(new List<ArticleListingResponse>(), 2, 1);

        Response.Headers.Append("display-page", response.Page.ToString());
        Response.Headers.Append("articles-count", response.Total.ToString());

        return Ok(response.Data);
    }

    /// <summary>
    ///     Creates an article
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateArticleAsync(
        [FromBody] ArticleCreationRequest request, string transactionId)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ?? null;
        if (userId is null || transactionId is null)
        {
            return NotFound();
        }

        //insert method
        var result = new ArticleCreationResult { ArticleIdentifier = 1, ResponseCode = ResponseCode.OK };

        return result.ResponseCode switch
        {
            ResponseCode.NotFound => NotFound(),
            ResponseCode.UnprocessableContent => UnprocessableEntity(),
            ResponseCode.InternalError => Problem(
                statusCode: 500,
                title: "Failed to create article"
                ),
            _ => Ok(result.ArticleIdentifier)
        };
    }
}
