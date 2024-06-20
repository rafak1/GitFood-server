using Microsoft.EntityFrameworkCore;
using Npgsql;
using Server.Logic.Abstract;

namespace Server.Logic;

internal class DatabaseExceptionHandler : IDatabaseErrorHanlder
{
    private const string _entityConcurrentUpdate = "Given element was modified on another workstation";
    private const string _entityWithSamePropertiesExists = "Element with same properties already exists";
    private const string _propertieViolation = "Some of data violates the properties checks";
    private const string _notNullViolation = "Some of required data were not provided";

    public string HandleSqlExceptions(Exception ex)
    {
        if (ex is DbUpdateConcurrencyException)
        {
            return _entityConcurrentUpdate;
        }
        PostgresException postgresException = null;
        if (ex is PostgresException)
            postgresException = ex as PostgresException;
        else if (ex is DbUpdateException dbUpdateEx && dbUpdateEx.InnerException is PostgresException)
            postgresException = dbUpdateEx.InnerException as PostgresException;
        else
            throw ex;
        var message = postgresException.SqlState switch 
        {
            PostgresErrorCodes.NullValueNotAllowed => _notNullViolation,
            PostgresErrorCodes.UniqueViolation => _entityWithSamePropertiesExists,
            PostgresErrorCodes.CheckViolation or
                PostgresErrorCodes.RestrictViolation or
                PostgresErrorCodes.ForeignKeyViolation => _propertieViolation,
            _ => null
        };
        // We want to know for now what it was and only there will be returned Internal error
        if(message is null)
            throw ex;

        return message;
    }
}