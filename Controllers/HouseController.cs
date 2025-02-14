using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HousingSManagement.Data;
using HousingSManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace HousingSManagement.Controllers
{
    [Authorize] // ✅ Ensure only authenticated users can access Houses
    public class HouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Publicly accessible to all authenticated users (View Only)
        public async Task<IActionResult> Index(string searchString, int? blockFilter)
        {
            var houses = _context.Houses.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                houses = houses.Where(h => h.OwnerName.Contains(searchString) ||
                                           h.HouseNumber.Contains(searchString));
            }

            if (blockFilter.HasValue)
            {
                houses = houses.Where(h => h.Block == blockFilter.Value.ToString());
            }

            ViewBag.SearchString = searchString;
            ViewBag.BlockFilter = blockFilter;

            return View(await houses.AsNoTracking().ToListAsync());
        }

        // ✅ Publicly accessible to view house details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
            if (house == null) return NotFound();

            return View(house);
        }

        // ✅ Only Admins Can Create Houses
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerName,HouseNumber,Block,PhoneNumber")] House house)
        {
            if (!ModelState.IsValid) return View(house);

            try
            {
                _context.Add(house);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🏡 House added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "❌ Error adding house.";
                return View(house);
            }
        }

        // ✅ Only Admins Can Edit Houses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses.FindAsync(id);
            if (house == null) return NotFound();

            return View(house);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,HouseNumber,Block,PhoneNumber")] House house)
        {
            if (id != house.Id) return NotFound();
            if (!ModelState.IsValid) return View(house);

            try
            {
                _context.Update(house);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🏡 House updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(house.Id)) return NotFound();
                throw;
            }
            catch
            {
                TempData["ErrorMessage"] = "❌ Error updating house.";
                return View(house);
            }
        }

        // ✅ Only Admins Can Delete Houses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
            if (house == null) return NotFound();

            return View(house);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                _context.Houses.Remove(house);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🏡 House deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ House not found!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(h => h.Id == id);
        }
    }
}
