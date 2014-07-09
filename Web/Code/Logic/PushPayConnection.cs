using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Contracts.Exceptions;
using Thinktecture.IdentityModel.Client;
using Web.Code.Contracts.Entities.ApiModels;
using Web.Code.Contracts.Enums;

namespace Web.Code.Logic
{
	/// <summary>
	/// Centralized logic for communicating with the Pushpay payment server
	/// </summary>
	public class PushpayConnection
	{
		#region Properties

		private ApiClient _Client = null;

		#endregion

		/// <summary>
		/// Helper method to create a client connection
		/// </summary>
		/// <returns></returns>
		private async Task<ApiClient> CreateClient()
		{
			if (_Client == null)
			{
				var baseUrl = Common.Configuration.Current.PushpayAPIBaseUrl;
				if (string.IsNullOrWhiteSpace(baseUrl)) RaiseError(new Exception("Please provide a PushpayAPIBaseUrl in your configuration AppSettings"));
				_Client = new ApiClient(baseUrl);

				// Authenticate
				var clientID = Common.Configuration.Current.PushpayClientID;
				var clientSecret = Common.Configuration.Current.PushpayClientSecret;
				if (string.IsNullOrWhiteSpace(clientID) || string.IsNullOrWhiteSpace(clientSecret)) RaiseError(new Exception("Please provide Pushpay client ID and secret tokens in your configuration file"));

				// Create an OAuth client to get the token required by Pushpay
				var oauthClient = new OAuth2Client(new Uri(Common.Configuration.Current.OAuth2TokenEndpoint), clientID, clientSecret);
				var response = await oauthClient.RequestClientCredentialsAsync("create_anticipated_payment read");
				if (response.AccessToken == null) RaiseError(new Exception("Failed to retrieve access token, error was: " + response.Raw));
				_Client.SetBearerToken(response.AccessToken);
			}
			return _Client;
		}

		/// <summary>
		/// Centralized error handling
		/// </summary>
		/// <param name="ex"></param>
		private void RaiseError(Exception ex)
		{
			throw ex;
		}

		/// <summary>
		/// Returns a list of merchant info
		/// </summary>
		public async Task<List<Merchant>> GetMerchants(string name)
		{
			var client = await this.CreateClient();
			var result = await client.Init("merchants", "Loading merchant list").AddParam("name", name).Execute<MerchantSearchResult>();
			return result.Items;
		}

		/// <summary>
		/// Sends the payment details to Pushpay, which returns with a URL where we redirect the user for payment
		/// </summary>
		/// <param name="paymentDetails"></param>
		/// <returns></returns>
		public async Task<AnticipatedPaymentRepresentation> SendPaymentToPushpay(EditAnticipatedPaymentModel paymentDetails)
		{
			var client = await this.CreateClient();
			var result = await client.Init("anticipatedpayments", "Sending payment to Pushpay").SetMethod(RequestMethodTypes.POST).SetContent(paymentDetails).Execute<AnticipatedPaymentRepresentation>();
			return result;
		}

		/// <summary>
		/// Returns information about the requested payment
		/// </summary>
		/// <param name="anticipatedPaymentToken"></param>
		/// <returns></returns>
		public async Task<AnticipatedPaymentRepresentation> GetPaymentInfo(string anticipatedPaymentToken)
		{
			var client = await this.CreateClient();
			var result = await client.Init("anticipatedpayment/" + anticipatedPaymentToken, "Loading payment information").SetMethod(RequestMethodTypes.GET).Execute<AnticipatedPaymentRepresentation>();
			return result;
		}
	}
}