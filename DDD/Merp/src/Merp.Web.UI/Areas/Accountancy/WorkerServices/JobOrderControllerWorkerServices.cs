using Merp.Accountancy.CommandStack.Commands;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Merp.Web.UI.Areas.Accountancy.Models.JobOrder;
using Merp.Accountancy.QueryStack;
using Merp.Accountancy.QueryStack.Model;

namespace Merp.Web.UI.Areas.Accountancy.WorkerServices
{
    public class JobOrderControllerWorkerServices
    {
        public IBus Bus { get; private set; }
        public IDatabase Database { get; private set; }
        public IRepository Repository { get; private set; }
        public IEventStore EventStore { get; private set; }
        public JobOrderControllerWorkerServices(IBus bus, IDatabase database, IRepository repository, IEventStore eventStore)
        {
            if(bus==null)
            {
                throw new ArgumentNullException("bus");
            }
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (eventStore == null)
            {
                throw new ArgumentNullException("eventStore");
            }

            this.Bus = bus;
            this.Database = database;
            this.Repository = repository;
            this.EventStore = eventStore;
        }

        public IEnumerable<IndexViewModel.JobOrder> GetList()
        {
            return (from jo in Database.JobOrders
                   select new IndexViewModel.JobOrder
                   {
                        CustomerId = jo.CustomerId,
                        CustomerName = jo.CustomerName,
                        IsCompleted = jo.IsCompleted,
                        Name = jo.Name,
                        Number = jo.Number,
                        Id = jo.Id,
                        OriginalId = jo.OriginalId
                   }).ToArray();
        }

        public string GetDetailViewModel(Guid jobOrderId)
        {
            if (Database.JobOrders.OfType<FixedPriceJobOrder>().Where(p => p.OriginalId == jobOrderId).Count() == 1)
            {
                return "FixedPrice";
            }
            else if (Database.JobOrders.OfType<TimeAndMaterialJobOrder>().Where(p => p.OriginalId == jobOrderId).Count() == 1)
            {
                return "TimeAndMaterial";
            }
            else
            {
                return "Unknown";
            }
        }

        #region Fixed Price Job Orders
        public CreateFixedPriceViewModel GetCreateFixedPriceViewModel()
        {
            var model = new CreateFixedPriceViewModel();
            model.DateOfStart = DateTime.Now;
            model.DueDate = DateTime.Now;
            return model;
        }

        public ExtendFixedPriceViewModel GetExtendFixedPriceViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.FixedPriceJobOrder>(jobOrderId);
            var model = new ExtendFixedPriceViewModel();
            model.NewDueDate = jobOrder.DueDate;
            model.Price = jobOrder.Price;
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderName = jobOrder.Name;
            model.CustomerName = jobOrder.Customer.Name;
            return model;
        }

        public FixedPriceJobOrderDetailViewModel GetFixedPriceJobOrderDetailViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.FixedPriceJobOrder>(jobOrderId);

