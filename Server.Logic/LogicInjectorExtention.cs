using Server.Logic.Abstract.Managers;
using Server.Logic.Managers;
using Server.Logic.Abstract.Authentication;
using Server.Logic.Authentication;
using Server.Logic.Abstract.Token;
using Server.Logic.Token;
using Microsoft.Extensions.DependencyInjection;
using Server.Logic.Abstract;
using Server.Logic.Abstract.Email;
using Server.Logic.Email;

namespace Server.Logic;

public static class LogicInjectorExtention
{
    public static IServiceCollection AddLogic(this IServiceCollection injector) 
    {
        return injector.AddBasicLogic()
            .AddManagers()
            .AddAuthentication()
            .AddToken()
            .AddEmail();
    }

    private static IServiceCollection AddBasicLogic(this IServiceCollection injector)
    {
        return injector.AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IPathProvider, PathProvider>()
            .AddSingleton<IFileSaver, FileSaver>()
            .AddSingleton<IRecipeViewModelFactory, RecipeViewModelFactory>();
    }

    private static IServiceCollection AddManagers(this IServiceCollection injector)
    {
        return injector.AddScoped<IProductManager, ProductManager>()
            .AddScoped<ICategoryManager, CategoryManager>()
            .AddScoped<IFridgeManager, FridgeManager>()
            .AddScoped<ILoginManager, LoginManager>()
            .AddScoped<IRecipeManager, RecipeManager>()
            .AddScoped<IFollowerManager, FollowerManager>()
            .AddScoped<IFoodCategoryManager, FoodCategoryManager>()
            .AddScoped<IPageingManager, PageingManager>()
            .AddScoped<IShoppingListManager, ShoppingListManager>();

    }

    private static IServiceCollection AddAuthentication(this IServiceCollection injector)
    {
        return injector.AddSingleton<IPasswordChecker, PasswordChecker>()
            .AddSingleton<ITokenGenerator, TokenGenerator>()
            .AddSingleton<ITokenConfigProvider, TokenConfigProvider>();
    }

    private static IServiceCollection AddToken(this IServiceCollection injector)
    {
        return injector.AddSingleton<ITokenStorage, TokenStorage>();
    }

    private static IServiceCollection AddEmail(this IServiceCollection injector)
    {
        return injector.AddScoped<IEmailManager, EmailManager>();
    }
}