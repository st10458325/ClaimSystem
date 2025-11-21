using ClaimSystem.Models;

namespace ClaimSystem.Services
{
    public class ClaimWorkflowService
    {
        // Compute total amount
        public void CalculateTotals(ClaimRecord claim)
        {
            claim.TotalAmount = Math.Round(claim.HoursWorked * claim.HourlyRate, 2);
        }

        // Automated approval rules (simple)
        public string ApplyApprovalRules(ClaimRecord claim)
        {
            if (claim.HoursWorked <= 40 && claim.TotalAmount <= 5000m)
                return "Approved";

            if (claim.HoursWorked > 200 || claim.HourlyRate > 2000m)
                return "UnderReview";

            return "Pending";
        }

        public void ProcessNewClaim(ClaimRecord claim)
        {
            CalculateTotals(claim);
            claim.Status = ApplyApprovalRules(claim);
        }
    }
}
