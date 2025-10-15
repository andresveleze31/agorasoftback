using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Operations;


namespace backend.Services
{
    public class ClerkService
    {
        private readonly ClerkBackendApi _clerk;

        public ClerkService(IConfiguration config)
        {
            var secret = config["Clerk:SecretKey"];
            _clerk = new ClerkBackendApi(bearerAuth: secret);
        }

        public async Task<Clerk.BackendAPI.Models.Components.Organization> CreateOrganizationAsync(string name)
        {
            // ✅ SOLO envía el nombre, sin slug
            var request = new CreateOrganizationRequestBody
            {
                Name = name
                // 👇 REMUEVE completamente la línea del Slug
                // Slug = name.ToLower().Replace(" ", "-") 
            };

            var response = await _clerk.Organizations.CreateAsync(request);

            return response.Organization;
        }
    }
}