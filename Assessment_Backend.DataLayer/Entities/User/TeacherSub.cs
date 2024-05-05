
namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("TeacherSub", Schema = "User")]
    public class TeacherSub
    {

        [Key]
        public int TS_Id { get; set; }

        public int SubsId { get; set; }

        public int TeacherId { get; set; }

        public int Count { get; set; }

        #region Relations

        public virtual Teacher? Teacher { get; set; }

        public virtual Sub? Sub { get; set; }
        #endregion
    }
}
