using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    public static class FacturacionPdfGenerator
    {
        public static byte[] GenerarPdf(List<OrdenPagoResumenDTO> vouchers, string razonSocial, DateTime? hasta, string usuario)
        {
            var doc = Document.Create(container =>
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "favicon.jpeg");
                byte[]? logoImage = null;
                if (File.Exists(logoPath))
                {
                    logoImage = File.ReadAllBytes(logoPath);
                }

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(70).AlignMiddle().Column(col =>
                        {
                            if (logoImage != null)
                                col.Item().Image(logoImage);
                        });

                        row.RelativeColumn().AlignMiddle().PaddingLeft(10).Column(col =>
                        {
                            col.Item().AlignCenter().Text("SERVICIO DE TRANSPORTE “LA CONCORDIA”")
                                .SemiBold().FontSize(14).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Servicio de Transporte Exclusivo Puerta a Puerta")
                                .SemiBold().FontSize(12).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Tlf: 2606425 Claro: 0994227299 Movistar: 0987117307")
                                .SemiBold().FontSize(10).FontColor(Colors.Black);
                        });

                        row.ConstantColumn(160).Border(1).BorderColor(Colors.Black).Padding(8).Column(col =>
                        {
                            col.Item().Text(t =>
                            {
                                t.Span("Fecha Emisión: ").Bold();
                                t.Span(DateTime.Now.ToString("d/M/yyyy"));
                            });
                            col.Item().Text(t =>
                            {
                                t.Span("Hora Emisión: ").Bold();
                                t.Span(DateTime.Now.ToString("HH:mm:ss"));
                            });
                            col.Item().Text(t =>
                            {
                                t.Span("Usuario : ").Bold();
                                t.Span(usuario);
                            });
                        });
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Item().AlignCenter().Text("FACTURACIÓN POR EMPRESA").Bold().FontSize(16);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.AutoItem().Text(t =>
                            {
                                t.Span("EMPRESA   ").Bold();
                                t.Span(razonSocial);
                            });
                            row.RelativeItem();
                            row.AutoItem().Text(t =>
                            {
                                t.Span("HASTA   ").Bold();
                                t.Span(hasta.HasValue ? hasta.Value.ToString("d/M/yyyy") : "Todas las fechas");
                            });
                        });

                        col.Item().PaddingTop(5).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Black);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // numvoucher
                                columns.RelativeColumn(2); // fecha
                                columns.RelativeColumn(1); // unidad
                                columns.RelativeColumn(3); // conductor
                                columns.RelativeColumn(1); // precio
                                columns.RelativeColumn(1); // monto pagado
                                columns.RelativeColumn(2); // estado proceso
                            });

                            table.Header(h =>
                            {
                                h.Cell().Text("N° Voucher").Bold();
                                h.Cell().Text("Fecha").Bold();
                                h.Cell().Text("Unidad").Bold();
                                h.Cell().Text("Conductor").Bold();
                                h.Cell().AlignRight().Text("Precio").Bold();
                                h.Cell().AlignRight().Text("Monto Pagado").Bold();
                                h.Cell().Text("Estado Proceso").Bold();
                            });

                            foreach (var v in vouchers)
                            {
                                table.Cell().Text(v.Numvoucher);
                                table.Cell().Text(v.Fechayhora?.ToString("dd/MM/yyyy HH:mm"));
                                table.Cell().Text(v.Unidad);
                                table.Cell().Text(v.Conductor);
                                table.Cell().AlignRight().Text(v.Precio?.ToString("0.00"));
                                table.Cell().AlignRight().Text(v.Preciodesc?.ToString("0.00"));
                                table.Cell().Text(v.Estadoproceso);
                            }

                            if (vouchers.Count == 0)
                            {
                                table.Cell().ColumnSpan(7).PaddingTop(10).Text("No hay vouchers registrados para esta empresa en el rango seleccionado.")
                                    .Italic().FontColor(Colors.Grey.Darken1);
                            }
                        });

                        col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Black);
                        col.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem(6).AlignRight().Text($"Total ({vouchers.Count} vouchers)").Bold();
                            row.RelativeItem(1).AlignRight().Text(vouchers.Sum(x => x.Precio ?? 0).ToString("0.00")).Bold();
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(6).AlignRight().Text("Total Monto Pagado").Bold();
                            row.RelativeItem(1).AlignRight().Text(vouchers.Sum(x => x.Preciodesc ?? 0).ToString("0.00")).Bold();
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}
