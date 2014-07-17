using System.Collections.Generic;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class MerchantSearchResult : BaseResponse
	{
		public List<Merchant> Items { get; set; }
	}
}