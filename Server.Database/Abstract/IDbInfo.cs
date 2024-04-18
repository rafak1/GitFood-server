
using Server.Data.Models;

namespace Server.Database.Abstract;

public interface IDbInfo 
{
    Task<int> SaveChangesAsync();
}