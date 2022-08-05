using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using Moq;

namespace ExpenseWalletTests
{
    public class TokenBuilderTests
    {
        private Mock<ITokenBuilder> _tokenBuilder;
        public TokenBuilderTests()
        {
            _tokenBuilder = new Mock<ITokenBuilder>();
        }
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetTokenWithInvalidCode()
        {
            var code = string.Empty;
            _tokenBuilder
                .Setup(x => x.GetTokenWithCode(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticationToken());
            var tokenBuilder = _tokenBuilder.Object;
            var token = await tokenBuilder.GetTokenWithCode(code);
            Assert.IsNull(token.Access_Token);
        }
        [Test]
        public async Task GetTokenWithValidCode()
        {
            var code = "1BDOWcqNtnxlcbmEC1dnD3xOAi19vS4Njl8H8aR12wM";
            _tokenBuilder.Setup(x => x.GetTokenWithCode(It.IsAny<string>()))
                .ReturnsAsync(Faker.AuthenticationToken);
            var tokenBuilder = _tokenBuilder.Object;
            var token = await tokenBuilder.GetTokenWithCode(code);
            Assert.IsTrue(!string.IsNullOrEmpty(token.Access_Token));
            Assert.IsTrue(!string.IsNullOrEmpty(token.Scope));
            Assert.IsTrue(!string.IsNullOrEmpty(token.Id_Token));
            Assert.IsTrue(token.Expires_In > 0);

        }
    }
}