using Server.Data.Models;

namespace Server.Logic.Abstract.Managers;

public interface IFoodCategoryManager
{
    public Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync();
}
