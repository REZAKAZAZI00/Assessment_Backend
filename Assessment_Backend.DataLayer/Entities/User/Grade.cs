namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("Grade", Schema = "User")]

    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Display(Name = "عنوان  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Title { get; set; }

        public bool IsDelete { get; set; }

        #region Relations

        public List<Student>? Students { get; set; }
        #endregion

    }
}
