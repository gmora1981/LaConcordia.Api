using Identity.Api.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Identity.Api.Reporteria
{
    // Replica "DETALLE DE FACTURA" (RptEmpresaVoucher) del sistema de escritorio: listado de
    // vouchers de una empresa agrupado por mes, con Punto Partida/Recorrido/Punto Final/
    // Empleado/Observación en vez del Conductor/Monto Pagado/Estado Proceso que se usa en
    // pantalla.
    public static class FacturacionPdfGenerator
    {
        private static readonly string[] MesesEs =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        public static byte[] GenerarPdf(List<OrdenPagoResumenDTO> vouchers, string ruc, string razonSocial, DateTime? hasta, string usuario)
        {
            var grupos = vouchers
                .OrderBy(v => v.Fechayhora)
                .GroupBy(v => v.Fechayhora.HasValue ? new { v.Fechayhora.Value.Year, v.Fechayhora.Value.Month } : new { Year = 0, Month = 0 })
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
                    page.Size(PageSizes.A4.Landscape());
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

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
                                    t.Span(usuario);
                                });
                            });
                        });

                        headerCol.Item().PaddingTop(8).AlignCenter().Text("DETALLE DE FACTURA").Bold().FontSize(14);

                        headerCol.Item().PaddingTop(6).Text(t =>
                        {
                            t.Span("HASTA   ").Italic().Bold();
                            t.Span(hasta.HasValue ? hasta.Value.ToString("d/M/yyyy") : "").Italic();
                        });

                        headerCol.Item().PaddingTop(2).Text(t =>
                        {
                            t.Span("RUC:   ").Bold();
                            t.Span(ruc + "   ");
                            t.Span("EMPRESA:   ").Bold();
                            t.Span(razonSocial);
                        });

                        headerCol.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        foreach (var grupo in grupos)
                        {
                            var nombreMes = grupo.Key.Month >= 1 && grupo.Key.Month <= 12
                                ? MesesEs[grupo.Key.Month - 1]
                                : "SIN FECHA";

                            col.Item().PaddingTop(6).Text(nombreMes).Bold().FontSize(10);

                            col.Item().PaddingTop(2).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);   // fecha
                                    columns.RelativeColumn(2);   // numvoucher
                                    columns.RelativeColumn(3);   // puntopartida
                                    columns.RelativeColumn(3);   // recorrido
                                    columns.RelativeColumn(3);   // puntofinal
                                    columns.RelativeColumn(3);   // empleado
                                    columns.RelativeColumn(2);   // observacion
                                    columns.RelativeColumn(1);   // unidad
                                    columns.RelativeColumn(1);   // precio
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("fecha").Bold();
                                    h.Cell().Text("numvoucher").Bold();
                                    h.Cell().Text("puntopartida").Bold();
                                    h.Cell().Text("recorrido").Bold();
                                    h.Cell().Text("puntofinal").Bold();
                                    h.Cell().Text("empleado").Bold();
                                    h.Cell().Text("observacion").Bold();
                                    h.Cell().Text("unidad").Bold();
                                    h.Cell().AlignRight().Text("precio").Bold();
                                });

                                foreach (var v in grupo)
                                {
                                    table.Cell().Text(v.Fechayhora?.ToString("dd/MM/yyyy"));
                                    table.Cell().Text(v.Numvoucher);
                                    table.Cell().Text(v.Puntopartida);
                                    table.Cell().Text(v.Recorrido);
                                    table.Cell().Text(v.Puntofinal);
                                    table.Cell().Text(v.Empleado);
                                    table.Cell().Text(v.Observacion);
                                    table.Cell().Text(v.Unidad);
                                    table.Cell().AlignRight().Text(v.Precio?.ToString("0.00"));
                                }
                            });
                        }

                        if (vouchers.Count == 0)
                        {
                            col.Item().PaddingTop(10).Text("No hay vouchers registrados para esta empresa en el rango seleccionado.")
                                .Italic().FontColor(Colors.Grey.Darken1);
                        }

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
                        col.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Total de Registros").Italic().Underline();
                                c.Item().Text(vouchers.Count.ToString());
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().AlignRight().Text("Total").Italic().Underline();
                                c.Item().AlignRight().Text(vouchers.Sum(x => x.Precio ?? 0).ToString("0.00"));
                            });
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
