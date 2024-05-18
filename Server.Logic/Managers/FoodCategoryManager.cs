using Server.Logic.Abstract.Managers;
using Server.Database;
using Server.ViewModels.Categories;
using Server.Logic.Abstract;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Server.Logic.Users;

namespace Server.Logic.Managers;

internal class FoodCategoryManager : IFoodCategoryManager
{
    private readonly GitfoodContext _dbInfo;

    public FoodCategoryManager(GitfoodContext dbInfo)
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult<int>> AddFoodCategoryAsync(string name, string description, string user)
    {
        if (Enum.IsDefined(typeof(ElevatedUsers), user))
        {
            var category = new FoodCategory
            {
                Name = name,
                Description = description
            };

            await _dbInfo.FoodCategories.AddAsync(category);
            await _dbInfo.SaveChangesAsync();

            return new ManagerActionResult<int>(category.Id, ResultEnum.OK);
        }
        else
        {
            return new ManagerActionResult<int>(-1, ResultEnum.Unauthorizated);
        }
    }

    public async Task<IManagerActionResult<FoodCategory[]>> GetAllFoodCategoriesAsync()
    {
        var categories = await _dbInfo.FoodCategories.ToArrayAsync();
        return new ManagerActionResult<FoodCategory[]>(categories, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> RemoveFoodCategoryAsync(int foodCategoryId, string user)
    {
        if (Enum.IsDefined(typeof(ElevatedUsers), user))
        {
            var category = await _dbInfo.FoodCategories.FindAsync(foodCategoryId);
            if (category == null)
            {
                return new ManagerActionResult(ResultEnum.NotFound);
            }

            _dbInfo.FoodCategories.Remove(category);
            await _dbInfo.SaveChangesAsync();

            return new ManagerActionResult(ResultEnum.OK);
        }
        else
        {
            return new ManagerActionResult(ResultEnum.Unauthorizated);
        }
    }

    public async Task<IManagerActionResult<FoodCategory[]>> GetFoodCategorySuggestionsAsync(string name, int resultsCount)
    {
        var categories = await _dbInfo.FoodCategories.Where(x => x.Name.Contains(name)).Take(resultsCount).ToArrayAsync();
        return new ManagerActionResult<FoodCategory[]>(categories, ResultEnum.OK);
    }

}
