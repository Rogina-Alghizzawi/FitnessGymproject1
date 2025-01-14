using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessGymproject.Models;

public partial class Admin
{
    public decimal AdminId { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Gender { get; set; }

    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }
    public string? Imageprofileurl { get; set; }
}
