using TechsysLog.Domain.Common;

namespace TechsysLog.TestUtilities.Doubles;

public sealed class FixedDateTimeProvider(DateTime fixedUtcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; private set; } = fixedUtcNow;

    public void Advance(TimeSpan delta) => UtcNow = UtcNow.Add(delta);
    public void Set(DateTime newUtcNow) => UtcNow = newUtcNow;

    public static FixedDateTimeProvider AtUtc(int year, int month, int day, int hour = 12, int minute = 0, int second = 0) =>
        new(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));
}
