using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Warehouse.Server.Data;
using Warehouse.Server.Models;

namespace Warehouse.Server.Controllers
{
    [Authorize]
    public class FilesController : ApiController
    {
        private readonly IMongoContext context;

        public FilesController(IMongoContext context)
        {
            this.context = context;
        }

        public HttpResponseMessage Get()
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

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(string id)
        {
            ObjectId fileId;
            if (!ObjectId.TryParse(id, out fileId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var file = context.Database.GridFS.FindOneById(fileId);
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var stream = file.OpenRead();
            var resp = Request.CreateResponse();
            resp.Content = new StreamContent(stream);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Image.Jpeg);
            return resp;
        }

        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");

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

                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(fileId)
                };
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(string ids)
        {
            if (ids == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var arr = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                var objectIds = arr.Select(x => new ObjectId(x));
                var query = Query.In("_id", new BsonArray(objectIds));
                context.Database.GridFS.Delete(query);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/files/{id}/products")]
        [HttpPost]
        public HttpResponseMessage AttachProducts(string id, string[] productIds)
        {
            if (productIds == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var file = context.Database.GridFS.FindOneById(new ObjectId(id));
            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var ids = productIds.Select(x => new ObjectId(x));
            var meta = new FileMetadata { ProductIds = new HashSet<ObjectId>(ids) };

            context.Database.GridFS.SetMetadata(file, meta.ToBsonDocument());

            return Request.CreateResponse(HttpStatusCode.Created);
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
