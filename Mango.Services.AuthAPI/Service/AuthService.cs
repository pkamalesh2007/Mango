using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext appDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if role does not exists
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower().Equals(loginRequestDto.UserName.ToLower()));
            bool isValid= await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if(user== null || isValid == false)
            {
                return new LoginResponseDto()
                {
                   User=null,
                   Token=""
                };


            }
            //if user was found generate the JWT token

            var roles = await _userManager.GetRolesAsync(user);

            var token=_jwtTokenGenerator.GenerateToken(user,roles);

            UserDto userDto = new()
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id
            };
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegisterationRequestDto registerationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDto.Email,
                Email = registerationRequestDto.Email,
                PhoneNumber = registerationRequestDto.PhoneNumber,
                NormalizedEmail = registerationRequestDto.Email.ToUpper(),
                Name=registerationRequestDto.Name

            };
            try
            {
                var result = await _userManager.CreateAsync(user,registerationRequestDto.Password);
                if(result.Succeeded)
                {
                    var userToReturn=_appDbContext.ApplicationUsers.First(u=>u.UserName == registerationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber,
                        Id = userToReturn.Id
                    };
                    return "";
                }

                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }


            }

            catch(Exception ex)
            {

            }
            return "Error Encountered";
        }
    }
}
