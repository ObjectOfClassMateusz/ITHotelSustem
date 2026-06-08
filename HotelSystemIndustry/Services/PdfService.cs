using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.ViewModels;

namespace HotelSystemIndustry.Services
{
    public class PdfService
    {
        public byte[] GenerateInvoicePdf(Invoice invoice)
        {
            var res = invoice.Reservation;
            var room = res?.Room;
            decimal sum = 0;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("FAKTURA VAT")
                               .FontSize(22).Bold().FontColor("#b8924a");
                            col.Item().Text(invoice.InvoiceNumber)
                               .FontSize(14).Bold();
                        });
                        row.ConstantItem(160).Column(col =>
                        {
                            col.Item().AlignRight()
                               .Text($"Data wystawienia: {invoice.IssueDate:dd.MM.yyyy}")
                               .FontSize(9).FontColor("#666666");
                            col.Item().AlignRight()
                               .Text($"Hotel System Industry")
                               .FontSize(9).Bold();
                        });
                    });

                    // ── CONTENT ───────────────────────────────────────────
                    page.Content().PaddingTop(20).Column(col =>
                    {
                        // Divider
                        col.Item().PaddingBottom(16)
                           .LineHorizontal(1.5f).LineColor("#b8924a");

                        // Reservation details
                        if (res != null)
                        {
                            col.Item().PaddingBottom(12).Text("Szczegóły rezerwacji")
                               .FontSize(11).Bold();

                            col.Item().PaddingBottom(16).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(3);
                                });

                                void Row(string label, string value)
                                {
                                    table.Cell().Padding(5).Background("#f9f6f0")
                                         .Text(label).FontSize(9).FontColor("#666666");
                                    table.Cell().Padding(5)
                                         .Text(value).FontSize(9).Bold();
                                }

                                Row("NIP", res.NIP ?? "—");
                                Row("Pokój", room != null ? $"#{room.RoomNumber} — {room.RoomType}" : "—");
                                Row("Check-in", res.CheckInDate.ToString("dd.MM.yyyy"));
                                Row("Check-out", res.CheckOutDate.ToString("dd.MM.yyyy"));
                                Row("Liczba nocy", res.NumberOfOvernightStays.ToString());
                                Row("Status", res.Status.ToString());

                                if (!string.IsNullOrEmpty(res.SpecialWishes))
                                    Row("Życzenia", res.SpecialWishes);
                            });

                            // Guests
                            if (res.Guests?.Any() == true)
                            {
                                col.Item().PaddingBottom(8).Text("Goście")
                                   .FontSize(11).Bold();

                                foreach (var g in res.Guests)
                                {
                                    col.Item().PaddingLeft(8).PaddingBottom(3)
                                       .Text($"• {g.FirstName} {g.LastName}" +
                                             (!string.IsNullOrEmpty(g.Email) ? $"  ({g.Email})" : ""))
                                       .FontSize(9);
                                }

                                col.Item().PaddingBottom(16);
                            }
                        }

                        // ── AMOUNT TABLE ──────────────────────────────────
                        col.Item().PaddingBottom(6).Text("Podsumowanie płatności")
                           .FontSize(11).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(4);
                                c.RelativeColumn(1);
                                c.RelativeColumn(1);
                                c.RelativeColumn(1);
                            });
                            // Header row
                            void HeaderCell(string text) =>
                                table.Cell().Padding(7).Background("#1a1712")
                                     .Text(text).FontSize(9).Bold().FontColor("#d4a843");
                            HeaderCell("Opis");
                            HeaderCell("Cena/noc");
                            HeaderCell("Noce");
                            HeaderCell("Razem");
                            // Data row
                            decimal pricePerNight = room?.BasePricePerNight ?? 0;
                            int nights = res?.NumberOfOvernightStays ?? 0;

                            table.Cell().Padding(7).Background("#fdfbf7")
                                 .Text($"Pobyt — pokój #{room?.RoomNumber ?? "—"}").FontSize(9);
                            table.Cell().Padding(7).Background("#fdfbf7").AlignRight()
                                 .Text($"{pricePerNight:C}").FontSize(9);
                            table.Cell().Padding(7).Background("#fdfbf7").AlignCenter()
                                 .Text(nights.ToString()).FontSize(9);
                            table.Cell().Padding(7).Background("#fdfbf7").AlignRight()
                                 .Text($"{pricePerNight * nights:C}").FontSize(9).Bold();
                            invoice.TotalAmount = pricePerNight * nights;

                            // Total row
                            table.Cell().ColumnSpan(3).Padding(7)
                                 .AlignRight().Text("SUMA:").FontSize(10).Bold();
                            table.Cell().Padding(7).Background("#b8924a")
                                 .AlignRight()
                                 .Text($"{invoice.TotalAmount:C}")
                                 .FontSize(10).Bold().FontColor(Colors.White);
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Hotel System Industry  •  ").FontColor("#999999").FontSize(8);
                            x.Span(invoice.InvoiceNumber).FontColor("#b8924a").FontSize(8).Bold();
                            x.Span("  •  Strona ").FontColor("#999999").FontSize(8);
                            x.CurrentPageNumber().FontColor("#999999").FontSize(8);
                            x.Span(" z ").FontColor("#999999").FontSize(8);
                            x.TotalPages().FontColor("#999999").FontSize(8);
                        });
                });
            }).GeneratePdf();
        }

        public byte[] GenerateMonthlySummaryPdf(MonthlySummaryVM vm)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("SPRAWOZDANIE MIESIĘCZNE")
                                   .FontSize(18).Bold().FontColor("#b8924a");
                                c.Item().Text($"{vm.HotelName}")
                                   .FontSize(12).Bold();
                            });
                            row.ConstantItem(140).Column(c =>
                            {
                                c.Item().AlignRight()
                                   .Text($"{vm.MonthName} {vm.Year}")
                                   .FontSize(11).Bold();
                                c.Item().AlignRight()
                                   .Text($"Wygenerowano: {DateTime.Today:dd.MM.yyyy}")
                                   .FontSize(8).FontColor("#999999");
                            });
                        });
                        col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor("#b8924a");
                    });

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        // ── KPI TILES ─────────────────────────────────────
                        col.Item().PaddingBottom(16).Row(row =>
                        {
                            void Tile(string label, string value, string bg)
                            {
                                row.RelativeItem().Padding(4).Background(bg)
                                   .Padding(12).Column(c =>
                                   {
                                       c.Item().Text(value).FontSize(18).Bold().FontColor("#1a1712");
                                       c.Item().Text(label).FontSize(8).FontColor("#7a7060");
                                   });
                            }

                            Tile("Rezerwacje", vm.TotalReservations.ToString(), "#f0e4cc");
                            Tile("Goście", vm.TotalGuests.ToString(), "#ddeedd");
                            Tile("Przychód", $"{vm.TotalRevenue:C}", "#fce8d0");
                            Tile("Śr. / noc", $"{vm.AvgRevenuePerNight:C}", "#e8e4f0");
                        });

                        // ── RESERVATIONS TABLE ────────────────────────────
                        col.Item().PaddingBottom(8).Text("Szczegóły rezerwacji")
                           .FontSize(11).Bold();

                        if (vm.Reservations.Any())
                        {
                            col.Item().PaddingBottom(20).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(28);  // Nr
                                    c.RelativeColumn(2);   // Pokój
                                    c.RelativeColumn(2);   // Check-in
                                    c.RelativeColumn(2);   // Check-out
                                    c.ConstantColumn(32);  // Noce
                                    c.RelativeColumn(2);   // Kwota
                                    c.RelativeColumn(3);   // Goście
                                });

                                void Hdr(string t) =>
                                    table.Cell().Padding(5).Background("#1a1712")
                                         .Text(t).FontSize(8).Bold().FontColor("#d4a843");

                                Hdr("Nr"); Hdr("Pokój"); Hdr("Check-in");
                                Hdr("Check-out"); Hdr("Noce"); Hdr("Kwota"); Hdr("Goście");

                                int n = 1;
                                foreach (var r in vm.Reservations)
                                {
                                    string bg = n % 2 == 0 ? "#fdfbf7" : "#f5f2ec";

                                    table.Cell().Padding(4).Background(bg)
                                         .Text(n.ToString()).FontSize(8).FontColor("#999999");
                                    table.Cell().Padding(4).Background(bg)
                                         .Text($"#{r.RoomNumber}").FontSize(8).Bold();
                                    table.Cell().Padding(4).Background(bg)
                                         .Text(r.CheckIn.ToString("dd.MM.yy")).FontSize(8);
                                    table.Cell().Padding(4).Background(bg)
                                         .Text(r.CheckOut.ToString("dd.MM.yy")).FontSize(8);
                                    table.Cell().Padding(4).Background(bg).AlignCenter()
                                         .Text(r.Nights.ToString()).FontSize(8);
                                    table.Cell().Padding(4).Background(bg).AlignRight()
                                         .Text($"{r.Revenue:C}").FontSize(8).Bold();
                                    table.Cell().Padding(4).Background(bg)
                                         .Text(r.GuestNames).FontSize(7).FontColor("#555555");
                                    n++;
                                }
                            });
                        }
                        else
                        {
                            col.Item().PaddingBottom(16).Text("Brak rezerwacji w tym miesiącu.")
                               .FontSize(9).FontColor("#999999").Italic();
                        }

                        // ── SUMMARY LINE ──────────────────────────────────
                        col.Item().LineHorizontal(1).LineColor("#ddd6c4");
                        col.Item().PaddingTop(8).Row(row =>
                        {
                            row.RelativeItem().Text(
                                $"Łączna liczba nocy: {vm.TotalNights}  •  " +
                                $"Śr. długość pobytu: {vm.AvgStayLength:F1} nocy")
                               .FontSize(9).FontColor("#7a7060");
                            row.ConstantItem(160).AlignRight()
                               .Text($"PRZYCHÓD: {vm.TotalRevenue:C}")
                               .FontSize(11).Bold().FontColor("#b8924a");
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span($"{vm.HotelName}  •  Sprawozdanie {vm.MonthName} {vm.Year}")
                               .FontColor("#999999").FontSize(8);
                        });
                });
            }).GeneratePdf();
        }
    }
}