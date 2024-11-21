using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorldCities.Server.Data.Models
{
    [Table("Countries")]
    [Index(nameof(Name))]
    [Index(nameof(Iso2))]
    [Index(nameof(Iso3))]
    public class Country
    {
        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Iso2 { get; set; }

        public required string Iso3 { get; set; }
        #endregion
        #region Navigation Properties
        public ICollection<City>? Cities { get; set; }
        #endregion
    }
}
