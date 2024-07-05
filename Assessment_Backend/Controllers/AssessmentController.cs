﻿using System.ComponentModel.DataAnnotations;

namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        #region Constructor
        private readonly IAssessmentService _assessmentService;
        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }
        #endregion


        [HttpPost("CreateAssessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> CreateAssessment(CreateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.CreateAssessmentAsync(assessmentDTO);
            return result;
        }

        [HttpPost("testforFile")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> test([FromForm ] TestCreateAssessmentDTO assessmentDTO, IFormFile file)
        {
            if (file!=null)
            {
                return Ok("file upload shod");
            }
            //var result = await _assessmentService.CreateAssessmentAsync(assessmentDTO);
            return BadRequest();
        }

        [HttpPut("UpdateAssessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> UpdateAssessment(UpdateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.UpdateAssessmentAsync(assessmentDTO);
            return result;
        }

        [HttpDelete("DeleteAssessment")]
        public async Task<ActionResult<OutPutModel<CourseDTO>>> DeleteAssessment(DeleteAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.DeleteAssessmentAsync(assessmentDTO);

            return result;
        }

        [HttpPost("AssignmentSubmission")]
        public async Task<ActionResult<OutPutModel<AssessmentDTO>>> AssignmentSubmission(AssignmentSubmissionDTO submissionDTO)
        {
            var result = await _assessmentService.AssignmentSubmissionAsync(submissionDTO);
            return result;
        }

        [HttpPost("ScoreRegistration")]
        public async Task<ActionResult<OutPutModel<AssessmentDTO>>> ScoreRegistration(ScoreRegistrationDTO scoreRegistrationDTO)
        {
            var result = await _assessmentService.ScoreRegistrationAsync(scoreRegistrationDTO);
            return result;
        }

        [HttpGet("GetAssignments/{assignmentId}")]
        public async Task<ActionResult<OutPutModel<List<SubmittedAssignmentDTO>>>> GetAssignments(int assignmentId)
        {
            var result = await _assessmentService.GetAssignmentSubmissionsByIdAsync(assignmentId);
            return result;
        }


        [HttpGet("GetAllAssignments")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<AssessmentDTO>>>> GetAssignments()
        {
            var result = await _assessmentService.GetAllAssignmentAsync();
            return result;
        }

    }
}
