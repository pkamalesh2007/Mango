using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService)
        {
           _authService=authService;
            _responseDto = new();

        }
        [HttpPost("register")]
        public async Task<IActionResult>Register([FromBody]RegisterationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if(loginResponse.User==null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "username and password is incorrect";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDto model)
        {
            var assignRoleSuccessfully = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignRoleSuccessfully)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "error encountered";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
    }
}
