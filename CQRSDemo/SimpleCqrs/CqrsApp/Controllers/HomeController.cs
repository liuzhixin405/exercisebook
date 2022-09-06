using CqrsLibrary.Commands;
using CqrsLibrary.Domain;
using CqrsLibrary.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CqrsApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICommandSender sender;
        private readonly IEventPublisher publisher;
        private readonly IReadModelFacade readmodel;
        public HomeController(ICommandSender sender, IEventPublisher publisher, IReadModelFacade readmodel)
        {
            this.sender = sender;
            this.publisher = publisher;
            this.readmodel = readmodel;
        }

        [HttpGet]
        public ActionResult<IEnumerable<InventoryItemListDto>> GetList()
        {
            var result = readmodel.GetInventoryItems().ToList();
            if (result.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<InventoryItemDetailsDto> Details(Guid id)
        {
            var result = readmodel.GetInventoryItemDetails(id);

            return result == null ? NoContent() : Ok(result);
        }

        [HttpPost]
        public ActionResult<bool> Add(string name)
        {
            var result = (new CreateInventoryItem(Guid.NewGuid(), name));
            return Ok(true);
        }

        [HttpGet]
        public InventoryItemDetailsDto ChangeName(Guid id)
        {
            return readmodel.GetInventoryItemDetails(id);
        }

        [HttpPost]
        public bool ChangeName(Guid id, string name, int version)
        {
            var command = new RenameInventoryItem(id, name, version);
            sender.Send(command);
            return true;
        }

        [HttpGet]
        public ActionResult<InventoryItemDetailsDto> Deactivate(Guid id)
        {
            var result = readmodel.GetInventoryItemDetails(id);
            return Ok(result);
        }

        [HttpPost]
        public void Deactivate(Guid id, int version)
        {
            sender.Send(new DeactivateInventoryItem(id, version));
        }
        [HttpGet]
        public ActionResult<bool> CheckIn(Guid id)
        {
            var result = readmodel.GetInventoryItemDetails(id);
            return Ok(true);
        }

        [HttpPost]
        public ActionResult<bool> CheckIn(Guid id, int number, int version)
        {
            sender.Send(new CheckInItemsToInventory(id, number, version));
            return Ok(true);
        }
        [HttpGet]
        public ActionResult<InventoryItemDetailsDto> Remove(Guid id)
        {
            var result = readmodel.GetInventoryItemDetails(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<bool> Remove(Guid id, int number, int version)
        {
            sender.Send(new RemoveItemsFromInventory(id, number, version));
            return Ok(true);

        }
    }
}
