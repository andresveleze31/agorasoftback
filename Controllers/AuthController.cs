using System;
using System.Threading.Tasks;
using backend.Db;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly OrganizationService _orgService;

        public AuthController(AppDbContext db, OrganizationService orgService)
        {
            _db = db;
            _orgService = orgService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // ✅ Verificar si el usuario ya existe
            var existingUser = _db.Users.FirstOrDefault(u => u.ClerkUserId == request.ClerkUserId);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El usuario ya está registrado." });
            }

            // ✅ Crear usuario en DB
            var user = new User
            {
                ClerkUserId = request.ClerkUserId,
                Email = request.Email
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // ✅ Guardar la organización usando el ID de Clerk que viene desde el front
            await _orgService.AddOrganizationAsync(request.ClerkOrgId, request.OrganizationName, user.Id);

            return Ok(new
            {
                message = "Usuario y organización registrados correctamente",
                organizationId = request.ClerkOrgId
            });
        }
    }

    public class RegisterRequest
    {
        public string ClerkUserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string ClerkOrgId { get; set; } = string.Empty; // 👈 Se pasa desde el front
    }
}
