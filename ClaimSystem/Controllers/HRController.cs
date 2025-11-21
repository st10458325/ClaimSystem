using ClaimSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClaimSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HRController : Controller
    {
        private readonly ClaimSystemContext _db;
        public HRController(ClaimSystemContext db) { _db = db; }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var q = _db.Claims.Include(c => c.Lecturer).Where(c => c.Status == "Approved");
            if (from.HasValue) q = q.Where(c => c.SubmittedOn >= from.Value);
            if (to.HasValue) q = q.Where(c => c.SubmittedOn <= to.Value);
            var approved = await q.OrderBy(c => c.LecturerId).ToListAsync();

            var summary = approved.GroupBy(c => c.Lecturer)
                .Select(g => new { Lecturer = g.Key, TotalAmount = g.Sum(x => x.TotalAmount), Claims = g.ToList() })
                .ToList();

            return View(summary);
        }

        public async Task<FileResult> ExportCsv(DateTime? from, DateTime? to)
        {
            var q = _db.Claims.Include(c => c.Lecturer).Where(c => c.Status == "Approved");
            if (from.HasValue) q = q.Where(c => c.SubmittedOn >= from.Value);
            if (to.HasValue) q = q.Where(c => c.SubmittedOn <= to.Value);

            var list = await q.OrderBy(c => c.LecturerId).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("LecturerId,LecturerName,Email,ClaimId,Hours,Rate,Total,SubmittedOn");
            foreach (var c in list)
            {
                sb.AppendLine($"{c.LecturerId},{Escape(c.Lecturer?.FullName)},{Escape(c.Lecturer?.Email)},{c.ClaimId},{c.HoursWorked},{c.HourlyRate},{c.TotalAmount},{c.SubmittedOn:yyyy-MM-dd}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"ApprovedClaims_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        private string Escape(string? s) => string.IsNullOrEmpty(s) ? "" : s.Replace(",", ";");
    }
}
