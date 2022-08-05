using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseWallet.Controllers
{
    public class StitchServiceController : Controller
    {
        private readonly ILogger<StitchServiceController> _logger;
        private readonly IUrlService _urlService;
        private readonly IWalletService _walletService;

        public StitchServiceController(ILogger<StitchServiceController> logger, IUrlService urlService, IWalletService walletService)
        {
            _logger = logger;
            _urlService = urlService;
            _walletService = walletService;
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
        public async Task<IActionResult> ExpenseBreakDown()
        {
            var vm = await _walletService.GetTransactionCategoryView();
            return View(vm);
        }

    }
}