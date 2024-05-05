namespace Assessment_Backend.DataLayer.Entities.Subs
{
    [Table("SubType", Schema = "Subs")]

    public class SubType
    {
        [Key]
        public int SubTypeId { get; set; }

        public int Count { get; set; } 

        [Display(Name = "عنوان اشتراک ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Title { get; set; }
        [Display(Name = "شرح اشتراک ")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string? Detail { get; set; }
        [Display(Name = "قیمت اشتراک ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public int Price { get; set; }
        [Display(Name = "قیمت  تخفیفی اشتراک ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public int PriceDiscounted { get; set; }

        public bool IsFree { get; set; }
        public bool IsDelete  { get; set; }


        #region Relations

        public virtual List<Sub>? Subs { get; set; }

        #endregion
    }
}
