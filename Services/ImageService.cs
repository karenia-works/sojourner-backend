using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Sojourner.Models;
using Sojourner.Models.Settings;

namespace Sojourner.Services
{
    public class ImageService
    {
        public ImageService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            this.bucket = new GridFSBucket(database, new GridFSBucketOptions()
            {
                BucketName = settings.ImageBucketName,
                ChunkSizeBytes = 5248000,

            });
        }

        private IGridFSBucket bucket;

        public async Task<ObjectId> uploadFileAsync(string fileName, string mimeType, Stream fileStream)
        {
            // var mimeType = sojourner_backend.MimeTypes.GetMimeType(fileName);
            var options = new GridFSUploadOptions()
            {
                Metadata = new ImageMetadata()
                {
                    mimeType = mimeType
                }.ToBsonDocument()
            };
            return await bucket.UploadFromStreamAsync(fileName, fileStream, options);
        }

        public async Task<ObjectId> uploadFileAsync(string fileName, string mimeType, byte[] fileBytes)
        {
            // var mimeType = sojourner_backend.MimeTypes.GetMimeType(fileName);
            var options = new GridFSUploadOptions()
            {
                Metadata = new ImageMetadata()
                {
                    mimeType = mimeType
                }.ToBsonDocument()
            };
            return await bucket.UploadFromBytesAsync(fileName, fileBytes, options);
        }

        public async Task<GridFSDownloadStream<ObjectId>> downloadFileAsync(ObjectId id)
        {
            return await bucket.OpenDownloadStreamAsync(
                id,
                new GridFSDownloadOptions()
                {
                    Seekable = true
                });
        }

        public async Task deleteFileAsync(ObjectId id)
        {
            await bucket.DeleteAsync(id);
        }
    }
}
