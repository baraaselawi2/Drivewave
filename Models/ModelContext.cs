using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace rental.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutU> AboutUs { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Creditcard> Creditcards { get; set; }

    public virtual DbSet<Home> Homes { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("DATA SOURCE=localhost:1521/orcl;USER ID=C##cab_car;PASSWORD=my_password;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##CAB_CAR")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<AboutU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C007528");

            entity.ToTable("ABOUT_US");

            entity.Property(e => e.Id)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Text)
                .IsUnicode(false)
                .HasColumnName("TEXT");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("SYS_C007505");

            entity.ToTable("CAR");

            entity.HasIndex(e => e.CarNumber, "SYS_C007506").IsUnique();

            entity.Property(e => e.CarId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CAR_ID");
            entity.Property(e => e.CarColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAR_COLOR");
            entity.Property(e => e.CarName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CAR_NAME");
            entity.Property(e => e.CarNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAR_NUMBER");
            entity.Property(e => e.CarSet)
                .HasColumnType("NUMBER")
                .HasColumnName("CAR_SET");
            entity.Property(e => e.CarValue2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CAR_VALUE2");
            entity.Property(e => e.CarValue3)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CAR_VALUE3");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("SYS_C007526");

            entity.ToTable("CONTACT_US");

            entity.Property(e => e.ContactId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CONTACT_ID");
            entity.Property(e => e.ContactDate)
                .HasColumnType("DATE")
                .HasColumnName("CONTACT_DATE");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.Message)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SUBJECT");
        });

        modelBuilder.Entity<Creditcard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C007535");

            entity.ToTable("CREDITCARD");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("BALANCE");
            entity.Property(e => e.Cardnumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CARDNUMBER");
            entity.Property(e => e.Cvv)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CVV");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Expiredate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIREDATE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Useracountfk)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USERACOUNTFK");

            entity.HasOne(d => d.UseracountfkNavigation).WithMany(p => p.Creditcards)
                .HasForeignKey(d => d.Useracountfk)
                .HasConstraintName("SYS_C007536");
        });

        modelBuilder.Entity<Home>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C007527");

            entity.ToTable("HOME");

            entity.Property(e => e.Id)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ID");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.Logopath)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("LOGOPATH");
            entity.Property(e => e.Paragraph)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("PARAGRAPH");
            entity.Property(e => e.WebsiteName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WEBSITE_NAME");
            entity.Property(e => e.Websitephone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("WEBSITEPHONE");
            entity.Property(e => e.Wensiteemail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WENSITEEMAIL");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("SYS_C007519");

            entity.ToTable("TESTIMONIAL");

            entity.Property(e => e.CommentId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("COMMENT_ID");
            entity.Property(e => e.CreatDate)
                .HasColumnType("DATE")
                .HasColumnName("CREAT_DATE");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserIdfk)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_IDFK");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");

            entity.HasOne(d => d.UserIdfkNavigation).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.UserIdfk)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C007520");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("SYS_C007514");

            entity.ToTable("TRIP");

            entity.Property(e => e.TripId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRIP_ID");
            entity.Property(e => e.CarIdfk)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CAR_IDFK");
            entity.Property(e => e.EndTime)
                .HasPrecision(6)
                .HasColumnName("END_TIME");
            entity.Property(e => e.LocationDrop)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOCATION_DROP");
            entity.Property(e => e.LocationPickup)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LOCATION_PICKUP");
            entity.Property(e => e.StartTime)
                .HasPrecision(6)
                .HasColumnName("START_TIME");
            entity.Property(e => e.UserIdfk)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_IDFK");
               entity.Property(e => e.Status)
        .HasMaxLength(20)
        .IsUnicode(false)
        .HasColumnName("STATUS");

            entity.HasOne(d => d.CarIdfkNavigation).WithMany(p => p.Trips)
                .HasForeignKey(d => d.CarIdfk)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C007516");

            entity.HasOne(d => d.UserIdfkNavigation).WithMany(p => p.Trips)
                .HasForeignKey(d => d.UserIdfk)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C007515");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("SYS_C007511");

            entity.ToTable("USER_ACCOUNT");

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CITY");
            entity.Property(e => e.CreatDate)
                .HasColumnType("DATE")
                .HasColumnName("CREAT_DATE");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("GENDER");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ROLE");
            entity.Property(e => e.UserImage)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_IMAGE");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    internal async Task GetUserAsync(ClaimsPrincipal user)
    {
        throw new NotImplementedException();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
