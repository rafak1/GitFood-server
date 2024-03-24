using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class Barcode
{
    public string Key { get; set; }

    public int? ProductId { get; set; }

    public virtual Product Product { get; set; }
}
