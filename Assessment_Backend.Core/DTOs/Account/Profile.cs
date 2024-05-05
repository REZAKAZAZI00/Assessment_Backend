namespace Assessment_Backend.Core.DTOs.Account
{
    public class UserProfileDTO
    {

        [Display(Name = " کد ملی")]
        public required string CodeMelli { get; set; }

        public int StudentId { get; set; }

        public int TeacherId { get; set; }

        public int UserId { get; set; }

        public string? Grade { get; set; }


        [Display(Name = " نام  ")]
        public string Name { get; set; }

        [Display(Name = " نام خانوادگی")]
        public string family { get; set; }

        [Display(Name = "ایمیل ")]
        public string Email { get; set; }


        [Display(Name = "شماره تلفن همراه ")]
        public string PhoneNumber { get; set; }

        [Display(Name = "کد استادی ")]
        public string? TeacherCode { get; set; }



    }
}
