using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities
{
	public class CartItem
	{
		public int ProductID { get; set; }
		public double Quantity { get; set; }
	}
}