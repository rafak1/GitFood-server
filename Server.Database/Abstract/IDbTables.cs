using Server.Data.Models;

namespace Server.Database.Abstract;

public interface IDbTables
{
    IDbSet<AddCategoriesRequest> GetAddCategoriesRequestTable();
    IDbSet<Category> GetCategoriesTable();
    IDbSet<FoodCategory> GetFoodCategoriesTable();
    IDbSet<Fridge> GetFridgeTable();
    IDbSet<FridgeProduct> GetFridgeProductTable();
    IDbSet<Product> GetProductsTable();
    IDbSet<Recipe> GetRecipesTable();
    IDbSet<RecipeChild> GetRecipesChildTable();
    IDbSet<RecipesComment> GetRecipesCommentsTable();
    IDbSet<RecipiesImage> GetRecipesImagesTable();
    IDbSet<RecipiesIngredient> GetRecipesIngredientsTable();
    IDbSet<ShoppingList> GetShoppingListTable();
    IDbSet<ShoppingListProduct> GetShoppingListProductTable();
    IDbSet<User> GetUsersTable();
}