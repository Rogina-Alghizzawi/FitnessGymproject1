using System;
using System.Collections.Generic;

namespace FitnessGymproject.Models;

public partial class Contactu
{
    public decimal ContactId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }
}
