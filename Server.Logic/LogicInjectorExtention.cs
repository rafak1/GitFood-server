using Server.Logic.Abstract;
using Server.Logic.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Logic;

public static class LogicInjectorExtention
{
    public static IServiceCollection AddLogic(this IServiceCollection injector) 
    {
        return injector.AddScoped<IBarcodeManager, BarcodeManager>();
    }
}