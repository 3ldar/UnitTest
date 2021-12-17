using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassLibrary1;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace TestProject1
{
    [TestClass]
    public class WhenUsingBankService
    {
        IBankService bankserice;

        Mock<IBankRepository> moqRepository;
        Mock<IReportingService> moqReportingService;
        List<int> overDrawnAccounts = new();
        [TestInitialize]
        public void Setup()
        {
            moqRepository = new Mock<IBankRepository>();
            moqReportingService = new Mock<IReportingService>();


            var dummyAccount = new Account { ID = 1, Balance = 100 };

            moqRepository.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(dummyAccount);

            moqReportingService.Setup(x => x.AccountIsOverdrawn(It.IsAny<int>()))
                .Callback<int>(r =>
                {
                    overDrawnAccounts.Add(r);
                });

            moqReportingService.Setup(x => x.GetOverdrawnAccounts()).Returns(() => overDrawnAccounts.ToArray());

            bankserice = new BankService(moqRepository.Object, moqReportingService.Object);
        }

        [TestMethod]
        public void ItShouldReturnAValidAccount()
        {
            var account = bankserice.GetAccount("3");
            Assert.IsNotNull(account);
            Assert.AreEqual(100, account.Balance);
            Assert.AreEqual(1, account.ID);
        }

        [TestMethod()]
        public void ItShouldCallRepositoryGetMethod()
        {
            var account = bankserice.GetAccount("5");
            moqRepository.Verify(e => e.GetAccount("5"), Times.Once);
        }

        [TestMethod]
        public void ItShouldUpdateAccountBalance()
        {
            var myAccount = new Account { ID = 1, Balance = 150 };
            bankserice.UpdateAccountBalance(myAccount, 50);

            Assert.AreEqual(myAccount.Balance, 200);

            moqRepository.Verify(e => e.SetBalance(1, 200));
            moqRepository.Verify(r => r.AddTransaction(1, 50, 200));
        }

        [TestMethod]
        public void ItShouldReportWhenBalanceIsLessThenZero()
        {
            var myAccount = new Account { ID = 1, Balance = 150 };
            bankserice.UpdateAccountBalance(myAccount, -250);
            moqReportingService.Verify(r => r.AccountIsOverdrawn(1), Times.Once);

        }

        [TestMethod]
        public void ItShouldListNegativeBalancedAccounts()
        {
            var myAccount = new Account { ID = 1, Balance = 150 };
            bankserice.UpdateAccountBalance(myAccount, -250);
            var yourAccount = new Account { ID = 2, Balance = 30 };

            bankserice.UpdateAccountBalance(yourAccount, -50);

            moqReportingService.Verify(r => r.AccountIsOverdrawn(It.IsAny<int>()), Times.Exactly(2));

            var negativeBalanceIds = bankserice.GetNegativeBalancedAccountIds();
            moqReportingService.Verify(r => r.GetOverdrawnAccounts(), Times.Exactly(1));
            int[] expected = { 1, 2 };

            Assert.IsTrue(expected.SequenceEqual(negativeBalanceIds));


        }
    }
}
