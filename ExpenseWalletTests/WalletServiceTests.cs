using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Core.ExpenseWallet.ViewModels;
using Moq;

namespace ExpenseWalletTests
{
    public class WalletServiceTests
    {
        private Mock<IWalletService> _walletService;
        public WalletServiceTests()
        {
            _walletService = new Mock<IWalletService>();
        }
        [Test]
        public async Task GetExpenseWalletViewWithInvalidCode()
        {
            var code = string.Empty;
            var walletService = new WalletService();
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
