using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organizationService;

        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// ✅ Obtiene una organización por su ClerkOrgId
        /// Ejemplo: GET /api/organization/org_2x1aBcdXYZ
        /// </summary>
        [HttpGet("{clerkOrgId}")]
        public async Task<IActionResult> GetOrganizationByClerkId(string clerkOrgId)
        {
            var org = await _organizationService.GetOrganizationByClerkIdAsync(clerkOrgId);

            if (org == null)
                return NotFound(new { message = "Organización no encontrada." });

            return Ok(org);
        }

        /// <summary>
        /// ✅ Obtiene todas las organizaciones
        /// Ejemplo: GET /api/organization
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var orgs = await _organizationService.GetOrganizations(); 
            // 🔹 Si quieres obtener todas, mejor hacemos un método nuevo en el servicio:
            // return Ok(await _organizationService.GetAllOrganizationsAsync());

            return Ok(orgs);
        }
    }
}
