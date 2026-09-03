using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholder.Core.Dto;

public class CreatePostDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Il campo titolo è richiesto")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Il titolo deve essere lungo tra 3 e 255 caratteri")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Il campo body è richiesto")]
    public required string Body { get; set; }
}