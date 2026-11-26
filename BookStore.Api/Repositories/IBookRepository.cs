using BookStore.Api.Models;

namespace BookStore.Api.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(string? search = null,
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 10);
        Task<Book?> GetByIdAsync(int id);
        Task<Book> AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Book book);

    }
}
