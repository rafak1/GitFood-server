using System;
using System.Collections.Generic;

namespace Server.Data.Models;

public partial class AddCategoriesRequest
{
    public string User { get; set; }

    public int? Request { get; set; }

    public DateOnly? Datetime { get; set; }

    public int Id { get; set; }

    public virtual Category RequestNavigation { get; set; }
}
