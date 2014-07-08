using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities
{
	public class Product
	{
		public string Name { get; set; }
		public int ProductID { get; set; }
		public string Description { get; set; }
		public double UnitPrice { get; set; }
	}
}