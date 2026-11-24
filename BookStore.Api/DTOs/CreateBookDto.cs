using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.DTOs
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Tiêu đề sách là bắt buộc!")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên tác giả là bắt buộc!")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sách là bắt buộc!")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sách không được là số âm!")]
        public decimal Price { get; set; }
    }
}
