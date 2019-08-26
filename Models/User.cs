using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Text;

namespace Sojourner.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string state { get; set; }
        public byte[] key { get; set; }

        public void hashMyPassword()
        {
            this.key = new byte[32];
            RandomNumberGenerator.Fill(this.key);
            var hashed = new HMACSHA256(key).ComputeHash(new UTF8Encoding().GetBytes(password));
            this.password = System.Convert.ToBase64String(hashed);
        }

        public bool checkPassword(string incoming)
        {
            var hashed = new HMACSHA256(this.key).ComputeHash(new UTF8Encoding().GetBytes(incoming));
            var realPassword = System.Convert.FromBase64String(this.password);
            return realPassword == hashed;
        }
    }
}