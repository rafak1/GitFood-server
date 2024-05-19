using Server.Data.Models;
using Server.ViewModels;
using Server.ViewModels.FoodCategories;

namespace Server.Logic.Abstract.Managers;

public interface IFoodCategoryManager
{
    public Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync();
    public Task<IManagerActionResult<int>> AddFoodCategoryAsync(string name, string description, string user);
    public Task<IManagerActionResult> RemoveFoodCategoryAsync(int foodCategoryId, string user);
    public Task<IManagerActionResult<IdExtendedViewModel<FoodCategoryViewModel>[]>> GetFoodCategorySuggestionsAsync(string name, int resultsCount);
}
