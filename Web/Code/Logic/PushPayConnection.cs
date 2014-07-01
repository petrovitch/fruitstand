using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Contracts.Exceptions;
using Pushpay.Api.Client;

namespace Web.Code.Logic
{
	/// <summary>
	/// Centralized logic for communicating with the Pushpay payment server
	/// </summary>
	public class PushpayConnection
	{

		#region Properties

		private ApiClient _Client = null;
		private ApiClient Client
		{
			get
			{
				if (_Client == null)
				{
					var baseUrl = ConfigurationManager.AppSettings["PushpayAPIBaseUrl"];
					if (string.IsNullOrWhiteSpace(baseUrl)) throw new UserException("Please provide a PushpayAPIBaseUrl in your configuration AppSettings");
					_Client = new ApiClient(baseUrl);
				}
				return _Client;
			}
		}

		#endregion

		/// <summary>
		/// Constructor / entry point
		/// </summary>
		public PushpayConnection()
		{
			Authenticate();
		}

		/// <summary>
		/// Passes our client credentials to Pushpay
		/// </summary>
		private void Authenticate()
		{
			this.Client.
		}
	}
}