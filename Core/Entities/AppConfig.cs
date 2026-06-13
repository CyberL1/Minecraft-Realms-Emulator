using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[PrimaryKey("Key")]
public class AppConfig
{
    public required string Key { get; set; }

    [Column(TypeName = "jsonb")] public required dynamic Value { get; set; }
}
