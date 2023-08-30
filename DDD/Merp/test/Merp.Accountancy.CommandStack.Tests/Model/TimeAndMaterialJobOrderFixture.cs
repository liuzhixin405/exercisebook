using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Merp.Infrastructure;
using Merp.Accountancy.CommandStack.Model;
using Merp.Accountancy.CommandStack.Services;
using Merp.Accountancy.CommandStack.Events;
using System.Collections.Generic;

namespace Merp.Accountancy.CommandStack.Tests.Model
{
    [TestClass]
    public class TimeAndMaterialJobOrderFixture
    {
        [TestClass]
        public class CalculateBalance_Method
        {
            [TestMethod]
            public void CalculateBalance_With_IncomingInvoicesOnly()
            {
                var generator = new Mock<IJobOrderNumberGenerator>();
                generator
                    .Setup(o => o.Generate())
                    .Returns("101/1989");

                var jobOrderId = Guid.NewGuid();
                var incomingInvoiceId = Guid.NewGuid();
                var eventStore = new Mock<IEventStore>();
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<IncomingInvoiceAssociatedToJobOrderEvent, bool>>()))
                    .Returns(new IncomingInvoiceAssociatedToJobOrderEvent[] { new IncomingInvoiceAssociatedToJobOrderEvent(incomingInvoiceId, jobOrderId) });
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<IncomingInvoiceRegisteredEvent, bool>>()))
                    .Returns(new IncomingInvoiceRegisteredEvent[] { new IncomingInvoiceRegisteredEvent(incomingInvoiceId, "42", DateTime.Now, 100, 22, 122, "fake", "fake", "", Guid.NewGuid(), "", "", "", "", "", "", "") });

                var sut = TimeAndMaterialJobOrder.Factory.CreateNewInstance(generator.Object,
                                                                        Guid.NewGuid(),
                                                                        "Managed Designs",
                                                                        Guid.NewGuid(),
                                                                        "Giuseppe Verdi",
                                                                        1,
                                                                        DateTime.Now,
                                                                        DateTime.Now.AddMonths(1),
                                                                        "Fake",
                                                                        "42",
                                                                        "Fake");
                decimal balance = sut.CalculateBalance(eventStore.Object);
                decimal expected = -100;
                Assert.AreEqual<decimal>(expected, balance);
            }

            [TestMethod]
            public void CalculateBalance_With_OutgoingInvoicesOnly()
            {
                var generator = new Mock<IJobOrderNumberGenerator>();
                generator
                    .Setup(o => o.Generate())
                    .Returns("101/1989");

                var jobOrderId = Guid.NewGuid();
                var outgoingInvoiceId = Guid.NewGuid();
                var eventStore = new Mock<IEventStore>();
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<OutgoingInvoiceAssociatedToJobOrderEvent, bool>>()))
                    .Returns(new OutgoingInvoiceAssociatedToJobOrderEvent[] { new OutgoingInvoiceAssociatedToJobOrderEvent(outgoingInvoiceId, jobOrderId) });
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<OutgoingInvoiceIssuedEvent, bool>>()))
                    .Returns(new OutgoingInvoiceIssuedEvent[] { new OutgoingInvoiceIssuedEvent(outgoingInvoiceId, "42", DateTime.Now, 100, 22, 122, "fake", "fake", "", Guid.NewGuid(), "", "", "", "", "", "", "") });

                var sut = TimeAndMaterialJobOrder.Factory.CreateNewInstance(generator.Object,
                                                                        Guid.NewGuid(),
                                                                        "Managed Designs",
                                                                        Guid.NewGuid(),
                                                                        "Giuseppe Verdi",
                                                                        1,
                                                                        DateTime.Now,
                                                                        DateTime.Now.AddMonths(1),
                                                                        "Fake",
                                                                        "42",
                                                                        "Fake");
                decimal balance = sut.CalculateBalance(eventStore.Object);
                decimal expected = 100;
                Assert.AreEqual<decimal>(expected, balance);
            }

            [TestMethod]
            public void CalculateBalance_having_both_Incoming_and_Outgoing_Invoices()
            {
                var generator = new Mock<IJobOrderNumberGenerator>();
                generator
                    .Setup(o => o.Generate())
                    .Returns("101/1989");

                var eventStore = new Mock<IEventStore>();

                var jobOrderId = Guid.NewGuid();
                var outgoingInvoiceId = Guid.NewGuid();
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<OutgoingInvoiceAssociatedToJobOrderEvent, bool>>()))
                    .Returns(new OutgoingInvoiceAssociatedToJobOrderEvent[] { new OutgoingInvoiceAssociatedToJobOrderEvent(outgoingInvoiceId, jobOrderId) });
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<OutgoingInvoiceIssuedEvent, bool>>()))
                    .Returns(new OutgoingInvoiceIssuedEvent[] { new OutgoingInvoiceIssuedEvent(outgoingInvoiceId, "42", DateTime.Now, 200, 44, 244, "fake", "fake", "", Guid.NewGuid(), "", "", "", "", "", "", "") });

                var incomingInvoiceId = Guid.NewGuid();
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<IncomingInvoiceAssociatedToJobOrderEvent, bool>>()))
                    .Returns(new IncomingInvoiceAssociatedToJobOrderEvent[] { new IncomingInvoiceAssociatedToJobOrderEvent(incomingInvoiceId, jobOrderId) });
                eventStore
                    .Setup(o => o.Find(It.IsAny<Func<IncomingInvoiceRegisteredEvent, bool>>()))
                    .Returns(new IncomingInvoiceRegisteredEvent[] { new IncomingInvoiceRegisteredEvent(incomingInvoiceId, "42", DateTime.Now, 100, 22, 122, "fake", "fake", "", Guid.NewGuid(), "", "", "", "", "", "", "") });

                var sut = TimeAndMaterialJobOrder.Factory.CreateNewInstance(generator.Object,
                                                                        Guid.NewGuid(),
                                                                        "Managed Designs",
                                                                        Guid.NewGuid(),
                                                                        "Giuseppe Verdi",
                                                                        1,
                                                                        DateTime.Now,
                                                                        DateTime.Now.AddMonths(1),
                                                                        "Fake",
                                                                        "42",
                                                                        "Fake");
                
                decimal balance = sut.CalculateBalance(eventStore.Object);
                decimal expected = 100;
                Assert.AreEqual<decimal>(expected, balance);
            }
        }

        [TestClass]
        public class MarkAsCompleted_Method
        {
            //[TestMethod]
            //public void Should_Throw_InvalidOperationException_On()
            //{
            //}
        }
    }
}
