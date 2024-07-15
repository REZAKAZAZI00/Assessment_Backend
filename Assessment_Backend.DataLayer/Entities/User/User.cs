namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("User", Schema = "User")]

    public class User
    {
        [Key]
        public int UserId { get; set; }

        public Role Role { get; set; }

        [Display(Name = " کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public required string  Password { get; set; }

        [Display(Name = " کد ملی")]
         public required string CodeMelli { get; set; }


        #region Relations

        public virtual  Student Student { get; set; }
        public  virtual Teacher Teacher { get; set; }
        #endregion
    }

    public enum Role:byte
    {
        Teacher,
        Student,

    }
}
