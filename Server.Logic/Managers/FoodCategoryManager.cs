using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Logic.Abstract;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Logic.Managers;

internal class FoodCategoryManager : IFoodCategoryManager
{
    private readonly GitfoodContext _dbInfo;

    public FoodCategoryManager(GitfoodContext dbInfo)
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync()
    {
        var categories = await _dbInfo.FoodCategories.ToArrayAsync();
        return new ManagerActionResult<FoodCategory[]>(categories, ResultEnum.OK);
    }
}
