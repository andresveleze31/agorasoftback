using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Db;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly StripeModel model;
        private readonly TokenService _tokenService;
        private readonly CustomerService _customerService;
        private readonly ChargeService _chargeService;
        private readonly ProductService _productService;

        private readonly OrganizationService _organizationService;

        private readonly AppDbContext _dbContext;


        public StripeController(
            IOptions<StripeModel> _model,
            TokenService tokenService,
            CustomerService customerService,
            OrganizationService organizationService,
            ChargeService chargeService,
            ProductService productService,
            AppDbContext dbContext)
        {
            model = _model.Value;
            _tokenService = tokenService;
            _customerService = customerService;
            _chargeService = chargeService;
            _organizationService = organizationService;
            _productService = productService;
            _dbContext = dbContext;
        }

        // ✅ Clase para recibir JSON del frontend
        // ✅ Clase para recibir JSON del frontend
        public class SubscribeRequest
        {
            public string PriceId { get; set; } = string.Empty;
            public string OrganizationId { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string SuccessUrl { get; set; } = "http://localhost:4200/admin/success";
            public string CancelUrl { get; set; } = "http://localhost:4200/admin/error";
        }


        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                StripeConfiguration.ApiKey = model.SecretKey;

                // 1. Crear o obtener el cliente en Stripe
                var customerOptions = new CustomerCreateOptions
                {
                    Email = request.CustomerEmail,
                    Name = request.CustomerName,
                    Metadata = new Dictionary<string, string>
                    {
                        { "organization_id", request.OrganizationId.ToString() }
                    }
                };

                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);

                // 2. Crear la sesión de checkout
                var options = new SessionCreateOptions
                {
                    Customer = customer.Id,
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = request.PriceId,
                            Quantity = 1
                        }
                    },
                    Mode = "subscription",
                    SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}&organization_id={request.OrganizationId}",
                    CancelUrl = request.CancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "organization_id", request.OrganizationId.ToString() }
                    }
                };

                var service = new SessionService();
                Session session = service.Create(options);

                // 3. Crear registro inicial de suscripción en la base de datos
                var subscription = new Models.Subscription
                {
                    StripeCustomerId = customer.Id,
                    PriceId = request.PriceId,
                    OrganizationId = request.OrganizationId.ToString(),
                    Status = "pending", // Estado inicial hasta que se complete el pago
                    CurrentPeriodStart = DateTime.UtcNow,
                    CurrentPeriodEnd = DateTime.UtcNow.AddDays(30), // Estimación inicial
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Subscriptions.Add(subscription);
                await _dbContext.SaveChangesAsync();

                await _organizationService.ActivateOrganizationAsync(request.OrganizationId.ToString());

                return Ok(new { url = session.Url, sessionId = session.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CreateCustomer")]
        public async Task<dynamic> CreateCustomer([FromBody] StripeCustomer customerInfo)
        {
            StripeConfiguration.ApiKey = model.SecretKey;

            var customerOptions = new CustomerCreateOptions
            {
                Email = customerInfo.Email,
                Name = customerInfo.Name
            };

            var customer = await _customerService.CreateAsync(customerOptions);
            return new { customer };
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            StripeConfiguration.ApiKey = model.SecretKey;

            var options = new ProductListOptions
            {
                Expand = new List<string>() { "data.default_price" }
            };

            var products = _productService.List(options);
            return Ok(products);
        }
    }
}
