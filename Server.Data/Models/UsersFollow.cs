using System;
using System.Collections.Generic;

namespace Server.DataModel;

public partial class UsersFollow
{
    public string User { get; set; }

    public string Follows { get; set; }

    public virtual User FollowsNavigation { get; set; }

    public virtual User UserNavigation { get; set; }
}
