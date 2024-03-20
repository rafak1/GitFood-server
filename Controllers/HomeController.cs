using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.ViewModels;
using Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _dbInfo;

    public HomeController(ILogger<HomeController> logger, DataContext database)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbInfo = database ?? throw new ArgumentNullException(nameof(database));
    }

    [HttpPatch]
    [Route("/addRandomBarcode")]
    public async Task<IActionResult> AddRandomBarcode() 
    {
        await _dbInfo.Barcodes.AddAsync(new Barcode{ BarcodeBytes = new byte[10], BarcodeNumber = "101010", Name = "dziwna rzecz"});
        await _dbInfo.SaveChangesAsync();
        return Ok();
    }

    
    [HttpPost]
    [Route("/addBarcode")]
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

    [HttpGet]
    [Route("/getBarcodes")]
    public IActionResult GetBarcodes() 
    {
        return Ok(_dbInfo.Barcodes.ToArray());
    }

    [HttpGet]
    [Route("/deleteBarcode")]
    public async Task<IActionResult> DeleteBarcode(string barcodeName) 
    {
        await _dbInfo.Barcodes.Where(x => x.BarcodeNumber == barcodeName).ExecuteDeleteAsync();
        return Ok();
    }
}
