

namespace Sojourner.Models.Settings
{
    public class DbSettings : IDbSettings
    {
        public string DbName { get; set; }
        public string DbConnection { get; set; }
        public string UserCollectionName { get; set; }
    }

    public interface IDbSettings
    {
        string DbName { get; set; }
        string DbConnection { get; set; }
        string UserCollectionName { get; set; }
    }
}
