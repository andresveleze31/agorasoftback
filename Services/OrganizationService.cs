using backend.Db;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class OrganizationService
    {
        private readonly AppDbContext _db;

        public OrganizationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Organization> AddOrganizationAsync(string clerkOrgId, string name, int ownerId)
        {
            var org = new Organization
            {
                ClerkOrgId = clerkOrgId, // 👈 Corregido: ClerkOrgId en lugar de ClerkOrganizationId
                Name = name,
                OwnerId = ownerId, // 👈 Corregido: OwnerId en lugar de UserId
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Organizations.Add(org);
            await _db.SaveChangesAsync();

            return org;
        }

        public async Task<bool> ActivateOrganizationAsync(string clerkOrgId)
        {
            var org = await _db.Organizations
                .FirstOrDefaultAsync(o => o.ClerkOrgId == clerkOrgId); // 👈 Corregido: ClerkOrgId

            if (org != null)
            {
                org.IsActive = true;
                org.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeactivateOrganizationAsync(string clerkOrgId)
        {
            var org = await _db.Organizations
                .FirstOrDefaultAsync(o => o.ClerkOrgId == clerkOrgId);

            if (org != null)
            {
                org.IsActive = false;
                org.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Organization?> GetOrganizationByClerkIdAsync(string clerkOrgId)
        {
            return await _db.Organizations
                .Include(o => o.Owner) // 👈 Incluir datos del dueño si es necesario
                .FirstOrDefaultAsync(o => o.ClerkOrgId == clerkOrgId);
        }

        public async Task<List<Organization>> GetOrganizations()
        {
            return await _db.Organizations
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }


        public async Task<List<Organization>> GetUserOrganizationsAsync(int ownerId)
        {
            return await _db.Organizations
                .Where(o => o.OwnerId == ownerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> OrganizationExistsAsync(string clerkOrgId)
        {
            return await _db.Organizations
                .AnyAsync(o => o.ClerkOrgId == clerkOrgId);
        }
    }
}