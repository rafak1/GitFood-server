using Newtonsoft.Json;

namespace Server.Data.Models;

public partial class Fridge
{
    public string UserLogin { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<FridgeProduct> FridgeProducts { get; set; } = new List<FridgeProduct>();

    [JsonIgnore]
    public virtual User UserLoginNavigation { get; set; }
}
