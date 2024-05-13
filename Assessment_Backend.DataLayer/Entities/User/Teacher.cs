namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("Teacher", Schema = "User")]

    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        public int UserId { get; set; }


        [Display(Name = " نام  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(70, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Name { get; set; }

        [Display(Name = " نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(70, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string family { get; set; }

        [Display(Name = "ایمیل ")]
        [MaxLength(150, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string Email { get; set; }


        [Display(Name = "شماره تلفن همراه ")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PhoneNumber { get; set; }

        [Display(Name = "کد استادی ")]
        [MaxLength(15, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TeacherCode  { get; set; }

        public bool IsDelete { get; set; }

        #region Relations

        public User User { get; set; }


        #endregion
    }
}
