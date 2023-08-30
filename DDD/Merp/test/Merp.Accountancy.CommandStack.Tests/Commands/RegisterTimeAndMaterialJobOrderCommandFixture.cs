using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Merp.Accountancy.CommandStack.Commands;

namespace Merp.Accountancy.CommandStack.Tests.Commands
{
    [TestClass]
    public class RegisterTimeAndMaterialJobOrderCommandFixture
    {
        [TestMethod]
        public void Ctor_should_set_properties_according_to_parameters()
        {
            DateTime dateOfStart = new DateTime(1990, 11, 11);
            DateTime? dateOfExpiration = new DateTime(1990, 11, 12);
            decimal? value = 143;
            string jobOrderName = "fake";
            Guid customerId = Guid.NewGuid();
            string customerName = "ACME";
            Guid managerId = Guid.NewGuid();
            string managerName = "John Doe";
            string purchaseOrderNumber = "42";
            string description = "xyz";
            var sut = new RegisterTimeAndMaterialJobOrderCommand(
                customerId,
                customerName,
                managerId,
                managerName,
                value,
                dateOfStart,
                dateOfExpiration,
                jobOrderName,
                purchaseOrderNumber,
                description
                );
            Assert.AreEqual<DateTime>(dateOfStart, sut.DateOfStart);
            Assert.AreEqual<DateTime?>(dateOfExpiration, sut.DateOfExpiration);
            Assert.AreEqual<decimal?>(value, sut.Value);
            Assert.AreEqual<Guid>(customerId, sut.CustomerId);
            Assert.AreEqual<string>(jobOrderName, sut.JobOrderName);
            Assert.AreEqual<string>(customerName, sut.CustomerName);
            Assert.AreEqual<Guid>(managerId, sut.ManagerId);
            Assert.AreEqual<string>(managerName, sut.ManagerName);
            Assert.AreEqual<string>(purchaseOrderNumber, sut.PurchaseOrderNumber);
            Assert.AreEqual<string>(description, sut.Description);
        }
    }
}
