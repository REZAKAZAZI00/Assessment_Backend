namespace Assessment_Backend.Core.DTOs.Assessment
{
    public class AssignmentSubmissionDTO
    {

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int AssignmentId { get; set; }

        public string Text { get; set; }

        public IFormFile? File { get; set; }
       
    }

    public class SubmittedAssignmentDTO
    {

        public int AS_Id { get; set; }
        public int AssignmentId { get; set; }
        public int RawScore { get; set; }

        public int LateScore { get; set; }

        public string Text { get; set; }
        public string FileName { get; set; }
        public DateTime? ReviewedDate { get; set; }

        public DateTime CreateDate { get; set; }

        public StudentDTO? Student { get; set; }


    }

    public class ScoreRegistrationDTO
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int AS_Id { get; set; }


        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Score { get; set; }
    }
}
