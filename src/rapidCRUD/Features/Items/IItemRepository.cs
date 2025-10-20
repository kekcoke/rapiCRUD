using Microsoft.EntityFrameworkCore;
using rapidCRUD.Infrastructure.Database;

namespace rapidCRUD.Features.Items;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id);
    Task<List<Item>> GetPagedAsync(int page);
    Task<List<Item>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(Item item);
}

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Item?> GetByIdAsync(Guid id)
    {
        return await _context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);    
    }

    public async Task<List<Item>> GetPagedAsync(int page)
    {
        const int pageSize = 10;
        return await _context.Items
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items.CountAsync(cancellationToken);
    }

    public async Task AddAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Item item)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
    }
}