using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestCCController : BaseController<RequestCC>
    {
        private readonly IRequestCCService _requestCCService;

        public RequestCCController(IRequestCCService requestCCService) : base(requestCCService)
        {
            _requestCCService = requestCCService;
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<RequestCC>>> GetByRequestId(int requestId)
        {
            var requestCCs = await _requestCCService.GetByRequestIdAsync(requestId);
            return Ok(requestCCs);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RequestCC>>> GetByUserId(int userId)
        {
            var requestCCs = await _requestCCService.GetByUserIdAsync(userId);
            return Ok(requestCCs);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddCCUser([FromBody] AddCCUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _requestCCService.AddCCUserAsync(request.RequestId, request.UserId);
                return Ok(new { message = "Kullanıcı başarıyla CC listesine eklendi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveCCUser([FromBody] RemoveCCUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _requestCCService.RemoveCCUserAsync(request.RequestId, request.UserId);
                return Ok(new { message = "Kullanıcı CC listesinden çıkarıldı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateCCUsers([FromBody] UpdateCCUsersRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _requestCCService.UpdateCCUsersAsync(request.RequestId, request.UserIds);
                return Ok(new { message = "CC kullanıcıları başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class AddCCUserRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
    }

    public class RemoveCCUserRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateCCUsersRequest
    {
        public int RequestId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
