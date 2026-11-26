using BookStore.Api.DTOs;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using BookStore.Api.Repositories;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BooksController : ControllerBase
{
    // BÂY GIỜ CHÚNG TA CHỈ GIAO TIẾP QUA INTERFACE NÀY!
    private readonly IBookRepository _bookRepo;

    // Dependency Injection: Hệ thống sẽ tự động bơm BookRepository vào đây
    public BooksController(IBookRepository bookRepo)
    {
        _bookRepo = bookRepo;
    }

    // 2. LẤY TẤT CẢ SÁCH
    [HttpGet]
    public async Task<IActionResult> GetAllBooks([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        // Dùng LINQ .Select() để chỉ nhặt những cột cần thiết biến thành DTO
        var books = await _bookRepo.GetAllAsync(search, sortBy, pageNumber, pageSize);

        var booksDtos = books.Select(book => new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price
        });
        return Ok(booksDtos);
    }

    // 3. LẤY SÁCH THEO ID: GET api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookRepo.GetByIdAsync(id);

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



        // Giao việc lưu sách cho Repository lo liệu

        await _bookRepo.AddAsync(newBook);

        var responseDto = new BookResponseDto
        {
            Id = newBook.Id,
            Title = newBook.Title,
            Author = newBook.Author,
            Price = newBook.Price
        };

        // Sau khi SaveChangesAsync, SQL Server sẽ TỰ ĐỘNG sinh ra Id thật cho newBook!
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, responseDto);
    }

    // 5. CẬP NHẬT SÁCH: PUT api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto bookDto)
    {
        var exsitingBook = await _bookRepo.GetByIdAsync(id);
        if(exsitingBook == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id} để cập nhật!"); // 404
        }
        exsitingBook.Title = bookDto.Title;
        exsitingBook.Author = bookDto.Author;
        exsitingBook.Price = bookDto.Price;

        await _bookRepo.UpdateAsync(exsitingBook);
        return NoContent();
    }

    // 6. XÓA SÁCH: DELETE api/books/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _bookRepo.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound($"Không tìm thấy cuốn sách có Id = {id} để xóa!"); // 404
        }
        // Giao việc xóa cho Repository
        await _bookRepo.DeleteAsync(book);

        return NoContent(); // Giao việc xóa cho Repository
    }   

}
