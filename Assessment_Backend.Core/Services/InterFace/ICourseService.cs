namespace Assessment_Backend.Core.Servies.InterFace
{
    public interface ICourseService
    {

        #region Term

        Task<OutPutModel<List<TermDTO>>> GetAllTermAsync();

        #endregion


        #region Course

        Task<OutPutModel<List<CourseDTO>>> CreateCourseAsync(CreateCourseDTO model);
        Task<OutPutModel<List<CourseDTO>>> DeleteCourseAsync(DeleteCourseDTO model);

        Task<OutPutModel<List<CourseDTO>>> UpdateCourseAsync(UpdateCourseDTO model);

        Task<List<CourseDTO>> GetCourseAsync();

        Task<OutPutModel<List<CourseDTO>>> GetAllCourseAsync();

        Task<OutPutModel<CourseDTO>> GetCourseByCourseIdAsync(int courseId);
        Task<OutPutModel<CourseDTO>> GetCourseByCourseLinkAsync(string link);

        Task<OutPutModel<List<CourseDTO>>> JoinClassAsync(JoinClassDTO model);
        Task<OutPutModel<List<CourseDTO>>> LeavingClassAsync(LeavingClassDTO model);

        #endregion
    }
}
