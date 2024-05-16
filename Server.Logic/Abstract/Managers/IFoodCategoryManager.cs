using Server.Data.Models;

namespace Server.Logic.Abstract.Managers;

public interface IFoodCategoryManager
{
    public Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync();
    public Task<IManagerActionResult<int>> AddFoodCategoryAsync(string name, string description, string user);
    public Task<IManagerActionResult> RemoveFoodCategoryAsync(int foodCategoryId, string user);
}
