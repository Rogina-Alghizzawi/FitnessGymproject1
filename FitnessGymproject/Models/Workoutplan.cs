using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Workoutplan
{
    public decimal WorkoutPlanId { get; set; }

    public decimal TrainerId { get; set; }

    public decimal? MemberId { get; set; }

    public string? PlanName { get; set; }

    public string? Exercises { get; set; }

    public string? Schedule { get; set; }

    public string? Goals { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? Price { get; set; } 

    public decimal? MembershipPlanId { get; set; }

    public virtual Member? Member { get; set; }

    public virtual MembershipPlan? MembershipPlan { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual Trainer? Trainer { get; set; } 
}
