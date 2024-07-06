namespace Assessment_Backend.Core.DTOs.Assessment
{
    public class AssessmentDTO
    {
        public int AssessmentId { get; set; }

        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string PenaltyRule { get; set; }

        public string? FileName { get; set; }

        public SubmittedAssignmentDTO? submitted { get; set; }

    }

    public class CreateAssessmentDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public int CourseId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IFormFile? File { get; set; }

        public string PenaltyRule { get; set; }

    }

    public class TestCreateAssessmentDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public int CourseId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string PenaltyRule { get; set; }
    }

    public class UpdateAssessmentDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]

        public int AssessmentId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string  FileName { get; set; }

        public IFormFile? File { get; set; }

        public string PenaltyRule { get; set; }
    }

    public class DeleteAssessmentDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int AssessmentId { get; set; }
    }

    public class ScoreDTO
    {
        public int LastScore { get; set; }

        public string CourseTitle { get; set; }

        public string TermId { get; set; }

    }

    public class ReportDTO
    {
        public StudentDTO Student { get; set; }
        public List<ScoreDTO> scores { get; set; }

        public int ScoreAvrge { get; set; }
    }


}
