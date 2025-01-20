using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FitnessGymproject.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aboutu> Aboutus { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Contactu> Contactus { get; set; }

    public virtual DbSet<Homepage> Homepages { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MembershipPlan> MembershipPlans { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<Workoutplan> Workoutplans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("DATA SOURCE=DESKTOP-S48O5C4:1521;USER ID=C##GYM;PASSWORD=Test321;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##GYM")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Aboutu>(entity =>
        {
            entity.HasKey(e => e.AboutUsId).HasName("SYS_C009040");

            entity.ToTable("ABOUTUS");

            entity.Property(e => e.AboutUsId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ABOUT_US_ID");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGE_URL");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TITLE");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE\n")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
            
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("SYS_C008988");

            entity.ToTable("ADMIN");

            entity.HasIndex(e => e.Email, "SYS_C008989").IsUnique();

            entity.Property(e => e.AdminId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ADMIN_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Imageprofileurl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPROFILEURL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Contactu>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("SYS_C009038");

            entity.ToTable("CONTACTUS");

            entity.Property(e => e.ContactId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CONTACT_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE\n")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
        });

        modelBuilder.Entity<Homepage>(entity =>
        {
            entity.HasKey(e => e.HomepageId).HasName("SYS_C009042");

            entity.ToTable("HOMEPAGE");

            entity.Property(e => e.HomepageId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("HOMEPAGE_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("LOGO_URL");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TITLE");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE\n")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");

        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("SYS_C009027");

            entity.ToTable("INVOICE");

            entity.Property(e => e.InvoiceId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("INVOICE_ID");
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("INVOICE_DATE");
            entity.Property(e => e.InvoicePdf)
                .HasColumnType("BLOB")
                .HasColumnName("INVOICE_PDF");
            entity.Property(e => e.PaymentId)
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENT_ID");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PAYMENT_STATUS");
            entity.Property(e => e.SubscriptionId)
                .HasColumnType("NUMBER")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("NUMBER")
                .HasColumnName("TOTAL_AMOUNT");

            entity.HasOne(d => d.Payment).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_INVOICE_PAYMENT");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("SYS_C009028");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("SYS_C008994");

            entity.ToTable("MEMBER");

            entity.HasIndex(e => e.Email, "SYS_C008995").IsUnique();

            entity.Property(e => e.MemberId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Imageprofileurl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPROFILEURL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<MembershipPlan>(entity =>
        {
            entity.HasKey(e => e.MembershipPlanId).HasName("SYS_C009052");

            entity.ToTable("MEMBERSHIP_PLAN");

            entity.Property(e => e.MembershipPlanId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBERSHIP_PLAN_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.DurationDays)
                .HasPrecision(5)
                .HasColumnName("DURATION_DAYS");
            entity.Property(e => e.IncludedServices)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("INCLUDED_SERVICES");
            entity.Property(e => e.PlanDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PLAN_DESCRIPTION");
            entity.Property(e => e.PlanName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PLAN_NAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE\n")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("SYS_C009017");

            entity.ToTable("PAYMENT");

            entity.Property(e => e.PaymentId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENT_ID");

            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER")
                .HasColumnName("AMOUNT");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");

            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBER_ID");

            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("PAYMENT_DATE");

            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYMENT_METHOD");

            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Success'")
                .HasColumnName("PAYMENT_STATUS");

            // Nullable SubscriptionId field
            entity.Property(e => e.SubscriptionId)
                .HasColumnType("NUMBER")
                .HasColumnName("SUBSCRIPTION_ID")
                .IsRequired(false); // Makes the field nullable

            // New properties for card details
            entity.Property(e => e.CardNumber)
                .HasMaxLength(19)
                .IsUnicode(false)
                .HasColumnName("CARD_NUMBER");

            entity.Property(e => e.ExpirationDate)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("EXPIRATION_DATE");

            entity.Property(e => e.SecurityCode)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("SECURITY_CODE");

            // Relationships
            entity.HasOne(d => d.Member)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C009018");

            // Updated to handle nullable SubscriptionId
            entity.HasOne(d => d.Subscription)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull) // Retain 'ClientSetNull' behavior for null subscriptions
                .HasConstraintName("FK_PAYMENT_SUBSCRIPTION");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("SYS_C009021");

            entity.ToTable("SUBSCRIPTION");

            entity.Property(e => e.SubscriptionId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.DurationDays)
                .HasPrecision(5)
                .HasDefaultValueSql("30")
                .HasColumnName("DURATION_DAYS");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.MembershipPlanId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBERSHIP_PLAN_ID");
            entity.Property(e => e.PaymentId)
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENT_ID");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("PAYMENT_STATUS");
            entity.Property(e => e.PlanName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PLAN_NAME");
            entity.Property(e => e.StartDate)
                .HasColumnType("DATE")
                .HasColumnName("START_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.TotalPayment)
                .HasColumnType("NUMBER")
                .HasColumnName("TOTAL_PAYMENT");
            entity.Property(e => e.WorkoutPlanId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("WORKOUT_PLAN_ID");
            entity.Property(e => e.EndDate)
      .HasColumnType("DATE")
      .HasColumnName("END_DATE");
            entity.HasOne(d => d.Member).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C009022");

            entity.HasOne(d => d.MembershipPlan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.MembershipPlanId)
                .HasConstraintName("FK_SUBSCRIPTION_MEMBERSHIP_PLAN");

            entity.HasOne(d => d.Payment).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SYS_C009024");

            entity.HasOne(d => d.WorkoutPlan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.WorkoutPlanId)
                .HasConstraintName("FK_WORKOUTPLAN_SUBSCRIPTION");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.TestimonialId).HasName("SYS_C009004");

            entity.ToTable("TESTIMONIAL");

            entity.Property(e => e.TestimonialId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIAL_ID");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Member).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("SYS_C009005");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("SYS_C009000");

            entity.ToTable("TRAINER");

            entity.HasIndex(e => e.Email, "SYS_C009001").IsUnique();

            entity.Property(e => e.TrainerId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TRAINER_ID");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("BIO");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Imageprofileurl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPROFILEURL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SPECIALIZATION");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
        });

        modelBuilder.Entity<Workoutplan>(entity =>
        {
            entity.HasKey(e => e.WorkoutPlanId).HasName("SYS_C009009");

            entity.ToTable("WORKOUTPLAN");

            entity.Property(e => e.WorkoutPlanId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("WORKOUT_PLAN_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Exercises)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("EXERCISES");
            entity.Property(e => e.Goals)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("GOALS");
            entity.Property(e => e.MemberId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBER_ID");
            entity.Property(e => e.MembershipPlanId)
                .HasColumnType("NUMBER")
                .HasColumnName("MEMBERSHIP_PLAN_ID");
            entity.Property(e => e.PlanName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PLAN_NAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Schedule)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SCHEDULE");
            entity.Property(e => e.TrainerId)
                .HasColumnType("NUMBER")
                .HasColumnName("TRAINER_ID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");

            entity.HasOne(d => d.Member).WithMany(p => p.Workoutplans)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C009011");

            entity.HasOne(d => d.MembershipPlan).WithMany(p => p.Workoutplans)
                .HasForeignKey(d => d.MembershipPlanId)
                .HasConstraintName("FK_WORKOUT_PLAN_MEMBERSHIP_PLAN");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Workoutplans)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("SYS_C009010");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
