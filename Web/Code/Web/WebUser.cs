using System;
using System.Web;
using System.Web.Security;
using Web.Code.Contracts.Cache;

namespace Web.Code.Web
{
	public class WebUser
	{
		private Guid? _personId;
		private IUserDataManager _dataManager;

		public IUserDataManager DataManager
		{
			get
			{
				if (_dataManager == null)
				{
					_dataManager = new SessionStorage();
					_dataManager.UniqueBaseKey = SessionID;
				}
				return _dataManager;
			}
		}

		private string SessionID
		{
			get { return ""; }
		}

		public bool IsAuthenticated
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name)) return true;
				return false;
			}
		}

		public Guid? PersonID
		{
			get
			{
				// Check in-memory first
				if (_personId.HasValue) return _personId.Value;

				// Create in session automatically for this demo site
				object sessionVal = HttpContext.Current.Session["personid"];
				if (sessionVal == null)
				{
					sessionVal = Guid.NewGuid();
					PersonID = (Guid) sessionVal;
				}
				return (Guid) sessionVal;
			}
			private set
			{
				_personId = value;
				HttpContext.Current.Session["personid"] = value;
			}
		}

		#region Sign In / Out

		/// <summary>
		///     Adds this person to our forms authentication
		/// </summary>
		/// <param name="personID"></param>
		/// <returns></returns>
		public void SignIn(Guid personID)
		{
			// Clear existing cookies
			ClearCookies();

			// Add here also so that subsequent calls in the same request can get the just-signed-in user
			_personId = personID;

			FormsAuthentication.SetAuthCookie(personID.ToString(), true);
		}

		/// <summary>
		///     Clear cookies
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
			PersonID = null;
		}

		/// <summary>
		///     Removes the person from the authentication system
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