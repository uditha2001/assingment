using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace userService.Api.entity
{
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long userId {  get; set; }
        [Required]
        public string userName {  get; set; }
        [Required]
        public string password {  get; set; }
        [Required]
        public string firstName {  get; set; }
        [Required]
        public string lastName {  get; set; }
        [Required]
        public string Email { get; set; }
    }
}
