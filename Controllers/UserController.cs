// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using backend.Db;
// using Microsoft.AspNetCore.Mvc;
// using Clerk.BackendAPI;
// using backend.Models;

// namespace backend.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class UserController : ControllerBase
//     {
//         private readonly AppDbContext _db;
//         private readonly ClerkBackendApi _clerk;

//         public UserController(AppDbContext db, IConfiguration config)
//         {
//             _db = db;
//             _clerk = new ClerkBackendApi(bearerAuth: config["Clerk:SecretKey"]);
//         }

//         [HttpPost("register")]
//         public async Task<IActionResult> RegisterUser([FromBody] dynamic payload)
//         {
//             string clerkUserId = payload.clerkUserId;
//             string email = payload.email;

//             var user = new User { ClerkUserId = clerkUserId, Email = email };
//             _db.Users.Add(user);
//             await _db.SaveChangesAsync();

//             return Ok(user);
//         }
//     }
// }