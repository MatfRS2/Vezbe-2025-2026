using AutoMapper;
using IdentityServer.Controllers.Base;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticationController : RegistrationControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IAuthenticationService authService) 
        : base(logger, mapper, userManager, roleManager)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterBuyer([FromBody] NewUserDto newUser)
    {
        return await RegisterNewUserWithRoles(newUser, [Roles.Buyer]);
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAdministrator([FromBody] NewUserDto newUser)
    {
        return await RegisterNewUserWithRoles(newUser, [Roles.Admin]);
    }
}