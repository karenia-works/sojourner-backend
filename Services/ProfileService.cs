using System;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using Sojourner.Models;
using System.Threading.Tasks;

namespace Sojourner.Services

{
    public class ProfileService
    {
        private readonly IMongoCollection<Profile> _profile;
        private readonly IDbSettings settings;
        private IMongoDatabase database;
        public ProfileService(IDbSettings settings)
        {
            this.settings = settings;
            var client = new MongoClient(settings.DbConnection);
            database = client.GetDatabase(settings.DbName);
            _profile = database.GetCollection<Profile>(settings.ProfileCollectionName);
        }

        public async Task<List<Profile>> getProfileList()
        {
            var query = await _profile.AsQueryable().OrderBy(p => p.userName).ToListAsync();
            return query;
        }

        // public async Task<List<Profile>> getWorkerProfileList()
        // {
        //     var query = await _profile.AsQueryable().Where(p1=>p1.role=="worker").OrderBy(p => p.userName).ToListAsync();
        //     return query;
        // }
        // public async Task<List<Profile>> getUserProfileList()
        // {
        //     var query = await _profile.AsQueryable().Where(p1=>p1.role=="IdertityServerApi").OrderBy(p => p.userName).ToListAsync();
        //     return query;
        // }
        // public async Task<List<Profile>> getAdminProfileList()
        // {
        //     var query = await _profile.AsQueryable().Where(p1=>p1.role=="worker").OrderBy(p => p.userName).ToListAsync();
        //     return query;
        // }

        public async Task<List<Profile>> getDirectProfileList(string role)
        {
            var query = await _profile.AsQueryable().Where(p1=>p1.role==role).OrderBy(p => p.userName).ToListAsync();
            return query;
        }

        // public async Task<Profile> getProfileById(string id_in)
        // {
        //     var query = await _profile.AsQueryable().
        //         Where(p => p.userId == id_in).FirstOrDefaultAsync();
        //     return query;
        // }

        public async Task<Profile> getProfileByEmail(string userName_in)
        {
            var query = await _profile.AsQueryable().
                Where(p => p.email == userName_in).FirstOrDefaultAsync();
            return query;
        }

        public async ValueTask<bool> insertProfile(Profile tar)
        {
            await _profile.InsertOneAsync(tar);
            return true;
        }

        public async Task<DeleteResult> deleteProfile(string email)
        {
            var res = await _profile.DeleteOneAsync(p => p.email == email);
            return res;
        }

        public async Task<UpdateResult> updateProfile(Profile profile)
        {
            var flicker = Builders<Profile>.Filter.Eq("email", profile.email);
            var update = Builders<Profile>
                .Update.Set("userName", profile.userName).Set("sex", profile.sex)
                .Set("phoneNumber", profile.phoneNumber).Set("avatar", profile.avatar)
                .Set("role", profile.role).Set("isRenting", profile.isRenting);
            var result = await _profile.UpdateOneAsync(flicker, update);
            return result;
        }
    }
}