            var model = new FixedPriceJobOrderDetailViewModel();
            model.CustomerName = jobOrder.Customer.Name;
            model.DateOfStart = jobOrder.DateOfStart;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderName = jobOrder.Name;
            model.Notes = string.Empty;
            model.Price = jobOrder.Price;
            model.IsCompleted = jobOrder.IsCompleted;
            return model;
        }

        public MarkFixedPriceJobOrderAsCompletedViewModel GetMarkFixedPriceJobOrderAsCompletedViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.FixedPriceJobOrder>(jobOrderId);

            var model = new MarkFixedPriceJobOrderAsCompletedViewModel();
            model.DateOfCompletion = DateTime.Now;
            model.CustomerName = jobOrder.Customer.Name;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderName = jobOrder.Name;
            return model;
        }

        public void CreateFixedPriceJobOrder(CreateFixedPriceViewModel model)
        {
            var command = new RegisterFixedPriceJobOrderCommand( 
                    model.Customer.OriginalId,
                    model.Customer.Name,
                    model.Manager.OriginalId,
                    model.Manager.Name,
                    model.Price,
                    model.DateOfStart,
                    model.DueDate,
                    model.Name, 
                    model.PurchaseOrderNumber,
                    model.Description
                );
            Bus.Send(command);
        }

        public void ExtendFixedPriceJobOrder(ExtendFixedPriceViewModel model)
        {
            var command = new ExtendFixedPriceJobOrderCommand(model.JobOrderId, model.NewDueDate, model.Price);
            Bus.Send(command);
        }

        public void MarkFixedPriceJobOrderAsCompleted(MarkFixedPriceJobOrderAsCompletedViewModel model)
        {
            var command = new MarkFixedPriceJobOrderAsCompletedCommand(model.JobOrderId, model.DateOfCompletion);
            Bus.Send(command);
        }
        #endregion

        #region Time And Material Job Orders
        public CreateTimeAndMaterialViewModel GetCreateTimeAndMaterialViewModel()
        {
            var model = new CreateTimeAndMaterialViewModel();
            model.DateOfStart = DateTime.Now;
            return model;
        }

        public ExtendTimeAndMaterialViewModel GetExtendTimeAndMaterialViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.TimeAndMaterialJobOrder>(jobOrderId);
            var model = new ExtendTimeAndMaterialViewModel();
            if(jobOrder.Value.HasValue)
            {
                model.Value = jobOrder.Value.Value;
            }
            if (jobOrder.DateOfExpiration.HasValue)
            {
                model.NewDateOfExpiration = jobOrder.DateOfExpiration;
            }
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderName = jobOrder.Name;
            model.CustomerName = jobOrder.Customer.Name;
            return model;
        }
        public MarkTimeAndMaterialJobOrderAsCompletedViewModel GetMarkTimeAndMaterialJobOrderAsCompletedViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.TimeAndMaterialJobOrder>(jobOrderId);

            var model = new MarkTimeAndMaterialJobOrderAsCompletedViewModel();
            model.DateOfCompletion = DateTime.Now;
            model.CustomerName = jobOrder.Customer.Name;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderName = jobOrder.Name;
            return model;
        }
        public void CreateTimeAndMaterialJobOrder(CreateTimeAndMaterialViewModel model)
        {
            var command = new RegisterTimeAndMaterialJobOrderCommand(
                    model.Customer.OriginalId,
                    model.Customer.Name,
                    model.Manager.OriginalId,
                    model.Manager.Name,
                    model.Value,
                    model.DateOfStart,
                    model.DateOfExpiration,
                    model.Name,
                    model.PurchaseOrderNumber,
                    model.Description
                );
            Bus.Send(command);
        }

        public void ExtendTimeAndMaterialJobOrder(ExtendTimeAndMaterialViewModel model)
        {
            var command = new ExtendTimeAndMaterialJobOrderCommand(model.JobOrderId, model.NewDateOfExpiration, model.Value);
            Bus.Send(command);
        }

        public TimeAndMaterialJobOrderDetailViewModel GetTimeAndMaterialJobOrderDetailViewModel(Guid jobOrderId)
        {
            var jobOrder = Repository.GetById<Merp.Accountancy.CommandStack.Model.TimeAndMaterialJobOrder>(jobOrderId);

            var model = new TimeAndMaterialJobOrderDetailViewModel();
            model.CustomerName = jobOrder.Customer.Name;
            model.DateOfStart = jobOrder.DateOfStart;
            model.JobOrderId = jobOrder.Id;
            model.JobOrderNumber = jobOrder.Number;
            model.JobOrderName = jobOrder.Name;
            model.Notes = string.Empty;
            model.Value = jobOrder.Value;
            model.IsCompleted = jobOrder.IsCompleted;
            return model;
        }

        public void MarkTimeAndMaterialJobOrderAsCompleted(MarkTimeAndMaterialJobOrderAsCompletedViewModel model)
        {
            var command = new MarkTimeAndMaterialJobOrderAsCompletedCommand(model.JobOrderId, model.DateOfCompletion);
            Bus.Send(command);
        }
        #endregion

    }
}