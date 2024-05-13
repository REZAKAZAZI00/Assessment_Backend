namespace Assessment_Backend.DataLayer.Entities.duty
{
    [Table("Course", Schema = "duty")]

    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        public int TeacherId { get; set; }

        public int TermId { get; set; }


        public string Title { get; set; }

        public string Description { get; set; }
        public string Link { get; set; }

        public int CountMembers { get; set; }

        public bool IsDelete { get; set; }=false;

        #region Relations

        public Term Term { get; set; }

        public Teacher? Teacher { get; set; }

        public List<Assessment>? Assessments { get; set; }

        public List<CourseEnrollment> CourseEnrollments { get; set; }
        #endregion
    }
}
