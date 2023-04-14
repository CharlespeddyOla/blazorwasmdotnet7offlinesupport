using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Text;
using System.Text.Json;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Client.OfflineAuth
{
    public class OfflineBackendHandler : HttpClientHandler
    {        
        private UserAccountService _userAccountService { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _userAccountService = new UserAccountService();
            var path = request.RequestUri.AbsolutePath;
            var method = request.Method;

            if (path == "/users/authenticate" && method == HttpMethod.Post)
            {
                return await authenticate();
            }
            else
            {
                // pass through any requests not handled above
                return await base.SendAsync(request, cancellationToken);
            }

            async Task<HttpResponseMessage> authenticate()
            {
                var bodyJson = await request.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<LoginRequest>(bodyJson);
                var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
                var userSession = await jwtAuthenticationManager.GenerateJwtToken(body.Email, body.Password, body.staffList);
                if (userSession is null)
                    return await unauthorized();
                else
                    return await ok(userSession);
            }

            // helper functions

            async Task<HttpResponseMessage> ok(object body)
            {
                return await jsonResponse(HttpStatusCode.OK, body);
            }

            async Task<HttpResponseMessage> unauthorized()
            {
                return await jsonResponse(HttpStatusCode.Unauthorized, new { message = "Unauthorized" });
            }

            async Task<HttpResponseMessage> jsonResponse(HttpStatusCode statusCode, object content)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                };

                // delay to simulate real api call
                await Task.Delay(500);

                return response;
            }
        }
    }
}
