using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Contract.SharedKernel
{
    public abstract class EntityBase
    {
        public string Id { get; set; }=new Guid().ToString();
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTimeOffset CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 创建人
        /// </summary>

        public string CreatorId { get; set; } = String.Empty;

        /// <summary>
        /// 是否软删除
        /// </summary>
        public bool Deleted { get; set; } = false;
        private List<DomainEventBase> _domainEvents = new List<DomainEventBase>();
        [NotMapped]
        public IEnumerable<DomainEventBase> DomainEvents=> _domainEvents.AsReadOnly();
        protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
        internal void ClearDomainEvents() => _domainEvents.Clear();
    }
}
