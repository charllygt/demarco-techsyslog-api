namespace TechsysLog.Domain.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
