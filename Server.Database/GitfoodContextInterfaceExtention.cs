using Server.Database.Abstract;
using Microsoft.EntityFrameworkCore;
using Server.Data.Models;
using Server.DataModel;

namespace Server.Database;

public partial class GitfoodContext : DbContext, IDbInfo
{
    async Task<int> IDbInfo.SaveChangesAsync() => await base.SaveChangesAsync();
    void IDbInfo.Update<T>(T entity) where T : class  => base.Update(entity);

    /*public IDbSet<Barcode> GetBarcodesTable()
        => new InternalDbSet<Barcode>(Barcodes);

    public IDbSet<Product> GetProductsTable()
        => new InternalDbSet<Product>(Products);

    public IDbSet<Category> GetCategoriesTable()
        => new InternalDbSet<Category>(Categories);

    public IDbSet<Fridge> GetFridgeTable()
        => new InternalDbSet<Fridge>(Fridges);

    public IDbSet<User> GetUsersTable()
        => new InternalDbSet<User>(Users);

    public IDbSet<FridgeUnit> GetFridgeUnitTable()
        => new InternalDbSet<FridgeUnit>(FridgeUnits);*/

    public IDbSet<AddCategoriesRequest> GetAddCategoriesRequestTable()
        => new InternalDbSet<AddCategoriesRequest>(AddCategoriesRequests);

    public IDbSet<Category> GetCategoriesTable()
        => new InternalDbSet<Category>(Categories);

    public IDbSet<FoodCategory> GetFoodCategoriesTable()
        => new InternalDbSet<FoodCategory>(FoodCategories);

    public IDbSet<Fridge> GetFridgeTable()
        => new InternalDbSet<Fridge>(Fridges);

    public IDbSet<FridgeProduct> GetFridgeProductTable()
        => new InternalDbSet<FridgeProduct>(FridgeProducts);

    public IDbSet<Product> GetProductsTable()
        => new InternalDbSet<Product>(Products);

    public IDbSet<ReciepesCategory> GetReciepesCategoriesTable()
        => new InternalDbSet<ReciepesCategory>(ReciepesCategories);

    public IDbSet<Recipe> GetRecipesTable()
        => new InternalDbSet<Recipe>(Recipes);

    public IDbSet<RecipeChild> GetRecipesChildTable()  
        => new InternalDbSet<RecipeChild>(RecipeChildren);

    public IDbSet<RecipesComment> GetRecipesCommentsTable()
        => new InternalDbSet<RecipesComment>(RecipesComments);

    public IDbSet<RecipesLike> GetRecipesLikesTable()
        => new InternalDbSet<RecipesLike>(RecipesLikes);

    public IDbSet<RecipiesImage> GetRecipesImagesTable()
        => new InternalDbSet<RecipiesImage>(RecipiesImages);

    public IDbSet<RecipiesIngredient> GetRecipesIngredientsTable()
        => new InternalDbSet<RecipiesIngredient>(RecipiesIngredients);

    public IDbSet<ShoppingList> GetShoppingListTable()
        => new InternalDbSet<ShoppingList>(ShoppingLists);

    public IDbSet<ShoppingListProduct> GetShoppingListProductTable()
        => new InternalDbSet<ShoppingListProduct>(ShoppingListProducts);

    public IDbSet<User> GetUsersTable() 
        => new InternalDbSet<User>(Users);

    public IDbSet<UsersFollow> GetUsersFollowTable()    
        => new InternalDbSet<UsersFollow>(UsersFollows);
}
