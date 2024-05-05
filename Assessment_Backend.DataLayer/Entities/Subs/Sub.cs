namespace Assessment_Backend.DataLayer.Entities.Subs
{
    [Table("Sub", Schema = "Subs")]

    public class Sub
    {

        [Key]
        public int SubsId { get; set; }

        [Required]
        public int SubTypeId { get; set; }

        [Required]
        public DateTime BoughtDate { get; set; }

        [Required]
        public int PurchasedPrice { get; set; }

        #region Relations

        public SubType? SubType { get; set; }
        public  List<TeacherSub>?  TeacherSubs { get; set; }
        #endregion
    }
}
