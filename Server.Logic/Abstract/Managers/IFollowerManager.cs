
using Server.Logic.Abstract;

namespace Server.Logic.Abstract.Managers;

public interface IFollowerManager
{
    public Task<IManagerActionResult> AddFollowerAsync(string userToFollow, string user);
    public Task<IManagerActionResult> RemoveFollowerAsync(string userToUnfollow, string user);
    public Task<IManagerActionResult<string[]>> GetFollowersAsync(string user);
    public Task<IManagerActionResult<string[]>> GetFollowingAsync(string user);
}