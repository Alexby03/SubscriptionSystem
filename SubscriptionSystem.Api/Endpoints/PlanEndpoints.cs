using SubscriptionSystem.Entities;
using SubscriptionSystem.Data;
using Microsoft.EntityFrameworkCore;

public static class PlanEndpoints
{
    public static void MapPlanEndpoints(this WebApplication app)
    {
        // List all available plans
        app.MapGet("/plans", async (AppDbContext db) =>
        {
            var plans = await db.Plans.ToListAsync();
            return Results.Ok(plans);
        });

        // Get details of a single plan by id
        app.MapGet("/plans/{id}", async (int id, AppDbContext db) =>
        {
            var plan = await db.Plans.FirstOrDefaultAsync(p => p.PlanId == id);
            if (plan == null)
            {
                return Results.NotFound(new { Message = $"Plan with id {id} not found." });
            }
            return Results.Ok(plan);
        });
    }
}
