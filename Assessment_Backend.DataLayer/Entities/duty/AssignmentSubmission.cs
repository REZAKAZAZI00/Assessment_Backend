namespace Assessment_Backend.DataLayer.Entities.duty
{
    [Table("AssignmentSubmission", Schema = "duty")]

    public class AssignmentSubmission
    {
        [Key]
        public int AS_Id { get; set; }

        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int RawScore { get; set; }

        public int LateScore { get; set; }

        public string Text { get; set; }
        public string FileName { get; set; }
        public DateTime? ReviewedDate { get; set; }

        public DateTime CreateDate { get; set; }

        #region Relations
        public Student Student { get; set; }

        public Assessment Assessment { get; set; }

        #endregion
    }
}

