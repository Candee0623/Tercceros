using System.ComponentModel.DataAnnotations;
using backend.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace backend.Models;

public class EventoCalendario
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public bool IsAllDay { get; set; } = false;

    [MaxLength(100)]
    public string? Location { get; set; }

    [Required]
    public Guid UsuarioId { get; set; }

    [ValidateNever]
    public Usuario? Usuario { get; set; }
}
