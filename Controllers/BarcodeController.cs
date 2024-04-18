using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract;
using Server.ViewModels.Barcodes;
using Server.Data.Models;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class BarcodeController : Controller
{
    private const string _controllerRoute = "/barcode";

    private const int _bearerOffset = 7;

    private readonly IBarcodeManager _barcodeManger;

    private readonly ITokenStorage _tokenStorage;

    public BarcodeController(GitfoodContext database, ITokenStorage tokenStorage)
    public BarcodeController(IBarcodeManager database)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));   
        _barcodeManger = _barcodeManger ?? throw new ArgumentNullException(nameof(_barcodeManger));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _barcodeManger.AddBarcodeAsync(barcode);
        var user = _tokenStorage.GetUser(Request.Headers.Authorization.ToString()[_bearerOffset..]);
        if(user == null) return BadRequest("No user found assigned to this token");

        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            Key = barcode.BarcodeNumber,
            ProductId = barcode.ProductId,
            User = user
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetBarcode(string barcodeNumber) 
        => Ok(await _barcodeManger.GetBarcodeAsync(barcodeNumber));

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteBarcode(string barcodeNumber) 
    {
        await _barcodeManger.DeleteBarcodeAsync(barcodeNumber);
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/suggest")]
    public async Task<IActionResult> SuggestBarcode(string barcodeName) 
    {
        return Ok( 
            await _dbInfo.Barcodes
            .Where(x => x.Key == barcodeName)
            .GroupBy(x => x.Key)
            .OrderByDescending(x => x.Count())
            .FirstOrDefaultAsync()
        );
    }
}
