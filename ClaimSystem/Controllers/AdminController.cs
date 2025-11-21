using ClaimSystem.Data;
using ClaimSystem.Models;
using ClaimSystem.Services;
using ClaimSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClaimSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PdfReportService _pdfService;
        private readonly ClaimSystemContext _db;


        public AdminController(UserManager<ApplicationUser> um,
                       RoleManager<IdentityRole> rm,
                       ClaimSystemContext db, 
                       PdfReportService pdfService)
        {
            _userManager = um;
            _roleManager = rm;
            _pdfService = pdfService;
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            var list = new List<AdminUserDisplayVM>();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u); // <-- async, SAFE

                list.Add(new AdminUserDisplayVM
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Roles = roles
                });
            }

            return View(list);
        }


        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email, FullName = vm.FullName, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            if (!await _roleManager.RoleExistsAsync(vm.Role))
                await _roleManager.CreateAsync(new IdentityRole(vm.Role));

            await _userManager.AddToRoleAsync(user, vm.Role);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var vm = new EditUserVM { Id = user.Id, FullName = user.FullName, Email = user.Email };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();
            user.FullName = vm.FullName;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            var res = await _userManager.UpdateAsync(user);
            if (!res.Succeeded)
            {
                foreach (var e in res.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AllClaims()
        {
            var claims = await _db.Claims
                .Join(_userManager.Users,
                      c => c.LecturerId,
                      u => u.Id,
                      (c, u) => new AdminClaimVM
                      {
                          LecturerName = u.FullName,
                          LecturerEmail = u.Email,
                          ClaimId = c.ClaimId,
                          HoursWorked = c.HoursWorked,
                          TotalAmount = c.TotalAmount,
                          Status = c.Status,
                          SubmittedOn = c.SubmittedOn,
                          UploadedFileName = c.UploadedFileName
                      })
                .OrderBy(x => x.SubmittedOn)
                .ToListAsync();

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf(DateTime? from, DateTime? to, string status = null)
        {
            // fetch claims (include Lecturer nav)
            var q = _db.Claims.Include(c => c.Lecturer).AsQueryable();

            if (from.HasValue) q = q.Where(c => c.SubmittedOn >= from.Value);
            if (to.HasValue) q = q.Where(c => c.SubmittedOn <= to.Value);
            if (!string.IsNullOrEmpty(status)) q = q.Where(c => c.Status == status);

            var claims = await q.OrderBy(c => c.SubmittedOn).ToListAsync();

            var pdfBytes = _pdfService.GenerateClaimsReport(claims, DateTime.UtcNow, from, to);

            return File(pdfBytes, "application/pdf", $"ClaimsReport_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf");
        }


    }
}
