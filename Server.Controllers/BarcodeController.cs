using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Logic.Abstract.Managers;
using Server.Logic.Abstract.Token;
using Server.ViewModels.Barcodes;

namespace Server.Controllers;

[Authorize]
[ApiController]
public class BarcodeController : Controller
{
    private const string _controllerRoute = "/barcode";

    private const int _bearerOffset = 7;

    private readonly IBarcodeManager _barcodeManger;

    private readonly ITokenStorage _tokenStorage;

    public BarcodeController(ITokenStorage tokenStorage, IBarcodeManager barcodeManager)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));   
        _barcodeManger = _barcodeManger ?? throw new ArgumentNullException(nameof(_barcodeManger));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        var user = _tokenStorage.GetUser(Request.Headers.Authorization.ToString()[_bearerOffset..]);
        if(user == null) 
            return BadRequest("No user found assigned to this token");

        return (await _barcodeManger.AddBarcodeAsync(barcode)).MapToActionResult();
    }

    [HttpGet]
    [Route($"{_controllerRoute}/get")]
    public async Task<IActionResult> GetBarcode(string barcodeNumber) 
        => (await _barcodeManger.GetBarcodeAsync(barcodeNumber)).MapToActionResult();

    [HttpDelete]
    [Route($"{_controllerRoute}/delete")]
    public async Task<IActionResult> DeleteBarcode(string barcodeNumber) 
        => (await _barcodeManger.DeleteBarcodeAsync(barcodeNumber)).MapToActionResult();

    [HttpGet]
    [Route($"{_controllerRoute}/suggest")]
    public async Task<IActionResult> SuggestBarcode(string barcodeName) 
        => (await _barcodeManger.SuggestBarcodeAsync(barcodeName)).MapToActionResult();
}
