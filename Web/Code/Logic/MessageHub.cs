
using Microsoft.AspNet.SignalR;
using Web.Code.Contracts.Entities;
using Web.Code.Contracts.Exceptions;

namespace Web.Code.Logic
{
	/// <summary>
	/// Allows us to broadcast server notifications in real-time, such that the client application can display feedback to the user
	/// </summary>
	public class MessageHub : Hub
	{

		private IHubContext Connection
		{
			get
			{
				return GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
			}
		}

		/// <summary>
		/// Notifies listeners that the API threw the given error
		/// </summary>
		/// <param name="errorMessage"></param>
		public void APIError(ApiResponseException errorMessage)
		{
			this.Connection.Clients.All.APIError(errorMessage);
		}

		/// <summary>
		/// The given request is being sent to our API
		/// </summary>
		/// <param name="requestDetails"></param>
		public void APIRequestSent(RequestDetails requestDetails)
		{
			this.Connection.Clients.All.APIRequestSent(requestDetails);
		}

		public void APIResponseReceived(string json)
		{
			this.Connection.Clients.All.APIResponseReceived(json);
		}
	}
}
