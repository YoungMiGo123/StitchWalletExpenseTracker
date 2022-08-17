using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Core.ExpenseWallet.ViewModels;
using Moq;

namespace ExpenseWalletTests
{
    public class WalletServiceTests
    {
        private Mock<IWalletService> _walletService;
        private Mock<IStitchSettings> _stitchSettings;
        private Mock<IHttpService> _httpService;
        private Mock<IInputOutputHelper> _ioHelper;
        private Mock<IFloatService> _floatService;
        private Mock<ITokenBuilder> _tokenBuilder;
        private Mock<IStitchRequestHelper> _stitchRequestHelper;

        public WalletServiceTests()
        {
            _walletService = new Mock<IWalletService>();
            _stitchSettings = new Mock<IStitchSettings>();
            _httpService = new Mock<IHttpService>();
            _ioHelper = new Mock<IInputOutputHelper>();
            _floatService = new Mock<IFloatService>();
            _tokenBuilder = new Mock<ITokenBuilder>();
            _stitchRequestHelper = new Mock<IStitchRequestHelper>();
        }
        [Test]
        public async Task GetExpenseWalletViewWithInvalidCode()
        {
            var code = string.Empty;
            _ioHelper.Setup(x => x.Read(It.IsAny<string>())).Returns(string.Empty);
            var walletService = new WalletService(_tokenBuilder.Object, _httpService.Object, _stitchSettings.Object, _ioHelper.Object, _floatService.Object, _stitchRequestHelper.Object);
            try
            {
                var result = await walletService.GetExpenseWalletView(code);
            }catch(Exception ex)
            {
                Assert.That(ex.Message.StartsWith("No code"), Is.True);
                return;
            }

            Assert.Fail();
        }
        [Test]
        public async Task GetExpenseWalletViewWithValidCode()
        {
            var code = Faker.GetCode();
            _walletService.Setup(x => x.GetExpenseWalletView(It.IsAny<string>()))
                .ReturnsAsync(new ExpenseWalletView());
            var walletService = _walletService.Object;  
            var result = await walletService.GetExpenseWalletView(code);
            Assert.True(result != null);
        }
        [Test]
        public async Task GetTransactionCategoryView()
        {
            _walletService.Setup(x => x.GetTransactionCategoryView()).ReturnsAsync(new TransactionCategoryView());
            var walletService = _walletService.Object;
            var result = await walletService.GetTransactionCategoryView();
            Assert.True(result != null);
        }
    }
}
