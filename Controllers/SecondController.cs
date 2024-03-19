using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.ViewModels;
using Server.Data.Models;

namespace Server.Controllers;

[ApiController]
public class SecondController : Controller
{
    private readonly ILogger<SecondController> _logger;
    private readonly DataContext _dbInfo;
    private const string ControllerPart = "secControler";

    public SecondController(ILogger<SecondController> logger, DataContext database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    [Route($"{ControllerPart}/addRandomBarcode")]
    public async Task<IActionResult> AddRandomBarcode() 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode{ BarcodeBytes = new byte[10], BarcodeNumber = "101010", Name = "dziwna rzecz"});
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    
    [HttpPost]
    [Route($"{ControllerPart}/addBarcode")]
    public async Task<IActionResult> AddBarcode(BarcodeViewModel barcode) 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode
        {
            BarcodeBytes = barcode.BarcodeBytes,
            BarcodeNumber = barcode.BarcodeNumber,
            Name = barcode.Name
        });
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    [Route($"{ControllerPart}/getBarcodes")]
    public IActionResult GetBarcodes() 
    {
        return Ok(_dbInfo.Barcodes.ToArray());
    }
}
