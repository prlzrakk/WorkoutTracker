using Infrastructure.Db;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WorkoutTracker.Api.Extensions;

public class DatabaseHealthCheck(ProjectContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect 
                ? HealthCheckResult.Healthy("Database is connected")
                : HealthCheckResult.Unhealthy("Database is not connected");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unavailable", ex);
        }
    }
}