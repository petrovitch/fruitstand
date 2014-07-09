using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class Merchant : BaseResponse
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}