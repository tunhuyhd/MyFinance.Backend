using System.ComponentModel.DataAnnotations.Schema;
using MyFinance.Domain.Common;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

[Table("accounts")]
public class Account : AuditableEntity, IAggregateRoot
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("account_type")]
    public AccountType AccountType { get; set; } = AccountType.Cash;

    [Column("balance")]
    public decimal Balance { get; set; }

    [Column("currency")]
    public string Currency { get; set; } = "VND";

    [Column("color")]
    public string Color { get; set; } = "#3B82F6";

    [Column("icon")]
    public string Icon { get; set; } = "wallet";

    [Column("is_default")]
    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
