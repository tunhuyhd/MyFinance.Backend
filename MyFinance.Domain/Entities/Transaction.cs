using System.ComponentModel.DataAnnotations.Schema;
using MyFinance.Domain.Common;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

[Table("transactions")]
public class Transaction : AuditableEntity, IAggregateRoot
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("type")]
    public TransactionType Type { get; set; }

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("note")]
    public string? Note { get; set; }

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }

    // For transfers: destination account
    [Column("to_account_id")]
    public Guid? ToAccountId { get; set; }

    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Category? Category { get; set; }
    public Account? ToAccount { get; set; }
}
