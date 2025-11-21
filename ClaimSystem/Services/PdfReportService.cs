using ClaimSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClaimSystem.Services
{
    public class PdfReportService
    {
        public byte[] GenerateClaimsReport(
            IEnumerable<ClaimRecord> claims,
            DateTime generatedOn,
            DateTime? from = null,
            DateTime? to = null)
        {
            var claimList = claims.ToList();
            var totalHours = claimList.Sum(c => c.HoursWorked);
            var totalAmount = claimList.Sum(c => c.TotalAmount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));
                    page.PageColor(Colors.White);

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Contract Monthly Claim System")
                                .FontSize(18).SemiBold();

                            col.Item().Text("Claims Report (PDF Export)")
                                .FontSize(13).SemiBold();

                            col.Item().Text($"Generated: {generatedOn:yyyy-MM-dd HH:mm}")
                                .FontSize(9).FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(140).AlignRight().Column(col =>
                        {
                            col.Item().Text("Rosebank College").FontSize(12).Bold();
                            col.Item().Text("HR / Admin Export").FontSize(10);
                        });
                    });

                    // CONTENT
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(8);

                        // Show date range info
                        string range = (from.HasValue || to.HasValue)
                            ? $"Range: {(from?.ToString("yyyy-MM-dd") ?? "—")} to {(to?.ToString("yyyy-MM-dd") ?? "—")}"
                            : "Range: All";

                        col.Item().Text(range).FontSize(10).FontColor(Colors.Grey.Darken2);

                        // TABLE
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);   // Lecturer
                                columns.RelativeColumn(2);   // Email
                                columns.RelativeColumn(1);   // Hours
                                columns.RelativeColumn(1);   // Rate
                                columns.RelativeColumn(1);   // Total
                                columns.RelativeColumn(1);   // Status
                                columns.RelativeColumn(1);   // Date
                            });

                            // HEADER
                            table.Header(h =>
                            {
                                h.Cell().Element(HeaderStyle).Text("Lecturer");
                                h.Cell().Element(HeaderStyle).Text("Email");
                                h.Cell().Element(HeaderStyle).AlignCenter().Text("Hours");
                                h.Cell().Element(HeaderStyle).AlignRight().Text("Rate");
                                h.Cell().Element(HeaderStyle).AlignRight().Text("Total");
                                h.Cell().Element(HeaderStyle).AlignCenter().Text("Status");
                                h.Cell().Element(HeaderStyle).AlignCenter().Text("Submitted");
                            });

                            // ROWS
                            foreach (var c in claimList)
                            {
                                table.Cell().Element(CellStyle).Text(c.Lecturer?.FullName ?? c.LecturerId);
                                table.Cell().Element(CellStyle).Text(c.Lecturer?.Email ?? "");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{c.HoursWorked:0.##}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{c.HourlyRate:0.00}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{c.TotalAmount:0.00}");
                                table.Cell().Element(CellStyle).AlignCenter().Text(c.Status);
                                table.Cell().Element(CellStyle).AlignCenter().Text(c.SubmittedOn.ToString("yyyy-MM-dd"));
                            }

                            static IContainer HeaderStyle(IContainer c) =>
                                c.PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Medium)
                                 .DefaultTextStyle(x => x.SemiBold())
                                 .PaddingLeft(4);

                            static IContainer CellStyle(IContainer c) =>
                                c.PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                 .PaddingLeft(4);
                        });

                        // TOTALS BLOCK
                        col.Item().PaddingTop(10).Column(cc =>
                        {
                            cc.Item().Row(r =>
                            {
                                r.RelativeItem().Text("");
                                r.ConstantItem(250).Row(rr =>
                                {
                                    rr.RelativeItem().Text("Total Hours:").SemiBold();
                                    rr.ConstantItem(120).AlignRight().Text($"{totalHours:0.##}");
                                });
                            });

                            cc.Item().Row(r =>
                            {
                                r.RelativeItem().Text("");
                                r.ConstantItem(250).Row(rr =>
                                {
                                    rr.RelativeItem().Text("Grand Total (ZAR):").Bold();
                                    rr.ConstantItem(120).AlignRight().Text($"{totalAmount:0.00}").Bold();
                                });
                            });
                        });

                        col.Item().PaddingTop(10)
                            .Text("This PDF was generated automatically for HR/admin processing.")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    // FOOTER
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" of ");
                        txt.TotalPages();
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
