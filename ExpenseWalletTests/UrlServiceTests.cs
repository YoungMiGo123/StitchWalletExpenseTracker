using Core.ExpenseWallet.Interfaces;
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
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public async Task BuildCorrectAuthorizationUrl()
        {
            _urlService.Setup(x => x.BuildUrl()).ReturnsAsync(Faker.AuthorizationUrl());
            var urlService = _urlService.Object;
            var authUrl = await urlService.BuildUrl();
            Assert.True(!string.IsNullOrEmpty(authUrl));
        }
    }
}
