using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseWalletTests
{
    public class UrlServiceTests
    {
        private Mock<IUrlService> _urlService;
        public UrlServiceTests()
        {
            _urlService = new Mock<IUrlService>();
        }
        [Test]
        public async Task BuildCorrectAuthorizationUrl()
        {
            _urlService.Setup(x => x.BuildUrl(It.IsAny<RedirectUrlModel>())).ReturnsAsync(Faker.AuthorizationUrl());
            var urlService = _urlService.Object;
            var authUrl = await urlService.BuildUrl(new RedirectUrlModel());
            Assert.True(!string.IsNullOrEmpty(authUrl));
        }
    }
}
