using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Authorization.Adapter.BasicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const int CountCredentials = 2;
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(IConfiguration configuration,
                                            IOptionsMonitor<AuthenticationSchemeOptions> options,
                                            ILoggerFactory logger,
                                            UrlEncoder encoder,
                                            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Você não tem permissão");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, CountCredentials);
            var username = credentials[0];
            var password = credentials[1];

            var logged = await HasLoginAsync(username, password)
                                .ConfigureAwait(false);

            if (!logged)
            {
                return AuthenticateResult.Fail("Usuário e/ou senha inválidos");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "CashPlataformAPI"),
                new Claim(ClaimTypes.Name, "cashplataformapi"),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<bool> HasLoginAsync(string username, string password)
        {
            bool login = true;
            var userValid = _configuration.GetSection("BasicAuthentication:User").Value;
            var passValid = _configuration.GetSection("BasicAuthentication:Pass").Value;

            if (username != userValid ||
                    password != passValid)
            {
                login = false;
                throw new Exception($"erro:  userValid: {userValid} passValid: {passValid} / username:{username} password: {password}");
            }

            return await Task.FromResult(login);
        }
    }
}
