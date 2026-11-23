using BookStore.Api.Data;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    // 1. Dependency Injection: Nhận AppDbContext từ hệ thống
    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    // 2. LẤY TẤT CẢ SÁCH TỪ SQL SERVER: GET api/books
    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }
    // 3. LẤY SÁCH THEO ID: GET api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound($"Không tìm thấy cuốn sách nào có mã ID = {id}!"); // Trả về mã 404 Not Found
        }

        return Ok(book);
    }

    // 4. TẠO MỚI SÁCH VÀO SQL SERVER: POST api/books
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] Book newBook)
    {
        if (newBook.Price < 0)
        {
            return BadRequest("Giá sách không được là số âm!");
        }

        // Thêm vào bộ nhớ theo dõi của EF Core

        _context.Books.Add(newBook);

        // BẮN LỆNH SQL THẬT (INSERT INTO Books...) VÀO SQL SERVER
        await _context.SaveChangesAsync();

        // Sau khi SaveChangesAsync, SQL Server sẽ TỰ ĐỘNG sinh ra Id thật cho newBook!
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
    }

    // 5. CẬP NHẬT SÁCH: PUT api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
    {
        var exsitingBook = await _context.Books.FindAsync(id);
        if(exsitingBook == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id} để cập nhật!"); // 404
        }
        if(updatedBook.Price < 0)
        {
            return BadRequest("Giá sách không được là số âm!"); // 400
        }
        exsitingBook.Title = updatedBook.Title;
        exsitingBook.Author = updatedBook.Author;
        exsitingBook.Price = updatedBook.Price;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 6. XÓA SÁCH: DELETE api/books/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id} để để xóa!"); // 404
        }

        _context.Remove(book);

        await _context.SaveChangesAsync();

        return NoContent();
    }   

 

}
