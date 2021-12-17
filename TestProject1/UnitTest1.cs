using System;

using ClassLibrary1;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private Calculator calculator;

        [TestInitialize]
        public void Setup()
        {
            calculator = new Calculator();
        }

        [TestMethod]
        public void TestAdd()
        {
            var result = calculator.Add(3, 5);

            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestDivide()
        {
            var result = calculator.Divide(4, 2);

            Assert.AreEqual(2, result);          
            
        }

        [TestMethod]
        //[ExpectedException()]
        public void TestDivideException()
        {
            Assert.ThrowsException<DivideByZeroException>(() => calculator.Divide(4, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivideExceptionAttribute()
        {
            var result = calculator.Divide(4, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDivideArgumentExceptionAttribute()
        {
            var result = calculator.Divide(4, 0);
        }
    }
}