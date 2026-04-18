namespace HUP.BuildingBlocks.Domain { public abstract class DomainEvent { public bool IsPublished { get; set; } public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow; } }
