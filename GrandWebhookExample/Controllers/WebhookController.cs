using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GrandWebhookExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandWebhookExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private const string _grandSecret = "c181f35e-79df-4376-9cc7-378a44128b86";
        private const int _test = 0;
        private const int _emailSubscribed = 40;
        private const int _orderPlaced = 20;
        private const int _stockUpdate = 10;
        private readonly ILogger _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }


        [HttpPost("Post")]
        public IActionResult Post([FromBody]GrandWebhook grandWebhook)
        {
            if (HttpContext.Request.Headers.TryGetValue("grand-webhook", out var signatureHeader))
            {
                //format => sh256=signature
                var signature = signatureHeader.ToString().Split("=").Last();
                if (VerifySignarure(grandWebhook.Body, _grandSecret, signature))
                {
                    //Check type then cast and do somethink 
                    if (grandWebhook.Type.Equals(_test))
                    {
                        var testHook = JsonSerializer.Deserialize<TestHook>(grandWebhook.Body);
                        _logger.LogInformation($"Receive test webhook form grandnode with message: {testHook.Message} ");
                    }
                    if (grandWebhook.Type.Equals(_emailSubscribed))
                    {
                        var emailModel = JsonSerializer.Deserialize<EmailModel>(grandWebhook.Body);
                        _logger.LogInformation($"Receive email subscribed webhook with email: {emailModel.Email} ");
                    }
                    if (grandWebhook.Type.Equals(_orderPlaced))
                    {
                        var order = JsonSerializer.Deserialize<OrderModel>(grandWebhook.Body);
                        _logger.LogInformation($"Receive order with id: {order.Id} , Order total {order.OrderTotal} ");
                    }
                    if (grandWebhook.Type.Equals(_stockUpdate))
                    {
                        var product = JsonSerializer.Deserialize<SimpleProductModel>(grandWebhook.Body);
                        _logger.LogInformation($"Receive update stock webhook with product id :{product.Id} and stock {product.Stock}");
                    }
                }
                else return BadRequest();
            }
            else return BadRequest();
            
            return Ok();
        }

        private bool VerifySignarure(string body ,string secret,string grandSignature)
        {
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            using var hasher = new HMACSHA256(secretBytes);
            var data = Encoding.UTF8.GetBytes(body);
            var signatureBytes = hasher.ComputeHash(data);
            var signature=BitConverter.ToString(signatureBytes);
            return signature.Equals(grandSignature);
        }
    }
}
