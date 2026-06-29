using System.ComponentModel.DataAnnotations.Schema;
using MyFinance.Domain.Common;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

[Table("savings_goals")]
public class SavingsGoal : AuditableEntity, IAggregateRoot
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("target_amount")]
    public decimal TargetAmount { get; set; }

    [Column("current_amount")]
    public decimal CurrentAmount { get; set; }

    [Column("deadline")]
    public DateTime? Deadline { get; set; }

    [Column("icon")]
    public string Icon { get; set; } = "piggy-bank";

    [Column("color")]
    public string Color { get; set; } = "#10B981";

    [Column("status")]
    public SavingsGoalStatus Status { get; set; } = SavingsGoalStatus.Active;

    public User User { get; set; } = null!;
}
