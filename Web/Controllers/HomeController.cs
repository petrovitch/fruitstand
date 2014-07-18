using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Code;
using Web.Code.Common;
using Web.Code.Contracts.Entities.ApiModels;
using Web.Code.Contracts.Exceptions;
using Web.Code.Logic;
using Web.Code.Web;
using Web.Models.Home;

namespace Web.Controllers
{
	public class HomeController : BaseController
	{
		/// <summary>
		///     Creates a basket and pushes a product request to Pushpay
		/// </summary>
		/// <param name="products"></param>
		/// <returns></returns>
		public async Task<ActionResult> SaveOrder(string products, string merchantID)
		{
			if (string.IsNullOrWhiteSpace(merchantID)) throw new UserException("Please select a merchant");

			// Open our cart
			var cart = new ShoppingCart();

			// The products are just strung together like |product1=3|product2=6.7| etc etc
			foreach (string productDesc in products.Split('|'))
			{
				string[] parts = productDesc.Split('=');
				if (parts.Length != 2) continue;
				int productID = int.Parse(parts[0]);
				double quantity = double.Parse(parts[1]);
				cart.UpdateQuantity(productID, quantity);
			}

			// Now process and redirect to our URL
			AnticipatedPaymentRepresentation paymentResponse = await cart.FinalizePayment(merchantID);
			return Json(paymentResponse, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		///     Called by the Pushpay API after a payment is made
		/// </summary>
		/// <param name="ap"></param>
		/// <returns></returns>
		public async Task<ActionResult> PaymentComplete(string ap)
		{
			var model = new PaymentCompleteModel();
			model.PaymentInfo = await new PushpayConnection().GetPaymentInfo(ap);

			if (model.PaymentInfo == null)
			{
				model.IsError = true;
				model.ErrorMessage = "The payment token '" + ap + "' is no longer valid. Perhaps your purchase session timed out?";
			}

			model.StatusInfo = await new PushpayConnection().GetPaymentStatus(ap);

			if (model.StatusInfo.Status == "UserCancelled" || model.StatusInfo.Status == "Unassociated")
			{
				// user cancelled paying or never started the payment process... redirect to the start
				return RedirectToAction("Index");
			}

			if (model.StatusInfo.Status != "Completed")
			{
				model.IsError = true;
				model.ErrorMessage = GetMessageForStatus(model.StatusInfo.Status);
			}

			return View(model);
		}

		private string GetMessageForStatus(string status)
		{
			switch (status)
			{
				case "Processing":
					return "Payment has not yet been completed";
				case "Failed":
					return "Payment failed, order has been cancelled.";
				default:
					return string.Format("Unexpected payment status '{0}'", status);
			}
		}

		/// <summary>
		///     Opens the developer console in a separate VIEW
		/// </summary>
		/// <returns></returns>
		public ActionResult DeveloperConsole()
		{
			return View();
		}

		/// <summary>
		///     Returns a view which shows the real-time messages that our API is sending/receiving. Designed to assist developers
		///     with understanding how the API works
		/// </summary>
		/// <returns></returns>
		public ActionResult APILogViewer()
		{
			var model = new APILogViewerModel {CurrentUserID = CurrentUser.PersonID};
			return PartialView("home/apilogviewer", model);
		}

		/// <summary>
		///     Returns a link to the product stock image
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		public ActionResult ProductImage(int productID)
		{
			// Nothing fancy here, just redirect to hard-coded image files for this demo app
			string path = new WebEnvironment().MapPath("~/content/i/products/" + productID + ".png");
			return File(path, "image/png");
		}

		/// <summary>
		///     Returns our list of merchants
		/// </summary>
		/// <returns></returns>
		public async Task<ActionResult> MerchantList()
		{
			var model = new MerchantListModel();
			model.Merchants = await new PushpayConnection().GetMerchants("");
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		///     Shows a page allowing the user to select products
		/// </summary>
		/// <returns></returns>
		public async Task<ActionResult> BrowseProducts()
		{
			var model = new BrowseProductsModel();
			model.Products = new DataRepository().GetProducts();
			model.TaxPercentage = Configuration.Current.TaxPercentage;

			// Load current merchant
			model.Merchants = await new PushpayConnection().GetMerchants("");
			model.CurrentMerchant = await new PushpayConnection().GetMerchant(Configuration.Current.MerchantID);

			return View(model);
		}

		/// <summary>
		///     Entry point / home page into the application
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			return RedirectToAction("BrowseProducts", "Home");
		}

		/// <summary>
		///     A landing page where the user can get started with the API
		/// </summary>
		/// <returns></returns>
		public ActionResult Start()
		{
			return View();
		}
	}
}