using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class MembershipPlan
{
    public decimal MembershipPlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public string PlanDescription { get; set; } = null!;

    public string? IncludedServices { get; set; }

    public decimal Price { get; set; }

    public short DurationDays { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Workoutplan> Workoutplans { get; set; } = new List<Workoutplan>();
}
