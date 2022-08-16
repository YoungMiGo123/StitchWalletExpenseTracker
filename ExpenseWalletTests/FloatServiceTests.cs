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
            _floatService = new FloatService(_inputOutputHelperMock.Object);
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
        public void AddValidFloatPaymentTest()
        {
            var result = _floatService.AddFloatPayment(Faker.GetValidFloatPayment());
            Assert.IsTrue(result);
        }
        [Test]
        public void AddInvalidFloatPaymentTest()
        {
            var result = _floatService.AddFloatPayment(Faker.GetInvalidFloatPayment());
            Assert.IsFalse(result);
        }
    }
}
