using System;

namespace BuildingBlocks.Domain
{
    public abstract class DomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}