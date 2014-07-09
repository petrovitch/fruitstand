using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public partial class AnticipatedPaymentRepresentation
	{
		public AnticipatedPaymentRepresentation()
		{
			Fields = new List<FieldConfigModel>();
		}

		[JsonProperty("_links")]
		public Dictionary<string, Link> Links { get; set; }

		/// <summary>
		/// Helper function to serialize this payment url from our link collection
		/// </summary>
		public string PaymentUrl
		{
			get
			{
				if (this.Links == null || !this.Links.ContainsKey("pay")) return "";
				return this.Links["pay"].Href;
			}
		}

		public string Description { get; set; }
		public string DescriptionTitle { get; set; }
		public List<FieldConfigModel> Fields { get; set; }
		public int MerchantId { get; set; }
		public PayerDetails Payer { get; set; }
		public string Reference { get; set; }
		public string ReturnTitle { get; set; }
		public string ReturnUrl { get; set; }
		public string Token { get; set; }
	}
}