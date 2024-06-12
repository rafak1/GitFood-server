using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class FollowerController : BaseController
{
    private const string _controllerRoute = "/follow";
    private readonly IFollowerManager _followerManager;

    public FollowerController(IFollowerManager followerManager, ITokenStorage tokenStorage) : base(tokenStorage)
    {
        _followerManager = followerManager ?? throw new ArgumentNullException(nameof(followerManager));
    }


    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddFollower(string userToFollow)
    {
        var user = GetUser();
        return (await _followerManager.AddFollowerAsync(userToFollow, user)).MapToActionResult();
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/remove")]
    public async Task<IActionResult> RemoveFollower(string userToUnfollow)
    {
        var user = GetUser();
        return (await _followerManager.RemoveFollowerAsync(userToUnfollow, user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getUserFollowers")]
    public async Task<IActionResult> GetFollowers()
    {
        var user = GetUser();
        return (await _followerManager.GetFollowersAsync(user)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/getFollowing")]
    public async Task<IActionResult> GetFollowing()
    {
        var user = GetUser();
        return (await _followerManager.GetFollowingAsync(user)).MapToActionResult();
    }

}