using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Code.Common;
using Web.Code.Contracts.Entities;
using Web.Code.Contracts.Entities.ApiModels;
using Web.Code.Web;

namespace Web.Code.Logic
{
	/// <summary>
	///     Handles the saving and removal of the user's current basket of items
	/// </summary>
	public class ShoppingCart
	{
		public List<CartItem> Items { get; private set; }

		public double SubTotal
		{
			get
			{
				List<Product> products = new DataRepository().GetProducts();

				double total = 0.0;
				foreach (CartItem cartItem in Items)
				{
					Product product = products.FirstOrDefault(x => x.ProductID == cartItem.ProductID);
					if (product == null) continue;
					total += (product.UnitPrice*cartItem.Quantity);
				}

				return total;
			}
		}

		public double TaxRate
		{
			get { return Configuration.Current.TaxPercentage; }
		}

		public double Tax
		{
			get { return SubTotal*TaxRate/100; }
		}

		public double GrandTotal
		{
			get { return SubTotal + Tax; }
		}

		/// <summary>
		///     Constructor
		/// </summary>
		public ShoppingCart()
		{
			Items = new List<CartItem>();
		}

		/// <summary>
		///     Formats our selected items to make a nice description of the overall basket
		/// </summary>
		/// <returns></returns>
		private string GetBasketContentsDescription()
		{
			List<Product> products = new DataRepository().GetProducts();

			var desc = new List<string>();
			foreach (CartItem cartItem in Items)
			{
				if (cartItem.Quantity <= 0) continue;

				Product product = products.FirstOrDefault(x => x.ProductID == cartItem.ProductID);
				if (product == null) continue;
				desc.Add(cartItem.Quantity.ToString("0.#") + " x " + product.Name);
			}

			return string.Join(Environment.NewLine, desc);
		}

		/// <summary>
		///     Calculates the payment info via Pushpay, then redirects the user to the secure URL where they can enter their
		///     payment details
		/// </summary>
		public async Task<AnticipatedPaymentRepresentation> FinalizePayment(string merchantKey)
		{
			// Create a model for PP
			var paymentDetails = new EditAnticipatedPaymentModel();
			paymentDetails.Payer = new PayerDetails {EmailAddress = "bruce@example.com", FullName = "Pushpay API"};

			// The amount is stored in a config field
			paymentDetails.Fields.Add(new FieldConfigModel
			{
				Key = "amount",
				Value = GrandTotal.ToString("0.00"),
				ReadOnly = true
			});

			// Other info
			paymentDetails.Description = GetBasketContentsDescription();
			paymentDetails.DescriptionTitle = "Fruit purchase";
			paymentDetails.MerchantKey = merchantKey;
			paymentDetails.Reference = Guid.NewGuid().ToString(); // Would typically be your own invoice/reference number according to your e-commerce shopping cart
			paymentDetails.ReturnTitle = "Return to the example site";
			paymentDetails.ReturnUrl = new WebEnvironment().GetFullUrl("home/paymentcomplete", true);

			// Send to pushpay
			var connection = new PushpayConnection();
			AnticipatedPaymentRepresentation response = await connection.SendPaymentToPushpay(paymentDetails);
			return response;
		}

		/// <summary>
		///     Adds a new item to this cart, or updates the quantity if it already exists
		/// </summary>
		/// <param name="productID"></param>
		/// <param name="quantity"></param>
		public void UpdateQuantity(int productID, double quantity)
		{
			// Delete?
			if (quantity <= 0)
			{
				DeleteProduct(productID);
				return;
			}

			// Get or create the item from our existing basket
			CartItem p = Items.FirstOrDefault(x => x.ProductID == productID);
			if (p == null)
			{
				p = new CartItem {ProductID = productID};
				Items.Add(p);
			}

			// Update the quantity
			p.Quantity = quantity;
		}

		/// <summary>
		///     Removes this product from our cart
		/// </summary>
		/// <param name="productID"></param>
		public void DeleteProduct(int productID)
		{
			CartItem p = Items.FirstOrDefault(x => x.ProductID == productID);
			if (p == null) return;
			Items.Remove(p);
		}
	}
}