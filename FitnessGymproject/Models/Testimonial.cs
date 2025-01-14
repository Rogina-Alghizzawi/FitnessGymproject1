using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Testimonial
{
    public decimal TestimonialId { get; set; }

    public decimal MemberId { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Member? Member { get; set; } 
}
