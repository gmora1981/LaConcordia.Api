using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Identity.Api.Reporteria
{
    public static class ImagenesChoferPdfGenerator
    {
        public static byte[] GenerarPdf(string frontal, string trasera, string matricula, string licencia, string vehiculo)
        {
            var doc = Document.Create(container =>
            {
                // ruta del logo 
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "favicon.jpeg");
                byte[] logoImage = null;
                if (File.Exists(logoPath))
                {
                    logoImage = File.ReadAllBytes(logoPath);
                }

                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                    // ===== CABECERA =====
                    page.Header().Height(80).Row(row =>
                    {
                        // Columna izquierda (logo)
                        row.RelativeColumn(2).AlignMiddle().AlignLeft().Column(col =>
                        {
                            if (logoImage != null)
                                col.Item().Image(logoImage).FitWidth();
                        });

                        // Columna central (título)
                        row.RelativeColumn(8).Column(col =>
                        {
                            col.Item().AlignCenter().Text("SERVICIO DE TRANSPORTE “LA CONCORDIA”")
                                .SemiBold().FontSize(14).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Servicio de Transporte Exclusivo Puerta a Puerta")
                                .SemiBold().FontSize(12).FontColor(Colors.Grey.Darken2);

                            col.Item().AlignCenter().Text("Tlf: 2606425 Claro: 0994227299 Movistar: 0987117307")
                                .SemiBold().FontSize(10).FontColor(Colors.Grey.Darken2);
                        });

                        // Columna derecha (fecha y hora)
                        row.RelativeColumn(2).Column(col =>
                        {
                            col.Item().AlignRight().Text($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(8).FontColor(Colors.Grey.Darken1).Bold();

                            col.Item().AlignRight().Text($"Hora Emisión: {DateTime.Now:HH:mm:ss}")
                                .FontSize(8).FontColor(Colors.Grey.Darken1).Bold();
                        });
                    });

                    // ===== CONTENIDO =====
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Bloque helper
                        void SeccionConBorde(string titulo, Action<IContainer> contenido)
                        {
                            col.Item().PaddingBottom(15).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(c =>
                            {
                                c.Item().Background(Colors.Grey.Lighten3).Padding(5).Text(titulo)
                                    .SemiBold().FontSize(12).FontColor(Colors.Black);

                                c.Item().PaddingTop(5).Element(contenido);
                            });
                        }

                        // Documento de Identidad
                        SeccionConBorde("Documento de Identidad", e =>
                        {
                            e.Row(row =>
                            {
                                row.RelativeColumn().Element(cell =>
                                {
                                    if (!string.IsNullOrEmpty(frontal))
                                        cell.Image(Convert.FromBase64String(frontal)).FitArea();
                                    else
                                        cell.Text("Frontal no encontrado").FontColor(Colors.Red.Medium).Bold();
                                });

                                row.RelativeColumn().Element(cell =>
                                {
                                    if (!string.IsNullOrEmpty(trasera))
                                        cell.Image(Convert.FromBase64String(trasera)).FitArea();
                                    else
                                        cell.Text("Trasera no encontrada").FontColor(Colors.Red.Medium).Bold();
                                });
                            });
                        });

                        // Matrícula
                        SeccionConBorde("Matrícula", e =>
                        {
                            if (!string.IsNullOrEmpty(matricula))
                                e.Image(Convert.FromBase64String(matricula)).FitArea();
                            else
                                e.Text("Matrícula no encontrada").FontColor(Colors.Red.Medium).Bold();
                        });

                        // Licencia
                        SeccionConBorde("Licencia", e =>
                        {
                            if (!string.IsNullOrEmpty(licencia))
                                e.Image(Convert.FromBase64String(licencia)).FitArea();
                            else
                                e.Text("Licencia no encontrada").FontColor(Colors.Red.Medium).Bold();
                        });

                        // Vehículo
                        SeccionConBorde("Vehículo", e =>
                        {
                            if (!string.IsNullOrEmpty(vehiculo))
                                e.Image(Convert.FromBase64String(vehiculo)).FitArea();
                            else
                                e.Text("Vehículo no encontrado").FontColor(Colors.Red.Medium).Bold();
                        });
                    });

                    // ===== FOOTER =====
                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Documento generado automáticamente - ").FontSize(8).FontColor(Colors.Grey.Darken1);
                        txt.Span("SERVICIO DE TRANSPORTE “LA CONCORDIA”").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}
