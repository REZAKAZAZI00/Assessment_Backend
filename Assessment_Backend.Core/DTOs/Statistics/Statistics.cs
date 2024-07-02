
namespace Assessment_Backend.Core.DTOs.Statistics
{
    public class TeacherStatisticsDTO
    {

        public int ClassCount { get; set; }
        public int AssessmentCount { get; set; }
    } 
    public class StudentStatisticsDTO
    {

        public int ClassCount { get; set; }
        public int CountDutyDone { get; set; }
        public int CountDutyUncompleted { get; set; }
    }
}
