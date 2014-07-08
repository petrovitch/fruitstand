using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.Contracts.Entities;

namespace Web.Code.Logic
{
	/// <summary>
	/// Provides access to our back-end data store, like product lists
	/// </summary>
	public class DataRepository
	{
		/// <summary>
		/// Returns the products that are available for purchase on the site
		/// </summary>
		/// <returns></returns>
		public List<Product> GetProducts()
		{
			var result = new List<Product>();
			result.Add(new Product() { ProductID = 1, Name = "Bananas", Description = "Our bananas have been quality certified by real monkey scientists.", UnitPrice = 10.5 });
			result.Add(new Product() { ProductID = 2, Name = "Oranges", Description = "Oranges are great for Vitamin C. Available in any color as long as its orange.", UnitPrice = 13 });
			result.Add(new Product() { ProductID = 3, Name = "Grapes", Description = "Often confused for oranges because they are the same shape, grapes are in fact smaller and a different color.", UnitPrice = 0.35 });
			return result;
		}
	}
}