using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Server.Logic;

internal class DatabaseExceptionHandlerBase 
{
    private const string _entityConcurrentUpdate = "Given element was modified on another workstation";
    private const string _entityWithSamePropertiesExists = "Element with same properties already exists";
    private const string _propertieViolation = "Some of data violates the properties checks";
    private const string _notNullViolation = "Some of required data were not provided";

    protected string HandleInnerExceptions(Exception ex)
    {
        if (ex is DbUpdateConcurrencyException)
        {
            return _entityConcurrentUpdate;
        }
        else if (ex is DbUpdateException dbUpdateEx && dbUpdateEx.InnerException is PostgresException postgresException)
        {
            var message = postgresException.SqlState switch 
            {
                PostgresErrorCodes.NullValueNotAllowed => _notNullViolation,
                PostgresErrorCodes.UniqueViolation => _entityWithSamePropertiesExists,
                PostgresErrorCodes.CheckViolation or PostgresErrorCodes.RestrictViolation => _propertieViolation,
                _ => null
            };
            // We want to know for now what it was and only there will be returned Internal error
            if(message is null)
                throw ex;

            return message;
        }
        // Here we don't know what it was so we will rethrow it
        throw ex;
    }
}