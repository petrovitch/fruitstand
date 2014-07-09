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
			result.Add(new Product() { ProductID = 1, Name = "Kiwifruit", Description = "Cultivation of the fuzzy kiwifruit spread from China in the early 20th century to New Zealand where the first commercial plantings occurred. The fruit was called 'yang tao' but was changed to 'Chinese gooseberry' by the New Zealanders.", UnitPrice = 2.49 });
			result.Add(new Product() { ProductID = 2, Name = "Feijoa", Description = "Feijoa is usually eaten by cutting it in half, then scooping out the pulp with a spoon. The fruit has a juicy, sweet seed pulp and slightly gritty flesh nearer the skin. If the utensils needed to eat it this way are not available, the feijoa may be torn or bitten in half, and the contents squeezed out and consumed.", UnitPrice = 8.99 });
			result.Add(new Product() { ProductID = 3, Name = "Tamarillo", Description = "Tamarillo are egg shaped and about 4-10 centimeters long. Their color varies from yellow and orange to red and almost purple. Sometimes they have dark, longitudinal stripes. Red fruits are more acetous, yellow and orange fruits are sweeter. The flesh has a firm texture and contains more and larger seeds than a common tomato.", UnitPrice = 7.99 });
			return result;
		}
	}
}