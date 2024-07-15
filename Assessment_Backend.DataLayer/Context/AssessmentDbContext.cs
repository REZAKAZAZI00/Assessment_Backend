namespace Assessment_Backend.DataLayer.Context
{
    public class AssessmentDbContext : DbContext
    {
        public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options) : base(options)
        {

        }


        #region User
       
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TeacherSub> TeacherSubs { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        #endregion

        #region Subs

        public DbSet<Sub> Subs { get; set; }
        public DbSet<SubType> SubTypes { get; set; }
        #endregion

        #region duty
        public DbSet<Term> Terms { get; set; }
        public DbSet<Grade> Grades { get; set; }

        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Course>  Courses { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions  { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments   { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
               .SelectMany(t => t.GetForeignKeys())
               .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<Assessment>()
                .HasQueryFilter(a=> !a.IsDelete);

            modelBuilder.Entity<Course>()
                .HasQueryFilter(a => !a.IsDelete);

            modelBuilder.Entity<Teacher>()
                .HasQueryFilter(t => !t.IsDelete);

            modelBuilder.Entity<Student>()
                .HasQueryFilter(s => !s.IsDelete);

            modelBuilder.Entity<Term>()
                .HasQueryFilter(t => !t.IsDelete);

            modelBuilder.Entity<Grade>()
                .HasQueryFilter(g => !g.IsDelete);

            modelBuilder.Entity<SubType>()
                .HasQueryFilter(s=> !s.IsDelete);

            // اضافه کردن محدودیت unique برای UserId در جدول Teachers
            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u=> u.CodeMelli)
                .IsUnique();    

            modelBuilder.Entity<User>()
                .Property(u=>u.CodeMelli)
                .IsRequired()
                .HasMaxLength(10);

            // اضافه کردن محدودیت unique برای UserId در جدول Students
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.UserId)
                .IsUnique();

            // تنظیم Cascade Delete برای رابطه بین Teacher و User
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیم Cascade Delete برای رابطه بین Student و User
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                      .HasMany(c => c.Assessments)
                      .WithOne(a => a.Course)
                      .HasForeignKey(a => a.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseEnrollments)
                .WithOne(ce => ce.Course)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assessment>()
               .HasMany(a => a.AssignmentSubmissions)
               .WithOne(s => s.Assessment)
               .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);




            base.OnModelCreating(modelBuilder);

        }

    }
}
