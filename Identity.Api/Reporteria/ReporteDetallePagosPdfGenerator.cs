using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Identity.Api.Reporteria
{
    // Replica "REPORTE DE DETALLE DE PAGOS POR MONITORIA" (FrmReporteUnidad -> "Detalle de
    // Pagos x Monitoria" / RptUnidadesDetallePagos) del sistema de escritorio: pagos de cuota
    // semanal del socio de una unidad, agrupados por semana, con subtotal por grupo.
    public static class ReporteDetallePagosPdfGenerator
    {
        public static byte[] GenerarPdf(List<DetallePagoMonitoriaDTO> pagos, string unidad, string? conductor, DateTime desde, DateTime hasta, string usuarioLogueado)
        {
            var grupos = pagos
                .OrderBy(p => p.Semana).ThenBy(p => p.Fechapago)
                .GroupBy(p => p.Semana)
                .ToList();

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

                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
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
                                    t.Span(usuarioLogueado);
                                });
                            });
                        });

                        headerCol.Item().PaddingTop(8).AlignCenter().Text("REPORTE DE DETALLE DE PAGOS POR MONITORIA").Bold().FontSize(14);

                        headerCol.Item().PaddingTop(8).Row(row =>
                        {
                            row.AutoItem().Text(t =>
                            {
                                t.Span("DESDE   ").Italic().Bold();
                                t.Span(desde.ToString("d/M/yyyy")).Italic();
                            });
                            row.RelativeItem();
                            row.AutoItem().Text(t =>
                            {
                                t.Span("HASTA   ").Italic().Bold();
                                t.Span(hasta.ToString("d/M/yyyy")).Italic();
                            });
                        });

                        headerCol.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("UNIDAD").Italic().Bold().Underline();
                                c.Item().Text(unidad);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("CONDUCTOR").Italic().Bold().Underline();
                                c.Item().Text(conductor ?? "");
                            });
                        });

                        headerCol.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        foreach (var grupo in grupos)
                        {
                            col.Item().PaddingTop(6).Text($"SEMANA : {grupo.Key}").Bold().FontSize(10);

                            col.Item().PaddingTop(2).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // periodo
                                    columns.RelativeColumn(2); // fecha pago
                                    columns.RelativeColumn(1); // valor pagado
                                    columns.RelativeColumn(2); // forma de pago
                                    columns.RelativeColumn(1); // detalle
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("PERIODO").Bold().Italic().Underline();
                                    h.Cell().Text("FECHA PAGO").Bold().Italic().Underline();
                                    h.Cell().AlignRight().Text("VALOR PAGADO").Bold().Italic().Underline();
                                    h.Cell().Text("FORMA DE PAGO").Bold().Italic().Underline();
                                    h.Cell().Text("DETALLE").Bold().Italic().Underline();
                                });

                                foreach (var p in grupo)
                                {
                                    table.Cell().Text(p.Periodo);
                                    table.Cell().Text(p.Fechapago.ToString("d/M/yyyy HH:mm:ss"));
                                    table.Cell().AlignRight().Text(p.Valorpagado?.ToString("0.00"));
                                    table.Cell().Text(p.Formadepago);
                                    table.Cell().Text(p.Detalle);
                                }
                            });

                            col.Item().PaddingTop(2).Row(row =>
                            {
                                row.RelativeItem();
                                row.AutoItem().Text(t =>
                                {
                                    t.Span("SUB TOTAL   ").Bold();
                                    t.Span(grupo.Sum(x => x.Valorpagado ?? 0).ToString("0.00"));
                                });
                            });
                        }

                        if (pagos.Count == 0)
                        {
                            col.Item().PaddingTop(10).Text("No hay pagos registrados para la unidad y el rango seleccionados.")
                                .Italic().FontColor(Colors.Grey.Darken1);
                        }

                        col.Item().PaddingTop(14).Text(t =>
                        {
                            t.Span("TOTAL DE CUENTAS PAGADAS   ").Bold().Italic();
                            t.Span(pagos.Sum(x => x.Valorpagado ?? 0).ToString("0.00")).Bold();
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
