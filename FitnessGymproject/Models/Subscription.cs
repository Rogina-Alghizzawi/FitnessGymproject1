using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Subscription
{
    public decimal SubscriptionId { get; set; }
    public DateTime? EndDate { get; set; }

    public decimal MemberId { get; set; }

    public string? PlanName { get; set; }

    public DateTime? StartDate { get; set; }

    public decimal? TotalPayment { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? PaymentId { get; set; }

    public decimal? WorkoutPlanId { get; set; }

    public short? DurationDays { get; set; }

    public decimal? MembershipPlanId { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Member? Member { get; set; } 

    public virtual MembershipPlan? MembershipPlan { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Workoutplan? WorkoutPlan { get; set; }
}
