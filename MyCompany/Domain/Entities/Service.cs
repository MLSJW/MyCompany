using MyCompany.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyCompany.Domain.Entities
{
    public class Service : EntityBase
    {
        [Display(Name = "Выберите категорию услуги")]
        public int? ServiceCategoryId { get; set; }
        public ServiceCategory? ServiceCategory { get; set; }

        [Display(Name = "Краткое описание")]
        [MaxLength(3_000)]
        public string? DescriptionShort { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(100000)]
        public string? Description { get; set; }

        [Display(Name = "Титульная картинка")]
        [MaxLength(300)]
        public string? Photo { get; set; }

        [Display(Name = "Тип услуги")]
        public ServiceTypeEnum Type { get; set; }

    }
}
