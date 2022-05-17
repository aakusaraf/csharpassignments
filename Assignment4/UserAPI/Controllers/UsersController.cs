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
    public class UsersController : ControllerBase
    {
        private readonly userDBContext _context;

        public UsersController(userDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tuple<int,string,string,string,long?,List<string>>>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRole)
                .ToListAsync();

            List<Tuple<int,string, string, string, long?, List<string>>> allusers
                = new List<Tuple<int,string, string, string, long?, List<string>>>();

            foreach(var user in users)
            {
                //Finding the role of the user
                var roles = await _context.UserRole
                    .Include(r => r.Role)
                    .Where(u => u.UserId == user.UserId)
                    .Select(r => r.Role.RoleName)
                    .ToListAsync();

                //Creating tuple list of users
                allusers.Add(
                    Tuple.Create(user.UserId,
                        user.UserName,
                    user.FirstName + " " + user.LastName, 
                    user.Email, user.Phone, 
                    roles));
            }
            return allusers;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tuple<int,string, string, string, long?, List<string>>>> GetUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);

            //Finding role of the user
            var role = await _context.UserRole
                .Include(r => r.Role)
                .Where(u => u.UserId == users.UserId)
                .Select(r => r.Role.RoleName)
                .ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            //Creating tuple of the user
            Tuple<int,string, string, string, long?, List<string>> user
                = Tuple.Create(users.UserId,users.UserName, users.FirstName + " " + users.LastName,
                users.Email, users.Phone,role);

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Tuple<int,string, string, string, long?> users)
        {
            if (id != users.Item1)
            {
                return BadRequest();
            }

            int spaceindex = users.Item3.IndexOf(" ",0);
            int length = users.Item3.Count();


            

            
            //Updated data of the user
            Users updateduser = new Users();
            updateduser.UserId = users.Item1;
            updateduser.UserName = users.Item2;
            updateduser.FirstName = users.Item3.Substring(0,spaceindex);
            updateduser.LastName = users.Item3.Substring(spaceindex+1);
            updateduser.Email = users.Item4;
            updateduser.Phone = users.Item5;

            _context.Entry(updateduser).State = EntityState.Modified;

            try
            {
                //Saving changes into database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Tuple<string, string, string, long?, string>>> PostUsers(Tuple<string,string,string,long?,string> newuser)
        {
            //Finding the role for the user
            var role = _context.Roles.Where(r => r.RoleName == newuser.Item5)
                .FirstOrDefault();

            //Finding space in the name to separate firstname and lastname
            var spaceindex = newuser.Item2.IndexOf(" ");

            Users users = new Users();

            users.UserName = newuser.Item1;
            users.FirstName = newuser.Item2.Substring(0, spaceindex);
            users.LastName = newuser.Item2.Substring(spaceindex + 1);
            users.Email = newuser.Item3;
            users.Phone = newuser.Item4;

            UserRole userrole = new UserRole();
            userrole.UserId = _context.Users.AsEnumerable().Last().UserId + 1;
            userrole.RoleId = role.RoleId;

            //Assigning role to the user
            users.UserRole.Add(userrole);

            //Adding user and saving change to database
            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.UserId }, newuser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tuple<string, string, string, long?, List<string>>>> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);

            //Finding role of the user
            var role = _context.UserRole.Include(r => r.Role)
                .Where(u => u.UserId == id)
                .Select(r => r.Role.RoleName)
                .ToList();

            if (users == null)
            {
                return NotFound();
            }

            //Creating tuple of the deleted user
            var deleteduser = Tuple.Create(users.UserName,
                users.FirstName+ " "+users.LastName,
                users.Email,
                users.Phone,
                role);

            //Deleting user and saving change to database
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return deleteduser;
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
