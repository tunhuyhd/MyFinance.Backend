using System.ComponentModel.DataAnnotations.Schema;
using MyFinance.Domain.Common;

namespace MyFinance.Domain.Entities;

[Table("budgets")]
public class Budget : AuditableEntity, IAggregateRoot
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("limit_amount")]
    public decimal LimitAmount { get; set; }

    [Column("spent_amount")]
    public decimal SpentAmount { get; set; }

    [Column("month")]
    public int Month { get; set; }

    [Column("year")]
    public int Year { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
