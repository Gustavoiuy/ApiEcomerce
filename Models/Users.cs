using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class Users
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Password { get; set; }
}
