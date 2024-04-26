
namespace Assessment_Backend.DataLayer.Entities
{
    public  class Grade
    {
        [Key]
        public int GradeId  { get; set; }

        public string Title { get; set; }

        public bool IsDelete { get; set; }

        #region Relations

        public List<Student>? Students { get; set; }
        #endregion

    }
}
