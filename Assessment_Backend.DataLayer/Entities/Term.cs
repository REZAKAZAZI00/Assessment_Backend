namespace Assessment_Backend.DataLayer.Entities
{
    public class Term
    {
        [Key]
        public int TermId { get; set; }

        public string Title { get; set; }

        public bool IsDelete { get; set; }


        #region Relations

        public Course Course { get; set; }

        #endregion
    }
}
