using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Context
{
    public class MainDbContext : DbContext
    {

        public DbSet<MeetingEntry> Meetings { get; set; }
        public DbSet<DealEntry> Deals { get; set; }
        public DbSet<MemoEntry> Memos { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entry>().HasKey(s => s.Id);
            modelBuilder.Entity<Entry>().ToTable("Diary");
            modelBuilder.Entity<Entry>().Property(s => s.EntryType).HasColumnType("smallint");
            modelBuilder.Entity<Entry>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Entry>()
                .HasDiscriminator(s => s.EntryType)
                .HasValue<Entry>(Contracts.EntryType.All)
                .HasValue<DealEntry>(Contracts.EntryType.Deal)
                .HasValue<MemoEntry>(Contracts.EntryType.Memo)
                .HasValue<MeetingEntry>(Contracts.EntryType.Meeting);

            modelBuilder.ApplyConfiguration(new DealConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
            modelBuilder.ApplyConfiguration(new MemoConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new ContactInfoConfiguration());
        }
    }

    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(s => s.Id);
            builder.ToTable("Contact");
            builder.HasQueryFilter(s => !s.IsDeleted);
            builder.HasMany(s => s.ContactInfos).WithOne().HasForeignKey(s => s.ContactId);
        }
    }

    public class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
    {
        public void Configure(EntityTypeBuilder<ContactInfo> builder)
        {
            builder.HasKey(s => s.Id);
            builder.ToTable("ContactInfo");
            builder.HasQueryFilter(s => !s.IsDeleted);
            builder.Property(s => s.ContactInfoType).HasColumnType("smallint");
        }
    }

    public class DealConfiguration : IEntityTypeConfiguration<DealEntry>
    {
        public void Configure(EntityTypeBuilder<DealEntry> builder)
        {
            builder.Property(s => s.EndDate).HasColumnName("EndDate");
            builder.Property(s => s.EntryType).HasColumnType("smallint");
        }
    }

    public class MeetingPlaceConfiguration : IEntityTypeConfiguration<MeetingPlace>
    {
        public void Configure(EntityTypeBuilder<MeetingPlace> builder)
        {
            builder.ToTable("MeetingPlace");
        }
    }

    public class MeetingConfiguration : IEntityTypeConfiguration<MeetingEntry>
    {
        public void Configure(EntityTypeBuilder<MeetingEntry> builder)
        {
            builder.Property(s => s.EndDate).HasColumnName("EndDate");

            builder.HasOne(s => s.MeetingPlace).WithOne()
                .HasPrincipalKey<MeetingEntry>(s => s.Id)
                .HasForeignKey<MeetingPlace>(s => s.Id);
            builder.Property(s => s.EntryType).HasColumnType("smallint");

            builder.Ignore(s => s.Place);
        }
    }

    public class MemoConfiguration : IEntityTypeConfiguration<MemoEntry>
    {
        public void Configure(EntityTypeBuilder<MemoEntry> builder)
        {
            builder.Property(s => s.EntryType).HasColumnType("smallint");
        }
    }
}
