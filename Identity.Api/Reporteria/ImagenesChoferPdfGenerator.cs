using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ===== CABECERA =====
                    page.Header().Height(80).Row(row =>
                    {
                        // Columna izquierda (logo)
                        row.RelativeColumn(2).AlignMiddle().AlignLeft().Column(col =>
                        {
                            if (logoImage != null)
                                col.Item().Image(logoImage);
                        });

                        // Columna central (título)
                        row.RelativeColumn(8).Column(col =>
                        {
                            col.Item().AlignCenter().Text("SERVICIO DE TRANSPORTE “LA CONCORDIA”")
                                .SemiBold().FontSize(14).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Servicio de Transporte Exclusivo Puerta a Puerta")
                                .SemiBold().FontSize(12).FontColor(Colors.Black);

                            col.Item().AlignCenter().Text("Tlf: 2606425 Claro: 0994227299 Movistar: 0987117307")
                                .SemiBold().FontSize(10).FontColor(Colors.Black);
                        });

                        // Columna derecha (fecha y hora)
                        row.RelativeColumn(2).Column(col =>
                        {
                            col.Item().AlignRight().Text($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();

                            col.Item().AlignRight().Text($"Hora Emisión: {DateTime.Now:HH:mm:ss}")
                                .FontSize(7).FontColor(Colors.Grey.Darken1).Bold();
                        });
                    });

                    // ===== CONTENIDO =====
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // DOCUMENTOS (Frontal y Trasera en fila)
                        col.Item().Text("Documento de Identidad").Bold().FontSize(12).Underline();
                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Element(e =>
                            {
                                if (!string.IsNullOrEmpty(frontal))
                                    e.Image(Convert.FromBase64String(frontal)).FitArea();
                                else
                                    e.Text("Frontal no encontrado").FontColor(Colors.Red.Medium);
                            });

                            row.RelativeColumn().Element(e =>
                            {
                                if (!string.IsNullOrEmpty(trasera))
                                    e.Image(Convert.FromBase64String(trasera)).FitArea();
                                else
                                    e.Text("Trasera no encontrada").FontColor(Colors.Red.Medium);
                            });
                        });

                        col.Item().PaddingVertical(10);

                        // MATRICULA
                        col.Item().Text("Matrícula").Bold().FontSize(12).Underline();
                        col.Item().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(matricula))
                                e.Image(Convert.FromBase64String(matricula)).FitArea();
                            else
                                e.Text("Matrícula no encontrada").FontColor(Colors.Red.Medium);
                        });

                        col.Item().PaddingVertical(10);

                        // LICENCIA
                        col.Item().Text("Licencia").Bold().FontSize(12).Underline();
                        col.Item().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(licencia))
                                e.Image(Convert.FromBase64String(licencia)).FitArea();
                            else
                                e.Text("Licencia no encontrada").FontColor(Colors.Red.Medium);
                        });

                        col.Item().PaddingVertical(10);

                        // VEHICULO (ocupa casi toda la hoja)
                        col.Item().Text("Vehículo").Bold().FontSize(12).Underline();
                        col.Item().Element(e =>
                        {
                            if (!string.IsNullOrEmpty(vehiculo))
                                e.Image(Convert.FromBase64String(vehiculo)).FitArea();
                            else
                                e.Text("Vehículo no encontrado").FontColor(Colors.Red.Medium);
                        });
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}
