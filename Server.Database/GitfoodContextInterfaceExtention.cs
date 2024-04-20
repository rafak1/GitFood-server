using Server.Database.Abstract;
using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Database;

public partial class GitfoodContext : DbContext, IDbInfo
{
    async Task<int> IDbInfo.SaveChangesAsync() => await base.SaveChangesAsync();
    void IDbInfo.Update<T>(T entity) where T : class  => base.Update(entity);

    public IDbSet<Barcode> GetBarcodesTable()
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
        => new InternalDbSet<FridgeUnit>(FridgeUnits);
}
