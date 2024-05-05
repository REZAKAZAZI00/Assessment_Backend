namespace Assessment_Backend.DataLayer.Entities.duty
{
    [Table("Assessment", Schema = "duty")]

    public class Assessment
    {
        [Key]
        public int AssessmentId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Display(Name = "نام فایل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FileName { get; set; }

        public string PenaltyRule { get; set; }

        public bool IsDelete { get; set; }

        #region Relations
        public Course? Course { get; set; }

        public List<AssignmentSubmission> AssignmentSubmissions { get; set; }

        #endregion
    }
}
