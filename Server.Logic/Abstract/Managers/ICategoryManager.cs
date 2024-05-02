using Server.Data.Models;
using Server.ViewModels;
using Server.ViewModels.Categories;

namespace Server.Logic.Abstract.Managers;

public interface ICategoryManager
{
    public Task<IManagerActionResult> AddNewCategoryRequestAsync(CategoryViewModel category, string user);
    public Task<IManagerActionResult<Category[]>> GetCategoriesAsync();
    public Task<IManagerActionResult<Category[]>> GetVerifiedCategoriesAsync();
    public Task<IManagerActionResult> DeleteCategoryAsync(int id);
}