using System.Net.Http;
using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http.Extensions;

namespace Sojourner.Controllers
{
    [Serializable]
    public class ImageUploadResult
    {
        public List<string> successFiles;
        public List<string> fileIds;
        public List<string> failedFiles;
        public List<string> failReasons;
    }

    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private ImageService _imgService;
        public ImageController(ImageService imageService)
        {
            this._imgService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> uploadImage(IFormFile file)
        {

            ObjectId id;
            using (var fileStream = file.OpenReadStream())
            {
                try
                {
                    var time = DateTime.Now;
                    id = await _imgService.uploadFileAsync(file.FileName, file.ContentType, fileStream);
                    var endTime = DateTime.Now;
                    Console.WriteLine(endTime - time);
                    // successFiles.Add(file.FileName);
                    // fileIds.Add(id.ToString());
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                    // failedFiles.Add(file.FileName);
                    // failReasons.Add(e.Message);
                }
            }
            var returnUri = UriHelper.BuildRelative(Request.PathBase, $"/api/v1/image/{id.ToString()}");
            return Created(returnUri, id.ToString());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> downloadImage(string id)
        {
            var objectId = new ObjectId(id);
            try
            {
                var stream = await _imgService.downloadFileAsync(objectId);
                var filename = stream.FileInfo.Filename;
                var mimeType = stream.FileInfo.Metadata.GetValue("mimeType").AsString;
                return new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = filename
                };
            }
            catch (GridFSFileNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteImage(string id)
        {
            try
            {
                await _imgService.deleteFileAsync(new ObjectId(id));
                return Ok();
            }
            catch (GridFSFileNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
