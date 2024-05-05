namespace Assessment_Backend.DataLayer.Entities.duty
{
    [Table("CourseEnrollment", Schema = "duty")]

    public class CourseEnrollment
    {
        [Key]
        public int CE_Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public DateTime DateTime { get; set; }

        #region Relations

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
        #endregion
    }
}
