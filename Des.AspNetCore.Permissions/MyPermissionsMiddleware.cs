using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Des.AspNetCore.Permissions
{
    internal class MyPermissionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPermissionPolicyProvider _permissions;

        public MyPermissionsMiddleware(RequestDelegate next, IPermissionPolicyProvider permissions)
        {
            _next = next;
            _permissions = permissions;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var result = await _permissions.GetPermissionsAsync(context.User);

            // set up the headers
            context.Response.ContentType = "application/json";
            var headers = context.Response.Headers;
            headers[HeaderNames.CacheControl] = "no-store, no-cache";
            headers[HeaderNames.Pragma] = "no-cache";
            headers[HeaderNames.Expires] = "Thu, 01 Jan 1970 00:00:00 GMT";
            context.Response.StatusCode = StatusCodes.Status200OK;

            var response = JsonSerializer.Serialize(result);

            await context.Response.WriteAsync(response);
        }
    }
}