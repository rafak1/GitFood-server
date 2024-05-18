using Server.Data.Models;
using Server.ViewModels;
using Server.ViewModels.Categories;

namespace Server.Logic.Abstract.Managers;

public interface ICategoryManager
{
    public Task<IManagerActionResult<int>> AddNewCategoryRequestAsync(CategoryViewModel category, string user);
    public Task<IManagerActionResult<Category[]>> GetCategoriesAsync();
    public Task<IManagerActionResult<Category[]>> GetVerifiedCategoriesAsync();
    public Task<IManagerActionResult> DeleteCategoryAsync(int id);
    public Task<IManagerActionResult<string[]>> GetUnitsAsync();
    public Task<IManagerActionResult<Category[]>> GetSuggestionsAsync(string name, int resultsCount);
}