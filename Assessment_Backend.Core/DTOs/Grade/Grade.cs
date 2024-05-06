namespace Assessment_Backend.Core.DTOs.Grade
{
    public class GradeDTO
    {
        public int GradeId { get; set; }

        [Display(Name = "عنوان  ")]
        public string Title { get; set; }
    }
}
