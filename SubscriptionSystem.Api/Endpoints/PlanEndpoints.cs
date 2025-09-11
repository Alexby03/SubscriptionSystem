using SubscriptionSystem.Models;

public static class PlanEndpoints
{
    public static void MapPlanEndpoints(this WebApplication app)
    {
        //list all available plans
        app.MapGet("/plans", () => Results.Ok(Plans.All));

        //plan details
        app.MapGet("/plans/{id}", (int id) =>
        {
            if (Plans.All.TryGetValue(id, out var plan))
            {
                return Results.Ok(plan);
            }
            return Results.NotFound(new { Message = "Plan not found." });
        });
    }
}