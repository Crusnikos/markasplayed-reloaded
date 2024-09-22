using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MarkAsPlayed.Api.Tests
{
    public class TestAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public string NameIdentifier;

        public const string DefaultScheme = "Test";

        public TestAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
            NameIdentifier = configuration["Firebase:TestUserId"]!;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                new[]
                {
                new Claim(ClaimTypes.NameIdentifier, NameIdentifier),
                },
                DefaultScheme
            );

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, DefaultScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
