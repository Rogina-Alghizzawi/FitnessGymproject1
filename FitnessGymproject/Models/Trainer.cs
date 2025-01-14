using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessGymproject.Models;

public partial class Trainer
{
    public decimal TrainerId { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Specialization { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Imageprofileurl { get; set; }
    
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }

    public string? Gender { get; set; }

    public virtual ICollection<Workoutplan> Workoutplans { get; set; } = new List<Workoutplan>();
}
