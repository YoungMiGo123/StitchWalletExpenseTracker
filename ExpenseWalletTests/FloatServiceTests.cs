using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Core.ExpenseWallet.Utilities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseWalletTests
{
    public class FloatServiceTests
    {
        private Mock<IInputOutputHelper> _inputOutputHelperMock;

        private IFloatService _floatService;
        public FloatServiceTests()
        {
            _inputOutputHelperMock = new Mock<IInputOutputHelper>();
            var tokenBuilderMock = new Mock<ITokenBuilder>();
            var stitckHelper = new Mock<IStitchRequestHelper>();
            var urlService = new Mock<IUrlService>();
            var stitchSettings = new Mock<IStitchSettings>();
            var httpService = new Mock<IHttpService>();
            _floatService = new FloatService(_inputOutputHelperMock.Object, stitchSettings.Object, tokenBuilderMock.Object, stitckHelper.Object, urlService.Object, httpService.Object);
        }

        [SetUp]
        public void Setup()
        {
            _inputOutputHelperMock.Setup(x => x.Read(It.IsAny<string>())).Returns(string.Empty);
            _inputOutputHelperMock.Setup(x => x.Write(It.IsAny<string>(), It.IsAny<string>())).Verifiable(); 
        }
        [Test]
        public void GetFloatTest()
        {
            var resultFloat = _floatService.GetFloat();
            Assert.IsNotNull(resultFloat);
        }
        [Test] 
        public void GetFloatBalanceTest()
        {
            _inputOutputHelperMock.Setup(x => x.Read(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(Faker.GetValidFloat()));
            var floatBalance = _floatService.GetFloatBalance();
            var lastPayment = Faker.GetValidFloat().FloatPayments.Last();
            Assert.That(floatBalance > 0, Is.True);
            Assert.That(floatBalance == lastPayment.Balance, Is.True);
        }
        [Test]
        public async Task AddValidFloatPaymentTest()
        {
            var result = await _floatService.AddFloatPayment(Faker.GetValidFloatPayment());
            Assert.That(result != null, Is.True);
        }
        [Test]
        public async Task AddInvalidFloatPaymentTest()
        {
            var result = await _floatService.AddFloatPayment(Faker.GetInvalidFloatPayment());
            Assert.That(result != null, Is.True);
        }
    }
}
