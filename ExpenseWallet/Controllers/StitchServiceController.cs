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
        private readonly ILogger<StitchServiceController> _logger;
        private readonly IUrlService _urlService;
        private readonly IWalletService _walletService;
        private readonly IFloatService _floatService;
        private readonly IStitchSettings _settings;
        private readonly IPaymentService _paymentService;

        public StitchServiceController(ILogger<StitchServiceController> logger, IUrlService urlService, IWalletService walletService, IFloatService floatService, IStitchSettings settings, IPaymentService paymentService)
        {
            _logger = logger;
            _urlService = urlService;
            _walletService = walletService;
            _floatService = floatService;
            _settings = settings;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var link = await _urlService.BuildUrl(new RedirectUrlModel() { AuthorizationUrl = _settings.AuthorizeUrl });
            return Redirect(link);
        }
        [HttpGet]
        [Route("return")]
        public async Task<IActionResult> Wallet(AuthenticationResponseModel responseModel)
        {
            ExpenseWalletView vm = Default.GetDefaultExpenseWalletView();
            try
            {
                vm = await _walletService.GetExpenseWalletView(responseModel.Code);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception happened while getting the wallet {ex}");
            }
            return View(vm);
        }
        public IActionResult TopUp()
        {
            var vm = new TopUpViewModel() { Message = string.Empty };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> TopUp(TopUpViewModel topUpViewModel)
        {
            try
            {
                if (!IsValidTopUpModel(topUpViewModel)) { return View(topUpViewModel); }
                var result = await AddFloatTopUp(topUpViewModel);
                if (!string.IsNullOrEmpty(result))
                {
                    return Redirect(result);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception happened while topping up the account {ex}");
            }
            return RedirectToAction("WalletTopUps");
        }
        public IActionResult WalletTopUps()
        {
            TopUpWalletView vm = Default.GetDefaultTopUpWalletView();
            try {
                vm = _walletService.GetTopUpWalletView();
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception happened while getting the wallet top ups {ex}");
            }
            
            return View(vm);
        }
        public async Task<IActionResult> ExpenseBreakDown()
        {
            TransactionCategoryView vm = Default.GetDefaultTransactionCategoryView;
            try
            {
                vm = await _walletService.GetTransactionCategoryView();
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception happened while getting the expense breakdown {ex}");
            }
            
            return View(vm);
        }
        [Route("HandleMfaTopUp")]
        [HttpGet]
        public IActionResult HandleMfaTopUp(MfaTopUpResponseModel mfaTopUpResponseModel)
        {
            try
            {
                _floatService.AddFloatPayment(mfaTopUpResponseModel);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception happened while mfa top uo {ex}");
            }
           
            return RedirectToAction("WalletTopUps");
        }
        private bool IsValidTopUpModel(TopUpViewModel topUpViewModel)
        {
            var isValidTopUp = false;
            try
            {
                var isValidAmount = SecurityUtilities.IsValidAmount(topUpViewModel.Amount);
                var isValidReference = SecurityUtilities.IsValidReference(topUpViewModel.Reference);
                if (!isValidAmount) { ModelState.AddModelError("Amount", "Amount is required and cannot be less than 0.01"); }
                if (!isValidReference) { ModelState.AddModelError("Reference", "Reference is required, it must be less than 20 chars and cannot contain special characters"); }
                isValidTopUp = isValidAmount || isValidReference;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error happened while validating the top up view model {ex}");
            }
            return isValidTopUp;
        }

        private async Task<string> AddFloatTopUp(TopUpViewModel topUpViewModel)
        {
            
            var currentFloatBalance = _floatService.GetFloatBalance();
            var amount = Convert.ToDouble(topUpViewModel.Amount, CultureInfo.InvariantCulture);
            var floatPayment = new FloatPayment()
            {
                Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero),
                Reference = topUpViewModel.Reference,
                Balance = Math.Round(currentFloatBalance + amount, 2, MidpointRounding.AwayFromZero),
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Currency = Default.DefaultCurrency
            };
            var stitchResponse = await _paymentService.GetPaymentInitiation(floatPayment);
            if (stitchResponse.HasErrors)
            {
                var mfaAuthUrl = _paymentService.GetMultifactorPaymentIniationUrl(stitchResponse);
                return mfaAuthUrl;
            }
            await _floatService.AddFloatPayment(floatPayment,stitchResponse);
            return string.Empty;
        }
    }
}