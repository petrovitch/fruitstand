using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class MerchantSearchResult : BaseResponse
	{
		public List<Merchant> Items { get; set; }
	}
}