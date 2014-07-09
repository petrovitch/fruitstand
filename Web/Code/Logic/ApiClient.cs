using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Contracts.Exceptions;
using Newtonsoft.Json;
using Web.Code.Common.Extensions;
using Web.Code.Contracts.Entities;
using Web.Code.Contracts.Entities.ApiModels;
using Web.Code.Contracts.Enums;
using Web.Code.Contracts.Exceptions;

namespace Web.Code.Logic
{
	/// <summary>
	/// Connects to any arbitrary REST-based API
	/// </summary>
	public class ApiClient
	{

		#region Properties

		private DeveloperMessageHub MessageHub = null;
	
		private HttpClient _client;
		private readonly MediaTypeFormatter[] _formatters;

		private RequestDetails RequestDetails = new RequestDetails();

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"></param>
		public ApiClient(string baseUrl)
		{
			this.RequestDetails.BaseUrl = baseUrl;

			// Our API formats everything as JSON
			var jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
			_formatters = new MediaTypeFormatter[] { jsonFormatter };


			// Client connection
			CreateClient();
			Reset();
		}

		/// <summary>
		/// Serializes the given object and sends to the API. Used for sending large request information to PP, which is too big or impractical to fit into GET/FORM values
		/// </summary>
		/// <param name="objectToSerializeAndSendToApi"></param>
		/// <returns></returns>
		public ApiClient SetContent(object objectToSerializeAndSendToApi)
		{
			this.RequestDetails.SerializedContent = objectToSerializeAndSendToApi.ToJSON();
			return this;
		}

		/// <summary>
		/// Initialises our API connection
		/// </summary>
		/// <param name="relativeUrl">The URL of our API call, relative to the base APi URL. For example, the base URL may be 'http://api.com/v1/' and this relative Url may be 'Merchants/GetMerchants'</param>
		/// <param name="description">Optional description which we simply propogate through to our developer UI to make debugging easier. Entirely optional</param>
		/// <returns></returns>
		public ApiClient Init(string relativeUrl,string description = "")
		{
			this.Reset();
			this.RequestDetails.RelativeUrl = relativeUrl;

			// Create a developer log so we can broadcast API activity
			this.MessageHub = new DeveloperMessageHub() { TransactionID = Guid.NewGuid(), Description = description };

			// Return self to facilitate call chaining
			return this;
		}

		/// <summary>
		/// Clears our parameters etc, so we can make another call
		/// </summary>
		private ApiClient Reset()
		{
			this.RequestDetails.ClearParameters();;

			// Return self to facilitate call chaining
			return this;
		}

		/// <summary>
		/// Appends a parameter to our API call
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public ApiClient AddParam(string key, object value)
		{
			this.RequestDetails.SetParameter(key, value);
			
			// Return self to facilitate call chaining
			return this;
		}

		/// <summary>
		/// Parses our query values into a message request, designed to be sent to our API
		/// </summary>
		/// <returns></returns>
		private HttpRequestMessage BuildRequest()
		{
			// Create the URL of our client call, based on the query string parameters the user has loaded up
			var url = this.RequestDetails.FullUrl;

			// Create our request
			var request = new HttpRequestMessage(new HttpMethod(this.RequestDetails.Method), url);

			//  If we have large object, we may be serialising it directly into the request body. Otherwise, if the query is POST, then we add our parameters to the request body
			if (!string.IsNullOrWhiteSpace(this.RequestDetails.SerializedContent))
			{
				request.Content = new StringContent(this.RequestDetails.SerializedContent, Encoding.UTF8, "application/json");
			}
			if (this.RequestDetails.FormValues.Any())
			{
				// Just encode our parameters to key/value pairs
				var queryValues = this.RequestDetails.FormValues.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value == null ? null : kvp.Value.ToString()));
				request.Content = new FormUrlEncodedContent(queryValues);
			}

			// Return
			return request;
		}

		/// <summary>
		/// Executes are request and parses into the requested type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual async Task<T> Execute<T>() where T : BaseResponse
		{

			// Create our requested, based on the various parameters we have for this call
			var request = this.BuildRequest();

			// Debug - notify listeners that we are making this request
			this.MessageHub.APIRequestSent(this.RequestDetails);

			// Send to the API
			var response = await _client.SendAsync(request);
			
			// Debug - get the raw JSON out so we can debug
			var responseDetails = new ResponseDetails();
			responseDetails.StatusCode = ((int)response.StatusCode).ToString() + " " + response.StatusCode.ToString();

			responseDetails.JSON = await response.Content.ReadAsStringAsync();

			// If it's an error, we parse out exception and throw
			if (!response.IsSuccessStatusCode)
			{
				ApiResponseException exception;
				try
				{
					var errorResponse = responseDetails.JSON.FromJSON<ErrorResponse>();
					exception = new ApiResponseException(response.StatusCode, response.ReasonPhrase, errorResponse);
				}
				catch (Exception ex)
				{
					// If it's not JSON formatted, we just render a general message
					exception = new ApiResponseException(HttpStatusCode.BadRequest, "A big exception occurred", new ErrorResponse() {Message = responseDetails.JSON});
				}
				this.MessageHub.APIError(exception);
				
				// Friendlier message for our UI
				throw new UserException(exception.Message);
			}
			
			// Parse and return
			this.MessageHub.APIResponseReceived(responseDetails);
			var result = responseDetails.JSON.FromJSON<T>();
 
			return result;
		}


		/// <summary>
		/// Creates our client connection to the API
		/// </summary>
		private void CreateClient()
		{
			_client = new HttpClient { BaseAddress = new Uri(this.RequestDetails.BaseUrl) };
			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/hal+json"));
		}

		/// <summary>
		/// Sets the OAuth bearer token, used to authenticate our requests
		/// </summary>
		/// <param name="token"></param>
		public virtual void SetBearerToken(string token)
		{
			_client.SetBearerToken(token);
		}

		/// <summary>
		/// Sets the method to one of POST/GET/DELETE/PUT
		/// </summary>
		/// <param name="methodType"></param>
		public ApiClient SetMethod(RequestMethodTypes methodType)
		{
			this.RequestDetails.Method = methodType.ToString();
			return this;
		}
	}
}