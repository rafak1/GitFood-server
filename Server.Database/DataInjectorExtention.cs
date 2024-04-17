using Microsoft.Extensions.DependencyInjection;

namespace Server.Database;

public static class DataInjectorExtnetion 
{
    public static IServiceCollection AddDatabase(this IServiceCollection injector) 
    {
        return injector.AddScoped<GitfoodContext>();
    }
}