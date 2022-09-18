using Braintree;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BulkyBook.MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class BrainTreeController : Controller
{
    private readonly IBrainTreeGateway _brainTreeGateway;

    public BrainTreeController(IBrainTreeGateway brainTreeGateway)
    {
        this._brainTreeGateway = brainTreeGateway;
    }

    public IActionResult Index()
    {
        var gateway = _brainTreeGateway.GetGateway();
        var clientToken = gateway.ClientToken.Generate();
        ViewBag.ClientToken = clientToken;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(IFormCollection collection)
    {
        Random rnd = new Random();
        string nonceFromTheClient = collection["payment_method_nonce"];
        var request = new TransactionRequest
        {
            Amount = rnd.Next(1, 100),
            PaymentMethodNonce = nonceFromTheClient,
            OrderId = "1111",
            Options = new TransactionOptionsRequest
            {
                SubmitForSettlement = true
            }
        };

        var gateway = _brainTreeGateway.GetGateway();
        Result<Transaction> result = gateway.Transaction.Sale(request);

        if (result.Target.ProcessorResponseText == "Approved")
        {
            TempData["Success"] = "Transaction was successful Transaction ID " + result.Target.Id + ", Amount Charged: " + result.Target.Amount;
        }
        else
        {
            TempData["Error"] = "There was an error: " + result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}