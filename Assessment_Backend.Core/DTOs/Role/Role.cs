namespace Assessment_Backend.Core.DTOs.Role
{
    public class RoleDTO
    {
        public int RoleId { get; set; }

        [Display(Name = "عنوان  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public required string Title { get; set; }
    }
}
