using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholder.DataAccess.Dto;

public class CreateUserDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Il campo name è richiesto")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Il nome deve essere lungo tra 3 e 100 caratteri")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Il campo username è richiesto")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Lo username deve essere lungo tra 3 e 50 caratteri")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Il campo email è richiesto")]
    [EmailAddress(ErrorMessage = "Il formato dell'email non è valido")]
    [StringLength(100, ErrorMessage = "L'indirizzo email deve essere lungo al massimo 100 caratteri")]
    public required string Email { get; set; }

    [StringLength(50, ErrorMessage = "Il numero di telefono può essere lungo al massimo 50 caratteri")]
    public string? Phone { get; set; }

    [StringLength(100, ErrorMessage = "L'indirizzo del sito web può essere lungo al massimo 100 caratteri")]
    public string? Website { get; set; }
}