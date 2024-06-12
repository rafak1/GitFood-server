
using System.Net;
using Microsoft.AspNetCore.Http;
using Server.Logic.Abstract;
using Newtonsoft.Json;

public class SqlErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabaseErrorHanlder _errorHanlder;

    public SqlErrorHandlingMiddleware(RequestDelegate next, IDatabaseErrorHanlder databaseErrorHanlder)
    {
        _next = next;
        _errorHanlder = databaseErrorHanlder ?? throw new ArgumentNullException(nameof(databaseErrorHanlder));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = _errorHanlder.HandleSqlExceptions(ex);                                                                                 
            context.Response.ContentType = "application/json";                                                                                         
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}