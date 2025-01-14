using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Invoice
{
    public decimal InvoiceId { get; set; }

    public decimal SubscriptionId { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public byte[]? InvoicePdf { get; set; }

    public decimal? PaymentId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Subscription? Subscription { get; set; }
} 
