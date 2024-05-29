using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MongoDB.Operations
{
	//If class is decorated with [BsonIgnoreExtraElements] all extra fields present in document and not in class are ignored
	//[BsonIgnoreExtraElements]
	public class Product
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("category")]
		public string Category { get; set; }

		[BsonElement("price")]
		public decimal Price { get; set; }

		[BsonElement("inStock")]
		public bool InStock { get; set; }

		[BsonElement("createdDate")]
		public DateTime CreatedDate { get; set; }

		[BsonElement("ratings")]
		public List<int> Ratings { get; set; }

		[BsonElement("tags")]
		public List<string> Tags { get; set; }

		[BsonElement("manufacturer")]
		public Manufacturer Manufacturer { get; set; }

		[BsonElement("specifications")]
		public Specifications Specifications { get; set; }

		[BsonElement("reviews")]
		public List<Review> Reviews { get; set; }


		//Any extra field that is present in the document but doesnt have any corresponding property in the class are added to this dictionary by default
		//If this is not present and class is not decorated with [BsonIgnoreExtraElements] then it will throw error stating new matching field for any field in document and not in class
		[BsonExtraElements]
		public Dictionary<string, object> ExtraElements { get; set; }
	}

	public class Manufacturer
	{
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("country")]
		public string Country { get; set; }

		[BsonElement("establishedYear")]
		public int EstablishedYear { get; set; }
	}

	public class Specifications
	{
		[BsonElement("dimensions")]
		public Dimensions Dimensions { get; set; }

		[BsonElement("weight")]
		public double Weight { get; set; }

		[BsonElement("color")]
		public string Color { get; set; }
	}

	public class Dimensions
	{
		[BsonElement("length")]
		public double Length { get; set; }

		[BsonElement("width")]
		public double Width { get; set; }

		[BsonElement("height")]
		public double Height { get; set; }
	}

	public class Review
	{
		[BsonElement("reviewerName")]
		public string ReviewerName { get; set; }

		[BsonElement("rating")]
		public int Rating { get; set; }

		[BsonElement("comment")]
		public string Comment { get; set; }

		[BsonElement("reviewDate")]
		public DateTime ReviewDate { get; set; }
	}
}
