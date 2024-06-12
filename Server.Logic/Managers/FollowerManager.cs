using Microsoft.EntityFrameworkCore;
using Server.Data.Models;
using Server.Database;
using Server.Logic.Abstract;
using Server.Logic.Abstract.Managers;

namespace Server.Logic.Managers;


public class FollowerManager : IFollowerManager
{
    private readonly GitfoodContext _dbInfo;

    private static readonly string _userNotFound = "User not found";

    public FollowerManager(GitfoodContext dbInfo) 
    {
        _dbInfo = dbInfo ?? throw new ArgumentNullException(nameof(dbInfo));
    }

    public async Task<IManagerActionResult> AddFollowerAsync(string userToFollow, string user)
    {
        var user1 = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == userToFollow);
        var user2 = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user);

        if(user1 == null || user2 == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _userNotFound);
        }

        user2.Follows.Add(user1);
        await _dbInfo.SaveChangesAsync();

        return new ManagerActionResult(ResultEnum.OK);
    }

    public async Task<IManagerActionResult<string[]>> GetFollowersAsync(string user)
    {
        var followers = await _dbInfo.Users.Where(x => x.Login == user).Select(x=>x.Users).FirstOrDefaultAsync();
        var followersNames = followers.Select(x=>x.Login).ToArray();
        return new ManagerActionResult<string[]>(followersNames, ResultEnum.OK);
    }

    public async Task<IManagerActionResult<string[]>> GetFollowingAsync(string user)
    {
        var followers = await _dbInfo.Users.Where(x => x.Login == user).Select(x=>x.Follows).FirstOrDefaultAsync();
        var followersNames = followers.Select(x=>x.Login).ToArray();
        return new ManagerActionResult<string[]>(followersNames, ResultEnum.OK);
    }

    public async Task<IManagerActionResult> RemoveFollowerAsync(string userToUnfollow, string user)
    {
        var user1 = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == userToUnfollow);
        var user2 = await _dbInfo.Users.FirstOrDefaultAsync(x => x.Login == user);

        if(user1 == null || user2 == null)
        {
            return new ManagerActionResult(ResultEnum.BadRequest, _userNotFound);
        }

        user2.Follows.Remove(user1);
        await _dbInfo.SaveChangesAsync();
        return new ManagerActionResult(ResultEnum.OK);
    }
}