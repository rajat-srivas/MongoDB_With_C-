//using Amazon.Runtime.Internal;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using static MongoDB.Operations.Middleware.Authentication.ApiKeyAuthenticationHandler;

//namespace MongoDB.Operations.Middleware.Authentication
//{
//	public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
//	{
//		private const string ApiKeyHeaderName = "X-API-KEY";

//		public ApiKeyAuthenticationHandler(
//			IOptionsMonitor<ApiKeyAuthenticationOptions> options,
//		ILoggerFactory logger,
//			UrlEncoder encoder,
//			ISystemClock clock)
//			: base(options, logger, encoder, clock)
//		{
//		}

//		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//		{
//			if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
//			{
//				return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
//			}

//			var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

//			if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
//			{
//				return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
//			}

//			// Validate the API Key
//			var existingApiKey = "your_predefined_api_key"; // This should be replaced with actual logic to validate the API key
			
//			if (existingApiKey.Equals(providedApiKey))
//			{
//				var claims = new List<Claim> { new Claim(ClaimTypes.Name, "API Key User") };
//				var identity = new ClaimsIdentity(claims, Options.AuthenticationScheme);
//				var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Options.Scheme);

//				return Task.FromResult(AuthenticateResult.Success(ticket));
//			}

//			return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
//		}

//		public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
//		{
//		}

//	}
//}

