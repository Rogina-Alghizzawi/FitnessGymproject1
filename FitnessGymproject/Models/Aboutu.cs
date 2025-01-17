using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessGymproject.Models;

public partial class Aboutu
{
    public decimal AboutUsId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? ImageUrl { get; set; }
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }


    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
