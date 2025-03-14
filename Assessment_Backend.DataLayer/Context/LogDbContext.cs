using Assessment_Backend.DataLayer.Entities.Log;

namespace Assessment_Backend.DataLayer.Context;

public class LogDbContext:DbContext
{

    public LogDbContext(DbContextOptions<LogDbContext> options):base(options)
    {
            
    }


    #region Log
    public DbSet<RequestLog> RequestLog { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
           .SelectMany(t => t.GetForeignKeys())
           .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        #region Log

        modelBuilder.Entity<RequestLog>()
            .HasKey(e => e.Id);

        #endregion


        base.OnModelCreating(modelBuilder);

    }

}

