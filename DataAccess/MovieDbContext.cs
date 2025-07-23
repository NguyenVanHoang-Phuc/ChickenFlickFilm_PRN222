using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class MovieDbContext : DbContext
{
    public MovieDbContext()
    {
    }

    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditorium> Auditoriums { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PriceByType> PriceByTypes { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatBooking> SeatBookings { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Theater> Theaters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasKey(e => e.AuditoriumId).HasName("PK__Auditori__B78BBE8817B6ED66");

            entity.Property(e => e.AuditoriumId).HasColumnName("auditorium_id");
            entity.Property(e => e.AuditoriumName)
                .HasMaxLength(255)
                .HasColumnName("auditorium_name");
            entity.Property(e => e.AuditoriumType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("auditorium_type");
            entity.Property(e => e.ColumnNumber).HasColumnName("column_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.RowNumber).HasColumnName("row_number");
            entity.Property(e => e.TheaterId).HasColumnName("theater_id");
            entity.Property(e => e.TotalSeats).HasColumnName("total_seats");

            entity.HasOne(d => d.AuditoriumTypeNavigation).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.AuditoriumType)
                .HasConstraintName("FK__Auditoriu__audit__31EC6D26");

            entity.HasOne(d => d.Theater).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.TheaterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Auditoriu__creat__30F848ED");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__5DE3A5B1E5D00E36");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(100)
                .HasDefaultValue("Pending")
                .HasColumnName("booking_status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.UserId).HasColumnName("user_id_");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__showti__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__user_i__47DBAE45");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__E79576870B789CDA");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.UserId).HasColumnName("user_id_");

            entity.HasOne(d => d.Movie).WithMany(p => p.Comments)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__Comments__movie___5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__user_i__571DF1D5");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__83CDF74937D0CE74");

            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.AgeRating)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("age_rating");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.Cast).HasColumnName("cast");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Director)
                .HasMaxLength(255)
                .HasColumnName("director");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Format)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("format");
            entity.Property(e => e.Genre)
                .HasMaxLength(255)
                .HasColumnName("genre");
            entity.Property(e => e.Language)
                .HasMaxLength(15)
                .HasColumnName("language");
            entity.Property(e => e.PosterUrl).HasColumnName("poster_url");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("rating");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TrailerUrl)
                .HasMaxLength(500)
                .HasColumnName("trailer_url");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAEFB810A6");

            entity.HasIndex(e => e.BookingId, "UQ__Payments__5DE3A5B0F4C4FC21").IsUnique();

            entity.HasIndex(e => e.TransactionId, "UQ__Payments__85C600AEC2619EEF").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("payment_status");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");
            entity.Property(e => e.VnpayResponseCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("vnpay_response_code");

            entity.HasOne(d => d.Booking).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.BookingId)
                .HasConstraintName("FK__Payments__bookin__534D60F1");
        });

        modelBuilder.Entity<PriceByType>(entity =>
        {
            entity.HasKey(e => e.Type).HasName("PK__PriceByT__9962CFB86027B233");

            entity.ToTable("PriceByType");

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seats__906DED9CF39589C0");

            entity.HasIndex(e => new { e.AuditoriumId, e.SeatNumber }, "unique_seat_per_auditorium").IsUnique();

            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.AuditoriumId).HasColumnName("auditorium_id");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("seat_number");
            entity.Property(e => e.SeatType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("seat_type");

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Seats)
                .HasForeignKey(d => d.AuditoriumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__auditoriu__36B12243");

            entity.HasOne(d => d.SeatTypeNavigation).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatType)
                .HasConstraintName("FK__Seats__seat_type__37A5467C");
        });

        modelBuilder.Entity<SeatBooking>(entity =>
        {
            entity.HasKey(e => e.SeatBookingId).HasName("PK__Seat_Boo__714E8C0F281D75A8");

            entity.ToTable("Seat_Bookings");

            entity.Property(e => e.SeatBookingId).HasColumnName("seat_booking_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.SeatBookings)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat_Book__booki__4BAC3F29");

            entity.HasOne(d => d.Seat).WithMany(p => p.SeatBookings)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat_Book__seat___4CA06362");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__Showtime__A406B518BAB5C55C");

            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.AuditoriumId).HasColumnName("auditorium_id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.ShowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("show_date");
            entity.Property(e => e.ShowTime).HasColumnName("show_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.AuditoriumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__audit__4316F928");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__movie__4222D4EF");
        });

        modelBuilder.Entity<Theater>(entity =>
        {
            entity.HasKey(e => e.TheaterId).HasName("PK__Theaters__B53C958F45B0B3F6");

            entity.Property(e => e.TheaterId).HasColumnName("theater_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MapUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("map_url");
            entity.Property(e => e.TheaterName)
                .HasMaxLength(255)
                .HasColumnName("theater_name");
            entity.Property(e => e.TotalTheaters).HasColumnName("total_theaters");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__EE50E8ED3D487198");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Users__A1936A6BB6F77697").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616436C5B022").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id_");
            entity.Property(e => e.Avatar)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Birthday)
                .HasColumnType("datetime")
                .HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsConfirmed).HasColumnName("isConfirmed");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
