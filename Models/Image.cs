
using MongoDB.Bson.Serialization;

namespace Sojourner.Models
{
    public class ImageMetadata
    {
        public string mimeType;
        static ImageMetadata()
        {
            BsonClassMap.RegisterClassMap<ImageMetadata>();
        }
    }
}
