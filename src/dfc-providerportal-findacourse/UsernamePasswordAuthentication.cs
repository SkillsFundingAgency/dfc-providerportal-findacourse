using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.FindACourse
{
    public class UsernamePasswordAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class UsernamePasswordAuthenticationDefaults
    {
        public const string AuthenticationScheme = "UsernamePassword";
    }

    public class UsernamePasswordAuthenticationHandler : AuthenticationHandler<UsernamePasswordAuthenticationOptions>
    {
        private const string AuthenticationType = "UsernamePassword";
        private const string UsernameHeader = "UserName";
        private const string PasswordHeader = "Password";

        public UsernamePasswordAuthenticationHandler(
            IOptionsMonitor<UsernamePasswordAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var username = Request.Headers[UsernameHeader].ToString();
            var password = Request.Headers[PasswordHeader].ToString();

            var isAuthenticated = username == Options.Username && password == Options.Password;

            AuthenticateResult result;
            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(AuthenticationType));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                result = AuthenticateResult.Success(ticket);
            }
            else
            {
                Logger.LogWarning($"Login failed for {username}");

                result = AuthenticateResult.Fail("Username or password is incorrect.");
            }

            return Task.FromResult(result);
        }
    }
}
