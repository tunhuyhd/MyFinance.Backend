using System.ComponentModel.DataAnnotations.Schema;
using MyFinance.Domain.Common;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

[Table("categories")]
public class Category : AuditableEntity, IAggregateRoot
{
    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("type")]
    public CategoryType Type { get; set; }

    [Column("icon")]
    public string Icon { get; set; } = "tag";

    [Column("color")]
    public string Color { get; set; } = "#6B7280";

    [Column("is_system")]
    public bool IsSystem { get; set; }

    public User? User { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
