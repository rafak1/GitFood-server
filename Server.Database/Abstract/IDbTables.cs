using Server.Data.Models;

namespace Server.Database.Abstract;

public interface IDbTables
{
    IDbSet<Barcode> GetBarcodesTable();
    IDbSet<Product> GetProductsTable();
    IDbSet<Category> GetCategoriesTable();
    IDbSet<Fridge> GetFridgeTable();
    IDbSet<User> GetUsersTable();
    IDbSet<FridgeUnit> GetFridgeUnitTable();
}