﻿namespace Assessment_Backend.Core.Servies.InterFace
{
    public interface IUserService
    {
        #region Role

        Task<OutPutModel<List<RoleDTO>>> GetAllRolesAsync();

        #endregion

        Task<OutPutModel<UserProfileDTO>> LoginAsync(LoginDTO model);


        Task<OutPutModel<bool>> RegisterTeacherAsync(RegisterTeacherDTO model);   

        Task<OutPutModel<bool>> RegisterStudentAsync(RegisterStudentDTO model);

        Task<bool> IsExistCodeMelliAsync(string code);

    }
}
