using Server.Data.Models;
using Server.DataModel;

namespace Server.Database.Abstract;

public interface IDbTables
{
    IDbSet<AddCategoriesRequest> GetAddCategoriesRequestTable();
    IDbSet<Category> GetCategoriesTable();
    IDbSet<FoodCategory> GetFoodCategoriesTable();
    IDbSet<Fridge> GetFridgeTable();
    IDbSet<FridgeProduct> GetFridgeProductTable();
    IDbSet<Product> GetProductsTable();
    IDbSet<ReciepesCategory> GetReciepesCategoriesTable();
    IDbSet<Recipe> GetRecipesTable();
    IDbSet<RecipeChild> GetRecipesChildTable();
    IDbSet<RecipesComment> GetRecipesCommentsTable();
    IDbSet<RecipesLike> GetRecipesLikesTable();
    IDbSet<RecipiesImage> GetRecipesImagesTable();
    IDbSet<RecipiesIngredient> GetRecipesIngredientsTable();
    IDbSet<ShoppingList> GetShoppingListTable();
    IDbSet<ShoppingListProduct> GetShoppingListProductTable();
    IDbSet<User> GetUsersTable();
    IDbSet<UsersFollow> GetUsersFollowTable();
}