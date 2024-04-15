using Microsoft.AspNetCore.Mvc;
using Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Server.DataModel;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class BarcodeController : Controller
{
    private const string _controllerRoute = "/barcode";

    private readonly GitfoodContext _dbInfo;

    private readonly ITokenStorage _tokenStorage;

    public BarcodeController(GitfoodContext database, ITokenStorage tokenStorage)
    {
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));   
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            Key = barcode.BarcodeNumber,
            ProductId = barcode.ProductId,
            User = _tokenStorage.getUser(Request.Headers["Authorization"])
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetBarcode(string barcodeNumber) 
    {
        return Ok(await _dbInfo.Barcodes.FirstOrDefaultAsync(x => x.Key == barcodeNumber));
    }

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteBarcode(string barcodeName) 
    {
        await _dbInfo.Barcodes.Where(x => x.Key == barcodeName).ExecuteDeleteAsync();
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
