using IBuyStuff.Application.Commands;
using IBuyStuff.Application.InputModels.Order;
using IBuyStuff.Application;
using IBuyStuff.Application.Services;
using IBuyStuff.Application.ViewModels.Orders;
using IBuyStuff.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IBuyStuff.Persistence.Facade;
using IBuyStuff.Domain.Products;
using IBuyStuff.Domain.Shared;
using IBuyStuff.Persistence.Utils;

namespace IBuyStuffer.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderControllerService _service;
       
        public OrderController(IOrderControllerService service)
        {
            _service = service;
        }
      

        #region Search task

        [HttpGet]
       
        public IActionResult SearchMain()
        {
            return Ok(new ViewModelBase());
        }

        [HttpGet]
       
        public ActionResult SearchResults(int id)
        {
            var model = _service.RetrieveOrderForCustomer(id);
            return Ok(model);
        }


        [HttpGet]
       
        public ActionResult LastOrderResults()
        {
            var customerId = User.Identity.Name;
            var model = _service.RetrieveLastOrderForCustomer(customerId);
            return Ok(model);
        }

        #endregion

        #region New Order task
        [HttpGet]
       
        public ActionResult New()
        {
            var customerId = User.Identity.Name;
            var shoppingCartModel = _service.CreateShoppingCartForCustomer(customerId);
            shoppingCartModel.EnableEditOnShoppingCart = true;
            SaveCurrentShoppingCart(shoppingCartModel);
            return Ok(shoppingCartModel);
        }
        #endregion

        #region Add Item task
        [HttpGet]
      
        public ActionResult AddToShoppingCartCommand(int productId, int quantity = 1)
        {
            var cart = RetrieveCurrentShoppingCart();
            cart = _service.AddProductToShoppingCart(cart, productId, quantity);
            SaveCurrentShoppingCart(cart);
            cart.EnableEditOnShoppingCart = true;
            // PRG (Post-Redirect-Get) pattern to avoid F5 refresh issues 
            // (and also key step to neatly separate Commands from Queries in the future)
            return Ok(cart);
        }

        #endregion

        #region Remove Order Item task

      
        [HttpGet]
      
        public ActionResult RemoveItemFromShoppingCart(int itemIndex = -1)
        {
            if (itemIndex < 0)
                return Ok("参数错误");

            var cart = RetrieveCurrentShoppingCart();
            if (itemIndex >= cart.OrderRequest.Items.Count)
                return Ok("索引超出范围");

            cart.OrderRequest.Items.RemoveAt(itemIndex);
            SaveCurrentShoppingCart(cart);

            // PRG (Post-Redirect-Get) pattern to avoid F5 refresh issues 
            // (and also key step to neatly separate Commands from Queries in the future)
            return Ok("success");
        }

        #endregion

        #region Checkout

        
        [HttpGet]
        
        public ActionResult Checkout(CheckoutInputModel checkout)
        {
            // Pre-payment steps
            var cart = RetrieveCurrentShoppingCart();
            var command = new ProcessOrderBeforePaymentCommand(cart, checkout);
            var response = CommandProcessor.Send<ProcessOrderBeforePaymentCommand, OrderProcessingViewModel>(command);
            cart.EnableEditOnShoppingCart = false;
           return Ok(response);
        }

       
        #endregion


        #region Internal members

        private static string GetShoppingCartName(string customerId)
        {
            return String.Format("I-Buy-Stuff-Cart:{0}", customerId);
        }
        private ShoppingCartViewModel RetrieveCurrentShoppingCart()
        {
            var customerId = "naa4e"; //User.Identity.Name 
            var cartName = GetShoppingCartName(customerId);
            var cart =  _service.CreateShoppingCartForCustomer(customerId);
            return cart;
        }
        private void SaveCurrentShoppingCart(ShoppingCartViewModel cart)
        {
            var customerId = User.Identity.Name;
            var cartName = GetShoppingCartName(customerId);
        }
        #endregion
    }
}
