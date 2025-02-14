using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("equipmentcategories")]
[Index("Name", Name = "Name", IsUnique = true)]
public partial class Equipmentcategory
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
