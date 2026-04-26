using System.ComponentModel.DataAnnotations;

namespace MyCompany.Domain.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Заполните название")]
        [Display(Name = "Название")]
        [MaxLength(200)]
        public string? Title { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

    }
}
