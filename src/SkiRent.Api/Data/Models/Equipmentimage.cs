using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("equipmentimages")]
[Index("EquipmentId", Name = "EquipmentId")]
public partial class EquipmentImage
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int EquipmentId { get; set; }

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [StringLength(255)]
    public string DisplayName { get; set; } = null!;

    [InverseProperty("MainImage")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    [ForeignKey("EquipmentId")]
    [InverseProperty("EquipmentImages")]
    public virtual Equipment EquipmentNavigation { get; set; } = null!;
}
