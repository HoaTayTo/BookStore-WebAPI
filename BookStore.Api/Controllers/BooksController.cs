using BookStore.Api.Data;
using BookStore.Api.DTOs;
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
        // Dùng LINQ .Select() để chỉ nhặt những cột cần thiết biến thành DTO
        var booksDtos = await _context.Books.Select(b=>new BookResponseDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Price = b.Price
        }).ToListAsync();
        return Ok(booksDtos);
    }

    // 3. LẤY SÁCH THEO ID: GET api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id}"); // 404
        }

        var bookDto = new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price
        };

        return Ok(bookDto);

    }

    // 4. TẠO MỚI SÁCH VÀO SQL SERVER: POST api/books
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookDto bookDto)
    {
        // Tạo đối tượng Book từ DTO
        var newBook = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Price = bookDto.Price
        };

        // Thêm newBook vào DbContext (chưa bắn lệnh SQL vào SQL Server)
        _context.Books.Add(newBook);

        // BẮN LỆNH SQL THẬT (INSERT INTO Books...) VÀO SQL SERVER
        await _context.SaveChangesAsync();

        // Sau khi SaveChangesAsync, SQL Server sẽ TỰ ĐỘNG sinh ra Id thật cho newBook!
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
    }

    // 5. CẬP NHẬT SÁCH: PUT api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto bookDto)
    {
        var exsitingBook = await _context.Books.FindAsync(id);
        if(exsitingBook == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id} để cập nhật!"); // 404
        }
        exsitingBook.Title = bookDto.Title;
        exsitingBook.Author = bookDto.Author;
        exsitingBook.Price = bookDto.Price;

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
