using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Merp.Accountancy.CommandStack.Commands;

namespace Merp.Accountancy.CommandStack.Tests.Commands
{
    [TestClass]
    public class IssueInvoiceCommandFixture
    {
        [TestMethod]
        public void Ctor_should_set_properties_according_to_parameters()
        {
            DateTime invoiceDate = new DateTime(1990, 11, 11);
            decimal amount = 101;
            decimal taxes = 42;
            decimal totalPrice = 143;
            string description = "fake";
            string paymentTerms = "none";
            string purchaseOrderNumber = "42";
            Guid customerId = Guid.NewGuid();
            string customerName = "Managed Designs S.r.l.";
            string streetName = "Via Torino 51";
            string city = "Milan";
            string postalCode = "20123";
            string country ="Italy";
            string vatIndex = "04358780965";
            string nationalIdentificationNumber = "04358780965";
            var sut = new IssueInvoiceCommand(
                invoiceDate,
                amount,
                taxes,
                totalPrice,
                description,
                paymentTerms,
                purchaseOrderNumber,
                customerId,
                customerName,
                streetName,
                city,
                postalCode,
                country,
                vatIndex,
                nationalIdentificationNumber);
            Assert.AreEqual<DateTime>(invoiceDate, sut.InvoiceDate);
            Assert.AreEqual<decimal>(amount, sut.Amount);
            Assert.AreEqual<decimal>(taxes, sut.Taxes);
            Assert.AreEqual<decimal>(totalPrice, sut.TotalPrice);
            Assert.AreEqual<string>(description, sut.Description);
            Assert.AreEqual<string>(paymentTerms, sut.PaymentTerms);
            Assert.AreEqual<string>(purchaseOrderNumber, sut.PurchaseOrderNumber);
            Assert.AreEqual<Guid>(customerId, sut.Customer.Id);
            Assert.AreEqual<string>(customerName, sut.Customer.Name);
            Assert.AreEqual<string>(streetName, sut.Customer.StreetName);
            Assert.AreEqual<string>(city, sut.Customer.City);
            Assert.AreEqual<string>(postalCode, sut.Customer.PostalCode);
            Assert.AreEqual<string>(country, sut.Customer.Country);
            Assert.AreEqual<string>(vatIndex, sut.Customer.VatIndex);
            Assert.AreEqual<string>(nationalIdentificationNumber, sut.Customer.NationalIdentificationNumber);
        }
    }
}
