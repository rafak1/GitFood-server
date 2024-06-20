
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public class UserErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    private const string _userNotFound = "No user found assigned to this token";

    public UserErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserNotFoundException)
        {                                                                           
            context.Response.ContentType = "application/json";                                                                                         
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(_userNotFound));
        }
    }
}