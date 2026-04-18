namespace HUP.BuildingBlocks.Domain { public abstract class ValueObject { protected abstract IEnumerable<object> GetEqualityComponents(); } }
