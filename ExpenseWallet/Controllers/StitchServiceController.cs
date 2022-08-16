using Core.ExpenseWallet;
using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Core.ExpenseWallet.Utilities;
using Core.ExpenseWallet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ExpenseWallet.Controllers
{
    public class StitchServiceController : Controller
    {
        private readonly IUrlService _urlService;
        private readonly IWalletService _walletService;
        private readonly IFloatService _floatService;

        public StitchServiceController(IUrlService urlService, IWalletService walletService, IFloatService floatService)
        {
            _urlService = urlService;
            _walletService = walletService;
            _floatService = floatService;
        }

        public async Task<IActionResult> Index()
        {
            var url = await _urlService.BuildUrl();
            return Redirect(url);
        }
        [HttpGet]
        [Route("return")]
        public async Task<IActionResult> Wallet(AuthenticationResponseModel responseModel)
        {
            var vm = await _walletService.GetExpenseWalletView(responseModel.Code);
            return View(vm);
        }
        public IActionResult TopUp()
        {
            var vm = new TopUpViewModel() { Message = string.Empty };
            return View(vm);
        }
        [HttpPost]
        public IActionResult TopUp(TopUpViewModel topUpViewModel)
        {
            var isValidTopUp = IsValidTopUpModel(topUpViewModel);
            if (!isValidTopUp) { return View(topUpViewModel); }
            var amount = Convert.ToDouble(topUpViewModel.Amount, CultureInfo.InvariantCulture);
            var currentFloatBalance = _floatService.GetFloatBalance();
            var floatPayment = new FloatPayment()
            {
                Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero),
                Reference = topUpViewModel.Reference,
                Balance = Math.Round(currentFloatBalance + amount, 2, MidpointRounding.AwayFromZero),
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Currency = Default.DefaultCurrency
            };
            var addedSuccessfully = _floatService.AddFloatPayment(floatPayment);
            if (!addedSuccessfully) {
                topUpViewModel.Message = Default.DefaultTopUpErrorMessage;
                return View(topUpViewModel); 
            }
            return RedirectToAction("WalletTopUps");
        }
        public IActionResult WalletTopUps()
        {
            var vm = _walletService.GetTopUpWalletView();
            return View(vm);
        }
        public async Task<IActionResult> ExpenseBreakDown()
        {
            var vm = await _walletService.GetTransactionCategoryView();
            return View(vm);
        }
        private bool IsValidTopUpModel(TopUpViewModel topUpViewModel)
        {
            var isValidAmount = SecurityUtilities.IsValidAmount(topUpViewModel.Amount);
            var isValidReference = SecurityUtilities.IsValidReference(topUpViewModel.Reference);
            if (!isValidAmount) { ModelState.AddModelError("Amount", "Amount is required and cannot be less than 0.01"); }
            if (!isValidReference) { ModelState.AddModelError("Reference", "Reference is required, it must be less than 20 chars and cannot contain special characters"); }
            return isValidAmount || isValidReference;
        }
    }
}