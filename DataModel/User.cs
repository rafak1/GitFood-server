using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class User
{
    public string Login { get; set; }

    public string Password { get; set; }

    public virtual ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
}
