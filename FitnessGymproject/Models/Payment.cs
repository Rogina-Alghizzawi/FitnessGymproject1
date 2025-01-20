using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Payment
{
    public decimal PaymentId { get; set; }

    public decimal MemberId { get; set; }

    public decimal? SubscriptionId { get; set; } 

    public DateTime? PaymentDate { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Member? Member { get; set; } 

    public virtual Subscription? Subscription { get; set; }
    public string? CardNumber { get; set; }          
    public string? ExpirationDate { get; set; }     
    public string? SecurityCode { get; set; }
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
