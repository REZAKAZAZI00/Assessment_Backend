namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("User", Schema = "User")]

    public class User
    {
        [Key]
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Display(Name = " کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public required string  Password { get; set; }

        [Display(Name = " کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public required string CodeMelli { get; set; }


        #region Relations

        public  Role Role { get; set; }
        public  Student Student { get; set; }
        public  Teacher Teacher { get; set; }
        #endregion
    }
}
