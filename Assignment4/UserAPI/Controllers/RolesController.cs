using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly userDBContext _context;

        public RolesController(userDBContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tuple<int,string>>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            List<Tuple<int, string>> userrole = new List<Tuple<int, string>>();

            foreach(var role in roles)
            {
                //Creating tuple list of the roles
                userrole.Add(Tuple.Create(role.RoleId, role.RoleName));
            }
            return userrole;
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tuple<int,string>>> GetRoles(int id)
        {
            var roles = await _context.Roles
                .Include(r => r.UserRole)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (roles == null)
            {
                return NotFound();
            }

            //Creating tuple of the role
            Tuple<int, string> role = Tuple.Create(roles.RoleId, roles.RoleName);
            

            return role;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoles(int id, Tuple<int,string> roles)
        {
            if (id != roles.Item1)
            {
                return BadRequest();
            }

            Roles updatedrole = new Roles();

            //Updated user data
            updatedrole.RoleId = roles.Item1;
            updatedrole.RoleName = roles.Item2;

            _context.Entry(updatedrole).State = EntityState.Modified;

            try
            {
                //Saving change to database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Tuple<int,string>>> PostRoles(Tuple<int,string> newrole)
        {
            //New Role
            Roles roles = new Roles();
            roles.RoleId = newrole.Item1;
            roles.RoleName = newrole.Item2;

            //Adding role and saving change to database
            _context.Roles.Add(roles);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoles", new { id = roles.RoleId }, roles);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tuple<int,string>>> DeleteRoles(int id)
        {
            var roles = await _context.Roles.FindAsync(id);
            if (roles == null)
            {
                return NotFound();
            }

            //Removing role and saving change to database
            _context.Roles.Remove(roles);
            await _context.SaveChangesAsync();

            //Creating tuple of deleted role
            var deletedrole = Tuple.Create(roles.RoleId, roles.RoleName);

            return deletedrole;
        }

        private bool RolesExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
