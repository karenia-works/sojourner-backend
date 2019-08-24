
namespace Sojourner.Models
{
    public class SearchRequest
    {
        public string kw { get; set; }
        public string room_type { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int limit { get; set; }
        public int skip { get; set; }
    }
}
