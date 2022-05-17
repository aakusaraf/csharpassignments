using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserAPIClient.Models;

namespace UserAPIClient.Controllers
{
    public class UsersController : Controller
    {
        private readonly userDBContext _context;
        HttpClient client = new HttpClient();
        string url = "https://localhost:44319/api/Users/";

        public UsersController(userDBContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = JsonConvert
                .DeserializeObject
                <List<Tuple<int, string, string, string, long?, List<string>>>>
                (await client.GetStringAsync(url));
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = JsonConvert
                .DeserializeObject
                <Tuple<int,string, string, string, long?, List<string>>>
                (await client.GetStringAsync(url+id.ToString()));
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["role"] = new SelectList(_context.Roles, "RoleName", "RoleName");
            return View();          
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,FirstName,LastName,Email,Phone,Role")] Users users)
        {
            if (ModelState.IsValid)
            {
                Tuple<string, string, string, long?, string> newuser
                    = Tuple.Create(users.UserName,
                    users.FirstName + " " + users.LastName,
                    users.Email,
                    users.Phone,
                    users.Role);
                
                await client.PostAsJsonAsync
                    <Tuple<string, string, string, long?, string>>
                    (url, newuser);
                return RedirectToAction(nameof(Index));
            }
            ViewData["role"] = new SelectList(_context.Roles, "RoleName", "RoleName",users.Role);
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usertuple = JsonConvert
                .DeserializeObject
                <Tuple<int, string, string, string, long?, List<string>>>
                (await client.GetStringAsync(url + id.ToString()));

            var spaceindex = usertuple.Item3.IndexOf(" ");

            var users = new Users
            {
                UserId = usertuple.Item1,
                UserName = usertuple.Item2,
                FirstName = usertuple.Item3.Substring(0, spaceindex),
                LastName = usertuple.Item3.Substring(spaceindex+1),
                Email = usertuple.Item4,
                Phone = usertuple.Item5,
                Role = usertuple.Item6.First()
            };
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FirstName,LastName,Email,Phone")] Users users)
        {
            if (id != users.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Tuple<int, string, string, string, long?> updateduser
                        = Tuple.Create(users.UserId, users.UserName,
                        users.FirstName + " " + users.LastName,
                        users.Email,
                        users.Phone);

                    await client.PutAsJsonAsync(url+id.ToString(), updateduser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = JsonConvert
                .DeserializeObject
                <Tuple<int, string, string, string, long?, List<string>>>
                (await client.GetStringAsync(url + id.ToString()));
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await client.DeleteAsync(url + id.ToString());
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
