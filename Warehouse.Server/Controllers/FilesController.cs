using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Warehouse.Server.Data;
using Warehouse.Server.Logger;
using Warehouse.Server.Models;

namespace Warehouse.Server.Controllers
{
    [Authorize]
    [RoutePrefix("api/files")]
    public class FilesController : ApiController
    {
        private readonly IMongoContext context;
        private readonly ILogger logger;

        public FilesController(IMongoContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [Route]
        public IHttpActionResult Get()
        {
            var files = context.Database.GridFS.FindAll();
            var data = files.Select(x => new FileDescription
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Size = x.Length,
                UploadDate = x.UploadDate,
                Metadata = (x.Metadata != null) ? BsonSerializer.Deserialize<FileMetadata>(x.Metadata) : null
            });

            logger.TrackRequest(Request, true);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            ObjectId fileId;
            if (!ObjectId.TryParse(id, out fileId))
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var file = context.Database.GridFS.FindOneById(fileId);
            if (file == null)
            {
                logger.TrackRequest(Request, false);
                return NotFound();
            }

            var stream = file.OpenRead();
            var resp = Request.CreateResponse(HttpStatusCode.OK);
            resp.Content = new StreamContent(stream);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Image.Jpeg);

            logger.TrackRequest(Request, true);
            return ResponseMessage(resp);
        }

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                logger.TrackRequest(Request, false);
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            var root = Path.Combine(Path.GetTempPath(), "skill-uploads");

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);

            await Request.Content.ReadAsMultipartAsync(provider);
            var fileData = provider.FileData.FirstOrDefault();

            if (fileData != null)
            {
                var file = fileData.LocalFileName;
                var remoteFileName = fileData.Headers.ContentDisposition.FileName;
                var contentType = fileData.Headers.ContentDisposition.Name;

                var fileId = Upload(file, remoteFileName, contentType);

                File.Delete(file);

                logger.Info("file uploaded: " + file);

                var resp = Request.CreateResponse(HttpStatusCode.Created);
                resp.Content = new StringContent(fileId);

                logger.TrackRequest(Request, true);
                return ResponseMessage(resp);
            }

            logger.TrackRequest(Request, false);
            return BadRequest();
        }

        [HttpDelete]
        [Route]
        public IHttpActionResult Delete(string ids)
        {
            if (ids == null)
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var arr = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                var objectIds = arr.Select(x => new ObjectId(x));
                var query = Query.In("_id", new BsonArray(objectIds));
                context.Database.GridFS.Delete(query);
            }

            logger.TrackRequest(Request, true);
            return Ok();
        }

        [HttpPost]
        [Route("{id}/products")]
        public IHttpActionResult AttachProducts(string id, string[] productIds)
        {
            if (productIds == null)
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var file = context.Database.GridFS.FindOneById(new ObjectId(id));
            if (file == null)
            {
                logger.TrackRequest(Request, false);
                return NotFound();
            }

            var ids = productIds.Select(x => new ObjectId(x));
            var meta = new FileMetadata { ProductIds = new HashSet<ObjectId>(ids) };

            context.Database.GridFS.SetMetadata(file, meta.ToBsonDocument());

            logger.TrackRequest(Request, true);
            return Created(string.Empty, string.Empty);
        }

        private string Upload(string file, string remoteFileName, string contentType)
        {
            using (var fs = new FileStream(file, FileMode.Open))
            {
                var options = new MongoGridFSCreateOptions { ContentType = contentType };
                var info = context.Database.GridFS.Upload(fs, remoteFileName, options);
                return info.Id.ToString();
            }
        }
    }
}
