namespace Assessment_Backend.Core.DTOs.Course
{

    public class CourseDTO
    {
        public int CourseId { get; set; }

        public string  TeacherName { get; set; }

        public string Term { get; set; }
        public int TermId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public string Link { get; set; }

        public int CountMembers { get; set; }

        public int? extant { get; set; }

        public List<StudentDTO>? Student { get; set; }

        public List<AssessmentDTO> Assessments { get; set; }
    }

    public class CreateCourseDTO
    {
        public int TeacherId { get; set; }

        [Required]
        public int TermId { get; set; }

        [Display(Name = "عنوان  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Title { get; set; }

        public string Description { get; set; }

        public int CountMembers { get; set; }



    }

    public class UpdateCourseDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public int CourseId { get; set; }


        public int TermId { get; set; }


        public string Title { get; set; }

        public string Description { get; set; }

        public bool ChangeLink { get; set; }

        public int CountMembers { get; set; }
    }

    public class DeleteCourseDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CourseId { get; set; }
    }
    public class JoinClassDTO
    {
        public required string ClassLink { get; set; }


    }

    public class LeavingClassDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CourseId { get; set; }
    }
}
