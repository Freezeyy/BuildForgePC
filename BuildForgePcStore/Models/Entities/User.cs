using System.ComponentModel.DataAnnotations;

namespace BuildForgePcStore.Models.Entities;

public class User
{
    public int UserId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Customer or Admin</summary>
    [Required, MaxLength(20)]
    public string Role { get; set; } = UserRoles.Customer;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
