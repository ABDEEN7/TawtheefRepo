using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tawtheef.Application.Common.Interfaces.Logging;

namespace Tawtheef.Infrastructure.Data.Interceptors;

public class SlowQueryInterceptor(IAppLogger logger, int slowQueryThreshold = 200) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > slowQueryThreshold)
        {
            logger.Warning($"Slow query detected ({eventData.Duration.TotalMilliseconds} ms): {command.CommandText}");
        }

        return base.ReaderExecuted(command, eventData, result);
    }
}
