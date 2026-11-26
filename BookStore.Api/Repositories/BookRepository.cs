using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using BookStore.Api.Data;
namespace BookStore.Api.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(string? search = null,
        string? sortBy = null,
        int pageNumber = 1,
        int pageSize = 10)
        {
            // 1. Khởi tạo câu truy vấn dạng IQueryable (chưa chạy xuống DB)
            var query = _context.Books.AsQueryable();

            // 2. Áp dụng các điều kiện tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            // 3. Sắp xếp nếu có
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "price_desc" => query.OrderByDescending(b => b.Price), // Giá giảm dần
                    "price_asc" => query.OrderBy(b => b.Price),           // Giá tăng dần
                    "title" => query.OrderBy(b => b.Title),           // Tên A-Z
                    _ => query.OrderBy(b => b.Id)               // Mặc định theo Id
                };
            }

            // 4. Phân trang
            var books = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return books;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }
        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }
        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
