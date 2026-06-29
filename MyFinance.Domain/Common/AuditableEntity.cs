using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinance.Domain.Common;

public interface IAuditableEntity
{
    Guid CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    Guid? LastModifiedBy { get; set; }
    DateTime? LastModifiedOn { get; set; }
}

public interface IAggregateRoot { }

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("created_on")]
    public DateTime CreatedOn { get; set; }

    [Column("last_modified_by")]
    public Guid? LastModifiedBy { get; set; }

    [Column("last_modified_on")]
    public DateTime? LastModifiedOn { get; set; }
}
