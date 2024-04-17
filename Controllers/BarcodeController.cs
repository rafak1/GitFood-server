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

    private readonly IBarcodeManager _barcodeManger;

    public BarcodeController(IBarcodeManager database)
    {
        _barcodeManger = _barcodeManger ?? throw new ArgumentNullException(nameof(_barcodeManger));
    }

    [HttpPost]
    [Route($"{_controllerRoute}/add")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _barcodeManger.AddBarcodeAsync(barcode);
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
}
