using System;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using Sojourner.Models;

namespace Sojourner.Services

{
    public class ProfileService
    {
        private readonly IMongoCollection<Profile> _profile;
        private readonly IDbSettings settings;
        public ProfileService(IDbSettings settings)
        {
            this.settings = settings;
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _profile = database.GetCollection<Profile>(settings.UserCollectionName);
        }

        public List<Profile> getProfileList(){
            var query = _profile.AsQueryable().OrderByDescending( p=>p.userName);
            return query.ToList();
        }

        public Profile getProfileById(string id_in){
            var query = _profile.AsQueryable().
                Where(p => p.userId == id_in).FirstOrDefault();
            return query;
        }

        public Profile getProfileByUserName(string userName_in){
            var query = _profile.AsQueryable().
                Where(p => p.userName == userName_in).FirstOrDefault();
            return query;
        }

        public bool insertProfile(Profile tar){
            _profile.InsertOne(tar);
            return true;
        }

        public DeleteResult deleteProfile(Profile tar){
            var res = _profile.DeleteOne(p => p.userId == tar.userId);
            return res;
        }

        public UpdateResult updateProfile(Profile profile){
            var flicker = Builders<Profile>.Filter.Eq("userId", profile.userId);
            var update  = Builders<Profile>.Update.Set("userName", profile.userName)
                .Set("sex", profile.sex).Set("email", profile.email)
                .Set("phoneNumber", profile.phoneNumber).Set("avatar", profile.avatar);
            var result = _profile.UpdateOne(flicker, update);
            return result;
        }
    }
}