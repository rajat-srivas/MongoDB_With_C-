using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;


namespace MongoDB.Operations.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private MongoClient _client;
		private IMongoDatabase _database;
		private IMongoCollection<Product> _collection;
		public ProductController()
		{
			string connectionUri = "mongodb+srv://rajatsrivastava:mongo%21%402024@devtest.3h3djvb.mongodb.net/?retryWrites=true&w=majority&appName=devtest";
			var settings = MongoClientSettings.FromConnectionString(connectionUri);
			settings.ServerApi = new ServerApi(ServerApiVersion.V1);
			_client = new MongoClient(settings);
			_database = _client.GetDatabase("stackup");
			_collection = _database.GetCollection<Product>("product");

			//SeedData(_collection);
		}

		[HttpPost]
		public IActionResult CreateProduct(Product product)
		{
			if(product.Category is null)
			{
				throw new ArgumentNullException("Category is required");
			}
			// Insert sample data
			_collection.InsertOne(product);
			return Ok();
		}
		[HttpGet]
		public IActionResult GetAllProducts()
		{
			var filter = Builders<Product>.Filter.Empty;
			var result = _collection.Find(filter).ToList() ;
			return Ok(result);
		}

		[HttpGet("filter")]
		
		public IActionResult FilterProducts()
		{
			//using Linq without builder
			var filteredData = _collection.Find(x => x.InStock == true).ToList();

			//using builder
			var filterBuilder = Builders<Product>.Filter;
			FilterDefinition<Product> filter = null;

			//filter based on equality, less than etc
			filter = filterBuilder.Lt(x => x.Price, 800) & filterBuilder.Eq(x => x.InStock, false);

			//array filter
			// filter = filterBuilder.AnyEq(x => x.Ratings, 1);

			filteredData = _collection.Find(filter).ToList();

			return Ok(filteredData);
		}

		[HttpGet("sort")]

		public IActionResult SortProducts()
		{
			var sortBuilder = Builders<Product>.Sort;

			var sortFilter = sortBuilder.Ascending(x => x.Price).Descending(x => x.Ratings);

			var filter = Builders<Product>.Filter.Empty;

			var result = _collection.Find(filter).Sort(sortFilter).ToList();

			return Ok(result);
		}

		[HttpGet("projection")]
		//Projections => which fields to include or exclude in the query results

		public IActionResult ProjectProducts()
		{
			var projectionBuilder = Builders<Product>.Projection;
			
			ProjectionDefinition<Product> projection = null;

			projection = projectionBuilder.Include(x => x.Name).Include(x => x.Category);

			//projection = projection.Exclude(x => x.Price);

			var filter = Builders<Product>.Filter.Eq(x => x.Category, "Electronics");

			var result = _collection.Find(filter).Project<Product>(projection).ToList();

			return Ok(result);
		}

		[HttpGet("aggregate")]
		//Projections => which fields to include or exclude in the query results

		public IActionResult AggregateProducts()
		{
			//var filter = Builders<Product>.Filter.Eq(x => x.InStock, true);

			var filter = Builders<Product>.Filter.Empty;

			//with sort
			var aggregate2 = _collection.Aggregate().Match(filter).SortByDescending(x => x.Price); 

			//with group
			var aggregate = _collection.Aggregate()
				.Match(filter)
				.Group(x => x.InStock,
					doc => new
					{
						Instocks = doc.Key,
						count = doc.Sum(x => 1)
					}
				);

			//with projection => create new response with new field

			var projections = Builders<Product>.Projection.Expression(x => new
			{
				Category = x.Category,
				Price = x.Price,
				Name = x.Name,
				Available = x.InStock ? "Yes" : "No"
			});
			var aggregate3 = _collection.Aggregate().Match(filter)
				.Project(projections);


			var result = aggregate3.ToList();

			return Ok(result);
		}

		[HttpPut("{id}")]
		public IActionResult UpdateProduct(string id)
		{
			var prodcutToUpdateFilter = Builders<Product>.Filter.Eq(x => x.Id, id);

			var update = Builders<Product>.Update.Set(x => x.Name, "Macbook Laptop");
			var result = _collection.UpdateOne(prodcutToUpdateFilter, update);
			return Ok(result.IsAcknowledged);
		}
		private void SeedData(IMongoCollection<Product> collection)
		{
			var products = new List<Product>
		{
			new Product
			{
				Name = "Laptop",
				Category = "Electronics",
				Price = 999.99m,
				InStock = true,
				CreatedDate = DateTime.UtcNow,
				Ratings = new List<int> { 4, 5 },
				Tags = new List<string> { "computer", "portable" },
				Manufacturer = new Manufacturer { Name = "TechCorp", Country = "USA", EstablishedYear = 1990 },
				Specifications = new Specifications { Dimensions = new Dimensions { Length = 13.3, Width = 9.0, Height = 0.8 }, Weight = 2.8, Color = "Silver" },
				Reviews = new List<Review>
				{
					new Review { ReviewerName = "Alice", Rating = 5, Comment = "Great laptop!", ReviewDate = DateTime.UtcNow.AddDays(-1) },
					new Review { ReviewerName = "Bob", Rating = 4, Comment = "Good performance.", ReviewDate = DateTime.UtcNow.AddDays(-2) }
				}
			},
			new Product
			{
				Name = "Smartphone",
				Category = "Electronics",
				Price = 699.99m,
				InStock = false,
				CreatedDate = DateTime.UtcNow.AddDays(-10),
				Ratings = new List<int> { 5, 5, 4 },
				Tags = new List<string> { "phone", "touchscreen" },
				Manufacturer = new Manufacturer { Name = "PhoneInc", Country = "China", EstablishedYear = 2005 },
				Specifications = new Specifications { Dimensions = new Dimensions { Length = 6.1, Width = 2.9, Height = 0.3 }, Weight = 0.4, Color = "Black" },
				Reviews = new List<Review>
				{
					new Review { ReviewerName = "Charlie", Rating = 5, Comment = "Amazing phone!", ReviewDate = DateTime.UtcNow.AddDays(-3) },
					new Review { ReviewerName = "Dave", Rating = 4, Comment = "Very good value.", ReviewDate = DateTime.UtcNow.AddDays(-5) }
				}
			},
			new Product
			{
				Name = "Desk",
				Category = "Furniture",
				Price = 199.99m,
				InStock = true,
				CreatedDate = DateTime.UtcNow.AddDays(-20),
				Ratings = new List<int> { 4, 4, 3 },
				Tags = new List<string> { "wood", "office" },
				Manufacturer = new Manufacturer { Name = "FurniCo", Country = "Sweden", EstablishedYear = 1980 },
				Specifications = new Specifications { Dimensions = new Dimensions { Length = 48.0, Width = 24.0, Height = 30.0 }, Weight = 50.0, Color = "Brown" },
				Reviews = new List<Review>
				{
					new Review { ReviewerName = "Eve", Rating = 4, Comment = "Sturdy and well-built.", ReviewDate = DateTime.UtcNow.AddDays(-7) },
					new Review { ReviewerName = "Frank", Rating = 3, Comment = "Decent quality.", ReviewDate = DateTime.UtcNow.AddDays(-10) }
				}
			}
		};
			// Insert sample data
			collection.InsertManyAsync(products);

		}
	}
}
