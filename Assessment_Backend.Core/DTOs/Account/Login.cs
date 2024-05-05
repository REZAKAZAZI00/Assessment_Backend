namespace Assessment_Backend.Core.DTOs.Account
{
    public class LoginDTO
    {

        [Display(Name = " کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public required string Password { get; set; }

        [Display(Name = " کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public required string CodeMelli { get; set; }
    }
}
