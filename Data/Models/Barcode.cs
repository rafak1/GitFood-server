using Microsoft.EntityFrameworkCore;

namespace Server.Data.Models;

[PrimaryKey(nameof(BarcodeId))]
public class Barcode {
    public int BarcodeId {get;}

    public byte[] BarcodeBytes {get; set;}

    public string BarcodeNumber {get; set;}

    public string Name {get; set;}
}