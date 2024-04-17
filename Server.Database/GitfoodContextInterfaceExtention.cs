using Server.Database.Abstract;
using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Database;

public partial class GitfoodContext : DbContext, IDbInfo
{
    public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();
    
    IQueryable<Barcode> IDbInfo.Barcodes => Barcodes;
}
