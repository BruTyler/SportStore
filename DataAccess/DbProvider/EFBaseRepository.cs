
namespace DataAccess.DbProvider
{
    public abstract class EFBaseRepository
    {
        protected readonly EFDbContext _db;

        public EFBaseRepository(EFDbContext context = null)
        {
            _db = context ?? new EFDbContext();
        }
    }
}
