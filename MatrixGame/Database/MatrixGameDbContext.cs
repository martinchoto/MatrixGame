using MatrixGame.Database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace MatrixGame.Database
{
	public class MatrixGameDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(Constants.ConnectionString);
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PlayableCharacter>()
				.Property(p => p.Class)
				.IsRequired();
		}

		internal async Task AsyncSaveChanges()
		{
			throw new NotImplementedException();
		}

		public DbSet<PlayableCharacter> PlayableCharacters { get; set; } = null!;
	}
}
