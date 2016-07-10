using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Warehouse.Server.Data;
using Warehouse.Server.Logger;
using Warehouse.Server.Models;

namespace Warehouse.Server.Controllers
{
    [Authorize]
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IMongoContext context;
        private readonly ILogger logger;

        public ProductsController(IMongoContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            logger.TrackRequest(Request, true);
            return Ok(context.Products.FindAll());
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            var data = context.Products.FindOneById(new ObjectId(id));

            logger.TrackRequest(Request, data != null);

            if (data != null)
            {
                return Ok(data);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("getMany")]
        public IHttpActionResult GetMany(string[] ids)
        {
            if (ids == null)
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var objectIds = ids.Select(x => new ObjectId(x));
            var query = Query<Product>.In(x => x.Id, objectIds);
            var data = context.Products.Find(query);
            if (data == null)
            {
                logger.TrackRequest(Request, false);
                return InternalServerError();
            }

            logger.TrackRequest(Request, true);
            return Ok(data);
        }

        [HttpGet]
        [Route("getNames")]
        public IHttpActionResult GetNames()
        {
            var data = context.Products
                .FindAll()
                .SetFields(Fields<Product>.Include(x => x.Name, x => x.Size));
            if (data != null)
            {
                var names = data.Select(x => new ProductName { Id = x.Id, Name = string.Concat(x.Name, " ", x.Size) });
                logger.TrackRequest(Request, true);
                return Ok(names);
            }

            logger.TrackRequest(Request, false);
            return InternalServerError();
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult PutDeprecated()
        {
            logger.TrackRequest(Request, false);
            return StatusCode(HttpStatusCode.Gone);
        }

        [HttpPut]
        [Route("~/api/v2/products/{id}")]
        public IHttpActionResult Put(string id, [FromBody] Product product)
        {
            var query = Query<Product>.EQ(p => p.Id, new ObjectId(id));
            var update = Update<Product>
                .Set(p => p.Name, product.Name)
                .Set(p => p.Size, product.Size)
                .Set(p => p.K, product.K)
                .Set(p => p.PriceOpt, product.PriceOpt)
                .Set(p => p.PriceRozn, product.PriceRozn)
                .Set(p => p.Weight, product.Weight)
                .Set(p => p.Count, product.Count)
                .Set(p => p.Nd, product.Nd)
                .Set(p => p.Length, product.Length)
                .Set(p => p.PriceIcome, product.PriceIcome)
                .Set(p => p.Internal, product.Internal)
                .Set(p => p.Firma, product.Firma)
            ;
            var res = context.Products.Update(query, update);

            logger.TrackRequest(Request, res.Ok);

            var code = res.Ok ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            return StatusCode(code);
        }

        [HttpPost]
        [Route]
        public IHttpActionResult Post([FromBody] Product product)
        {
            var res = context.Products.Save(product);

            logger.TrackRequest(Request, res.Ok);

            if (res.Ok)
            {
                var message = new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(product.Id.ToString())
                };
                return ResponseMessage(message);
            }
            return InternalServerError();
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

            var arr = ids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length <= 0)
            {
                logger.TrackRequest(Request, false);
                return BadRequest();
            }

            var objectIds = arr.Select(x => new ObjectId(x));
            var query = Query<Product>.In(x => x.Id, objectIds);
            var res = context.Products.Remove(query);
            if (res.Ok)
            {
                logger.TrackRequest(Request, true);
                return Ok();
            }

            logger.TrackRequest(Request, false);
            return InternalServerError();
        }

        [HttpPut]
        [Route("updatePrice")]
        public IHttpActionResult UpdatePriceDeprecated()
        {
            logger.TrackRequest(Request, false);
            return StatusCode(HttpStatusCode.Gone);
        }

        [HttpPut]
        [Route("~/api/v2/products/updatePrice")]
        public IHttpActionResult UpdatePrice(ProductPriceUpdate[] items)
        {
            var bulk = context.Products.InitializeUnorderedBulkOperation();
            foreach (var x in items)
            {
                var query = Query<Product>.EQ(p => p.Id, new ObjectId(x.Id));
                var update = Update<Product>
                    .Set(p => p.PriceOpt, x.NewPriceOpt)
                    .Set(p => p.PriceRozn, x.NewPriceRozn);
                bulk.Find(query).UpdateOne(update);
            }
            bulk.Execute();

            logger.TrackRequest(Request, true);
            return Ok();
        }

        [HttpGet]
        [Route("{id}/files")]
        public IHttpActionResult GetFiles(string id)
        {
            var productId = new ObjectId(id);
            var ids = new[] {productId};
            var query = Query.In("metadata.products", new BsonArray(ids));
            var files = context.Database.GridFS.Find(query);

            var data = files.Select(x => new FileDescription
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Size = x.Length,
                UploadDate = x.UploadDate,
            });

            logger.TrackRequest(Request, true);
            return Ok(data);
        }
    }
}
