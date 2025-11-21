using ClaimSystem.Data;
using ClaimSystem.Models;
using ClaimSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ClaimSystemContext _db;
        private readonly ClaimWorkflowService _workflow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ClaimsController(ClaimSystemContext db, ClaimWorkflowService workflow, UserManager<ApplicationUser> um, IWebHostEnvironment env)
        {
            _db = db; _workflow = workflow; _userManager = um; _env = env;
        }

        // Lecturer - submit
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create() => View(new ClaimRecord());

        [HttpPost, Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create(ClaimRecord vm, IFormFile? upload)
        {
            var lecturerId = _userManager.GetUserId(User);
            vm.LecturerId = lecturerId!;

            _workflow.ProcessNewClaim(vm);

            if (upload != null && upload.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(upload.FileName)}";
                var path = Path.Combine(uploads, fileName);
                using var fs = new FileStream(path, FileMode.Create);
                await upload.CopyToAsync(fs);
                vm.UploadedFileName = fileName;
            }

            if (upload != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var ext = Path.GetExtension(upload.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Only PDF or Word documents are allowed.");
                    return View(vm); // return form with error
                }
            }

            _db.Claims.Add(vm);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(MyClaims));
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> MyClaims()
        {
            var lecturerId = _userManager.GetUserId(User);
            var claims = await _db.Claims.Where(c => c.LecturerId == lecturerId).OrderByDescending(c => c.SubmittedOn).ToListAsync();
            return View(claims);
        }

        // Coordinator view
        [Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> CoordinatorIndex()
        {
            var list = await _db.Claims.Include(c => c.Lecturer).Where(c => c.Status == "Pending" || c.Status == "UnderReview").OrderBy(c => c.SubmittedOn).ToListAsync();
            return View(list);
        }

        [HttpPost, Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = "Approved";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(CoordinatorIndex));
        }

        [HttpPost, Authorize(Roles = "Coordinator,Admin")]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = "Rejected";
            claim.Notes = (claim.Notes ?? "") + $"\n[Rejected: {reason}]";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(CoordinatorIndex));
        }

        public async Task<IActionResult> Download(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null || claim.UploadedFileName == null)
                return NotFound();

            var path = Path.Combine(_env.WebRootPath, "uploads", claim.UploadedFileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(bytes, "application/octet-stream", claim.UploadedFileName);
        }

    }
}
