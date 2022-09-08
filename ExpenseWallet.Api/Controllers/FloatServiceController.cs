using Core.ExpenseWallet;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseWallet.Api.Controllers
{
    public class FloatServiceController : Controller
    {
        private readonly ILogger<FloatServiceController> _logger;
        private readonly IFloatService _floatService;
        private readonly IUrlService _urlService;
        private readonly IStitchSettings _settings;
        private readonly IInputOutputHelper _inputOutputHelper;
        private readonly ITokenBuilder _tokenBuilder;

        public FloatServiceController(ILogger<FloatServiceController> logger, IFloatService floatService, IUrlService urlService, IStitchSettings settings, IInputOutputHelper inputOutputHelper, ITokenBuilder tokenBuilder)
        {
            _logger = logger;
            _floatService = floatService;
            _urlService = urlService;
            _settings = settings;
            _inputOutputHelper = inputOutputHelper;
            _tokenBuilder = tokenBuilder;
        }

        [HttpGet("LinkAccount")]
        public async Task<IActionResult> LinkAccount()
        {
            var paymentAuthorizationUrl = await _floatService.GetPaymentAuthorizationUrl();
            var redirectUrl = _settings.RedirectUrls.Last();
            var link = await _urlService.BuildUrl(new RedirectUrlModel { AuthorizationUrl = paymentAuthorizationUrl, RedirectUrl = redirectUrl, UseExistingAuthModel = true });
            return Redirect(link);
        }
        [HttpGet]
        [Route("return")]
        public async Task<IActionResult> ProcessCallback(AuthenticationResponseModel responseModel)
        {
            try
            {
                var userToken = await _tokenBuilder.GetTokenWithCode(responseModel.Code, useExistingAuth: true);
                _inputOutputHelper.Write(SecurityUtilities.UserTokenJsonPath, JsonConvert.SerializeObject(userToken));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while getting the token {ex}");
            }
            var redirectUrl = _settings.RedirectUrls.First().Replace("return", "StitchService/TopUp");
            return Redirect(redirectUrl);
        }
        [HttpGet]
        [Route("GetAuthenticationToken")]
        public IActionResult GetAuthenticationToken()
        {
            var result = new AuthenticationToken();
            try
            {
                var authenticationResponseModel = _inputOutputHelper.Read(SecurityUtilities.UserTokenJsonPath);
                result = JsonConvert.DeserializeObject<AuthenticationToken>(authenticationResponseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while getting the token {ex}"); ;
            }
            return Ok(result);
        }
        [HttpPost]
        [Route("UpdateAuthModel")]
        public IActionResult UpdateAuthModel([FromBody] AuthModel authModel)
        {
            if (authModel != null)
            {
                _inputOutputHelper.Write(SecurityUtilities.StitchSettingsJsonPath, JsonConvert.SerializeObject(authModel));
            }
            return Ok(true);
        }
    }
}
