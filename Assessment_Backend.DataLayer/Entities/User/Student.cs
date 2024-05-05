

namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("Student", Schema = "User")]

    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        public required int UserId { get; set; }

        public int GradeId { get; set; }

        [Display(Name = " نام  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(70, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public string Name { get; set; }
        [Display(Name = " نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(70, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]

        public string family { get; set; }

        [Display(Name = "شماره تلفن همراه ")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل ")]
        [MaxLength(150, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string Email { get; set; }


        #region Relations

        public  User User { get; set; }

        public  Grade? Grade { get; set; }

        public List<CourseEnrollment> CourseEnrollments { get; set; }

        public  List<AssignmentSubmission> AssignmentSubmissions { get; set; }

        #endregion
    }
}
