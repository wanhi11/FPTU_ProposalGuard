using System.Collections;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Infrastructure.Data.Context;
using FPTU_ProposalGuard.Infrastructure.Repositories;

namespace FPTU_ProposalGuard.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly FptuProposalGuardDbContext _context;
    private Hashtable _repositories;
    
    public UnitOfWork(FptuProposalGuardDbContext context)
    {
        _context = context;

        // Initialize repo hashtable if not exist
        if (_repositories == null) _repositories = new Hashtable();
    }
    
    public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class
    {
        
		// Retrieves the name of the entity type
		var type = typeof(TEntity).Name;

		// Checks repository for particular entity is created
		if (!_repositories.ContainsKey(type))
		{
			// Defines the type of the generic repository
			var repositoryType = typeof(GenericRepository<TEntity, TKey>);

			// Creates an instance of the generic repository 
			var repositoryInstance = Activator.CreateInstance(repositoryType, _context);

			// Adds the created repository to the Hashtable
			_repositories.Add(type, repositoryInstance);
		}

		return (IGenericRepository<TEntity, TKey>)_repositories[type]!;
	}

    public int SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public int SaveChangesWithTransaction()
    {
        int result = -1;

        // Starts a new database transaction
        using (var dbContextTransaction = _context.Database.BeginTransaction())
        {
            try
            {
                // Saves all changes and commits the transaction if successful
                result = _context.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception)
            {
                // If an exception occurs, the transaction is rolled back
                result = -1;
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        return result;
    }

    public async Task<int> SaveChangesWithTransactionAsync()
    {
        int result = -1;

        // Starts a new database transaction
        using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Saves all changes and commits the transaction if successful
                result = await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }
            catch (Exception)
            {
                // If an exception occurs, the transaction is rolled back
                result = -1;
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        return result;
    }

    public void Dispose() => _context.Dispose();
}