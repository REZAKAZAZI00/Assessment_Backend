namespace Assessment_Backend.DataLayer.Entities.User
{
    [Table("Role", Schema = "User")]

    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Display(Name = "عنوان  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public required string Title { get; set; }

        public bool IsDelete { get; set; } = false;


        #region Relations

        public virtual User? User { get; set; }

        #endregion

    }
}
