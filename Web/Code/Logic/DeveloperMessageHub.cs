
using System;
using Microsoft.AspNet.SignalR;
using Web.Code.Contracts.Entities;
using Web.Code.Contracts.Enums;
using Web.Code.Contracts.Exceptions;
using Web.Code.Web;

namespace Web.Code.Logic
{
	/// <summary>
	/// Allows us to broadcast server notifications in real-time, such that the client application can display feedback to the user
	/// </summary>
	public class DeveloperMessageHub : Hub
	{
		#region Properties

		public Guid TransactionID = Guid.Empty;

		private IHubContext Connection
		{
			get
			{
				return GlobalHost.ConnectionManager.GetHubContext<DeveloperMessageHub>();
			}
		}

		public string Description = "";

		#endregion

		/// <summary>
		/// Allows the person to join this group
		/// </summary>
		/// <param name="groupType"></param>
		/// <param name="groupID"></param>
		public void JoinGroup(MessageBroadcasterGroupTypes groupType, object groupID)
		{
			if (groupID == null) return;
			var groupName = groupType.ToString() + "_" + groupID.ToString();
			this.Connection.Groups.Add(Context.ConnectionId, groupName);
		}

		/// <summary>
		/// Notifies listeners that the API threw the given error
		/// </summary>
		/// <param name="errorMessage"></param>
		public void APIError(ApiResponseException errorMessage)
		{
			this.Connection.Clients.Group(GetCurrentAPIRequestGroupName()).APIError(errorMessage, this.TransactionID, this.Description);
		}

		public void APIResponseReceived(ResponseDetails responseDetails)
		{
			this.Connection.Clients.Group(GetCurrentAPIRequestGroupName()).APIResponseReceived(responseDetails, this.TransactionID, this.Description);
		}

		/// <summary>
		/// The given request is being sent to our API
		/// </summary>
		/// <param name="requestDetails"></param>
		public void APIRequestSent(RequestDetails requestDetails)
		{
			this.Connection.Clients.Group(GetCurrentAPIRequestGroupName()).APIRequestSent(requestDetails, this.TransactionID, this.Description);
		}

		/// <summary>
		/// Formats the unique 
		/// </summary>
		/// <returns></returns>
		public string GetCurrentAPIRequestGroupName()
		{
			return MessageBroadcasterGroupTypes.APIRequestsForUser.ToString() + "_" + new WebUser().PersonID.GetValueOrDefault(Guid.Empty).ToString();
		}
	}
}
