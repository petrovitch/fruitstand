using System;
using System.Web;
using System.Web.Security;
using Contracts.Cache;

namespace Web.Code.Web
{
	public class WebUser 
	{

		private IUserDataManager _DataManager = null;
		public IUserDataManager DataManager
		{
			get
			{
				if (_DataManager == null)
				{
					_DataManager = new SessionStorage();
					_DataManager.UniqueBaseKey = SessionID;
				}
				return _DataManager;
			}
		}

		private string SessionID
		{
			get
			{
				return "";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name)) return true;
				return false;
			}
		}

		private Guid? _PersonID = null;
		public Guid? PersonID
		{
			get
			{
				// Check in-memory first
				if (_PersonID.HasValue) return _PersonID.Value;

				// Create in session automatically for this demo site
				var sessionVal = HttpContext.Current.Session["personid"];
				if (sessionVal == null)
				{
					sessionVal = Guid.NewGuid();
					this.PersonID = (Guid)sessionVal;
				}
				return (Guid) sessionVal;

			}
			private set
			{
				_PersonID = value;
				HttpContext.Current.Session["personid"] = value;
			}
		}

		#region Sign In / Out

		/// <summary>
		/// Adds this person to our forms authentication
		/// </summary>
		/// <param name="personID"></param>
		/// <returns></returns>
		public void SignIn(Guid personID)
		{
			// Clear existing cookies
			ClearCookies();

			// Add here also so that subsequent calls in the same request can get the just-signed-in user
			this._PersonID = personID;

			FormsAuthentication.SetAuthCookie(personID.ToString(), true);
		}

		/// <summary>
		/// Clear cookies
		/// </summary>
		private void ClearCookies()
		{
			// Clear session
			if (HttpContext.Current != null)
			{
				if (HttpContext.Current.Session != null) HttpContext.Current.Session.Clear();
				HttpContext.Current.Response.Cookies.Clear();
			}

			// Clear local variables
			this.PersonID = null;
		}

		/// <summary>
		/// Removes the person from the authentication system
		/// </summary>
		public void SignOut()
		{
			// Kill current user
			ClearCookies();
			FormsAuthentication.SignOut();
		}

		#endregion

	}
}
