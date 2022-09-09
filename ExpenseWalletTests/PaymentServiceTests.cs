using Core.ExpenseWallet.Data;
using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Moq;

namespace ExpenseWalletTests
{
    public class PaymentServiceTests
    {

        private Mock<IStitchRequestHelper> _stitchRequestHelperMock { get; set; }
        private Mock<IHttpService> _httpServiceMock { get; set; }
        private Mock<IStitchSettings> _stitchSettingsMock { get; set; }
        private Mock<IInputOutputHelper> _inputOutputHelperMock { get; set; }
        private IPaymentService _paymentService { get; set; }
        public PaymentServiceTests()
        {

            _stitchRequestHelperMock = new Mock<IStitchRequestHelper>();
            _httpServiceMock = new Mock<IHttpService>();
            _stitchSettingsMock = new Mock<IStitchSettings>();
            _inputOutputHelperMock = new Mock<IInputOutputHelper>();
        }
        [SetUp]
        public void Setup()
        {
            _httpServiceMock.Setup(x => x.Get<AuthenticationToken>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new AuthenticationToken());
            _stitchSettingsMock.Setup(x => x.RedirectUrls).Returns(new List<string>() { "https://localhost:8000/return", "https://localhost:8080/return" });
            _stitchRequestHelperMock.Setup(x => x.GetStitchResponseWithVariablesAsync<StitchResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AuthenticationToken>())).ReturnsAsync(Faker.GetMockSuccessStitchResponse());
            _inputOutputHelperMock.Setup(x => x.Write(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            _paymentService = new PaymentService(_stitchRequestHelperMock.Object, _httpServiceMock.Object, _stitchSettingsMock.Object, _inputOutputHelperMock.Object);
        }

        [Test]
        public async Task GetPaymentInitiationTest_WithoutErrors()
        {
            var stitchResponse = await _paymentService.GetPaymentInitiation(Faker.GetValidFloatPayment());
            Assert.That(stitchResponse != null);
            Assert.That(!stitchResponse.HasErrors);
            Assert.That(stitchResponse.data != null);
            Assert.That(stitchResponse.data.userInitiatePayment != null);
            Assert.That(stitchResponse.data.userInitiatePayment.paymentInitiation != null);
            Assert.That(!string.IsNullOrEmpty(stitchResponse.data.userInitiatePayment.paymentInitiation.amount.quantity));
            var amount = Convert.ToDouble(stitchResponse.data.userInitiatePayment.paymentInitiation.amount.quantity);
            Assert.That(amount > 0);
        }
        [Test]
        public async Task GetPaymentInitiationTest_WithErrors()
        {
            _stitchRequestHelperMock.Setup(x => x.GetStitchResponseWithVariablesAsync<StitchResponse>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AuthenticationToken>())).ReturnsAsync(new StitchResponse()
            {
                Errors = Faker.GetMockErrors()
            });
            var stitchResponse = await _paymentService.GetPaymentInitiation(Faker.GetValidFloatPayment());
            Assert.That(stitchResponse != null);
            Assert.That(stitchResponse.HasErrors);
            Assert.That(stitchResponse.Errors.Any(x => !string.IsNullOrEmpty(x.Extensions.userInteractionUrl)));
        }
    }
}
