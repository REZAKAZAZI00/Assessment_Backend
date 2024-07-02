namespace Assessment_Backend.DataLayer.Entities.duty
{
    [Table("Term", Schema = "duty")]

    public class Term
    {
        [Key]
        public int TermId { get; set; }

        public string Title { get; set; }

        public bool IsDelete { get; set; }


        #region Relations

        public List<Course> Course { get; set; }

        #endregion
    }
}
