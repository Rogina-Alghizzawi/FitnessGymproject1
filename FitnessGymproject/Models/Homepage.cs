using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessGymproject.Models;

public partial class Homepage
{
    public decimal HomepageId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }
    public string? VideoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
