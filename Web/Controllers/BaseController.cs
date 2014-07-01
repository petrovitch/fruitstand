using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Contracts.Enums;
using Contracts.Exceptions;
using Web.Code.Common.Extensions;
using Web.Code.Web;
using Web.Models;

namespace Web.Controllers
{
	public class BaseController : System.Web.Mvc.Controller
	{
		private WebUser _CurrentUser = null;
		protected WebUser CurrentUser
		{
			get
			{
				if (_CurrentUser == null) _CurrentUser = new WebUser();
				return _CurrentUser;
			}
		}

		/// <summary>
		/// Global error handling
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnException(ExceptionContext filterContext)
		{
			// Format response according to the requested return type
			var returnType = ReturnTypes.html;
			var acceptType = (Request.Headers["accept"] ?? "").ToLower();
			if (acceptType.Contains("javascript")) returnType = ReturnTypes.json;

			// Record
			if (filterContext.Exception != null)
			{

				var myException = filterContext.Exception;
				if (!(myException is UserException))
				{
					// TODO: Log error and return nice general error message instead, but for this example site, full error is shown
					myException = new UserException(myException.ToString());
				}

				// Show nice message if one of our user exceptions
				filterContext.ExceptionHandled = true;

				var errorModel = new ErrorModel();
				if (myException is UserExceptionCollection)
				{
					var exceptions = ((UserExceptionCollection)myException).Exceptions;
					exceptions.ForEach(x => errorModel.Exceptions.Add(x.Message));
				}
				else
				{
					errorModel.Exceptions.Add(myException.Message);
				}
				 
				// If JSON, we can just fallback to the global ajax error handler written in client-side
				switch (returnType)
				{
					case ReturnTypes.json:
						filterContext.HttpContext.Response.Clear();
						filterContext.HttpContext.Response.ContentEncoding = Encoding.UTF8;
						filterContext.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
						filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
						filterContext.HttpContext.Response.StatusCode = 200;
						filterContext.Result = Json(errorModel, JsonRequestBehavior.AllowGet);
						break;
					default:
						// If this is a partial request, we don't want to render the header etc down - just the message
						// Note that IsChildAction only works if it is rendered using RenderAction(), not RenderPartial()
						if (Request.IsAjaxRequest() || filterContext.IsChildAction)
						{
							filterContext.HttpContext.Response.Clear();
							filterContext.HttpContext.Response.ContentEncoding = Encoding.UTF8;
							filterContext.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
							filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
							filterContext.HttpContext.Response.StatusCode = 500;
							filterContext.Result = PartialView("home/error", errorModel);
						}
						else
						{
							filterContext.Result = View("Home/Error", errorModel);
						}
						break;

				}
				
				return;
			}

			// Ultimately, defer to base
			base.OnException(filterContext);
		}
	}
}