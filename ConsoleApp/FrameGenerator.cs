using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainLogic.Services.Particulas;
using SkiaSharp;

namespace ConsoleApp
{
    public static class FrameGenerator
    {
        private const int ANCHO = 1200;
        private const int CENTROX = 560;
        private const int ALTO = 900;
        private const int CENTROY = 450;
        private const int PADDING = 20;
        public static List<string> GenerarFramesPng(this Espacio espacio, string rutaSalida, int cantidadFrames, double deltaTimePorFrame)
        {
            var framePaths = new List<string>();
            // Mapeo directo de frecuencias a colores específicos
            Func<double, SKColor> frecuenciaAColor = (freq) =>
            {
                return freq switch
                {
                    1000 => SKColor.Parse("#0066FF"), // Azul para 1000 Hz
                    600 => SKColor.Parse("#00CC00"), // Verde para 600 Hz
                    100 => SKColor.Parse("#FF0000"), // Rojo para 100 Hz
                    500 => SKColor.Parse("#FFFF00"), // Amarillo para 500 Hz
                    _ => SKColor.Parse("#808080") // Gris para frecuencias desconocidas
                };
            };
            
            var framesDir = Path.Combine(Path.GetDirectoryName(rutaSalida), "frames");
            Directory.CreateDirectory(framesDir);

            try
            {
                // Generar frames como PNG
                for (int frameIdx = 0; frameIdx < cantidadFrames; frameIdx++)
                {
                    var bitmap = new SKBitmap(ANCHO, ALTO, SKColorType.Rgba8888, SKAlphaType.Opaque);
                    
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.White);

                        // Grid
                        using (var paint = new SKPaint { Color = new SKColor(224, 224, 224), StrokeWidth = 0.5f })
                        {
                            for (double x = PADDING; x < ANCHO - PADDING; x += PADDING)
                            {
                                canvas.DrawLine((float)x, PADDING, (float)x, ALTO - PADDING, paint);
                            }
                            for (double y = PADDING; y < ALTO - PADDING; y += PADDING)
                            {
                                canvas.DrawLine(PADDING, (float)y, ANCHO - PADDING, (float)y, paint);
                            }
                        }

                        // Ejes
                        using (var paint = new SKPaint { Color = SKColors.Black, StrokeWidth = 2f })
                        {
                            canvas.DrawLine(PADDING, ALTO - PADDING, ANCHO - PADDING, ALTO - PADDING, paint);
                            canvas.DrawLine(PADDING, PADDING, PADDING, ALTO - PADDING, paint);
                        }

                        // Etiquetas de ejes en negrita
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                        using (var font = new SKFont(typeface, 16f))
                        using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                        {
                            canvas.DrawText("X", ANCHO - 30, ALTO - 30, font, paint);
                            canvas.DrawText("Y", PADDING + 30, PADDING - 30, font, paint);
                        }

                        var posicionesTexto = new List<(double x, double y, Particula p)>();
                        
                        // Primero dibujar fotones como haz de luz irradiado
                        var fotones = espacio.Particulas.Values.SelectMany(g => g.Where(p => p.Carga == 0)).ToList();
                        if (fotones.Any())
                        {
                            // Dibujar rayos de luz desde la posición de cada fotón
                            const int cantidadRayos = 36;
                            const float radioMaximo = 200f;
                            
                            foreach (var foton in fotones)
                            {
                                var color = frecuenciaAColor(foton.Causa.Frecuencia);
                                var colorConAlpha = new SKColor(color.Red, color.Green, color.Blue, 50); // Semi-transparente
                                
                                // Posición del fotón en el canvas
                                float xFoton = CENTROX + (float)foton.Posicion2D.X;
                                float yFoton = CENTROY - (float)foton.Posicion2D.Y;
                                
                                // Dibujar múltiples rayos irradiados desde la posición del fotón
                                for (int i = 0; i < cantidadRayos; i++)
                                {
                                    float angulo = (float)(2 * Math.PI * i / cantidadRayos);
                                    float x1 = xFoton + radioMaximo * (float)Math.Cos(angulo);
                                    float y1 = yFoton - radioMaximo * (float)Math.Sin(angulo);
                                    
                                    using (var paint = new SKPaint 
                                    { 
                                        Color = colorConAlpha, 
                                        StrokeWidth = 2f,
                                        IsAntialias = true 
                                    })
                                    {
                                        canvas.DrawLine(xFoton, yFoton, x1, y1, paint);
                                    }
                                }
                                
                                // Dibujar círculo de luz en la posición del fotón
                                using (var paint = new SKPaint 
                                { 
                                    Color = new SKColor(color.Red, color.Green, color.Blue, 150),
                                    IsAntialias = true 
                                })
                                {
                                    canvas.DrawCircle(xFoton, yFoton, 12f, paint);
                                }
                            }
                        }
                        
                        // Luego dibujar partículas con carga
                        foreach (var grupoParticulas in espacio.Particulas.Values)
                        {
                            foreach (var particula in grupoParticulas.Where(p => p.Carga != 0)) // Solo dibujar partículas con carga
                            {
                                var color = frecuenciaAColor(particula.Causa.Frecuencia);                                            
                                var x = CENTROX + (float) particula.Posicion2D.X;
                                var y = CENTROY - (float) particula.Posicion2D.Y;
                                
                                // Dibujar círculo
                                using (var paint = new SKPaint { Color = color, IsAntialias = true })
                                {
                                    canvas.DrawCircle(x, y, 6f, paint); // Escalar el tamaño del círculo
                                }
                                
                                // Dibujar nombre de la partícula
                                var xText = x + 10;
                                var yText = y - 5;
                                var textoExistente = posicionesTexto.Any(pt => Math.Abs(pt.x - xText) < 50 && Math.Abs(pt.y - yText) < 20);
                                if (!textoExistente)
                                {
                                    using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal))
                                    using (var font = new SKFont(typeface, 12f))
                                    using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                                    {
                                        var predicado = $"{particula.Causa.Texto} {particula.Naturaleza.Texto}";
                                        if(frameIdx > 1) // Mostrar detalles de frecuencia y amplitud después de algunos frames para evitar saturar la imagen inicialmente
                                        {
                                            predicado += $" ({particula.Causa.Frecuencia:F2} Hz, {particula.Efecto.Amplitud:F2} A)";
                                        }
                                        canvas.DrawText(predicado, xText, yText, font, paint);
                                        posicionesTexto.Add((xText, yText, particula));
                                    }
                                }
                                
                            }
                        }

                        // Tiempo simulado en negrita
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                        using (var font = new SKFont(typeface, 20f))
                        using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                        {
                            canvas.DrawText($"Tiempo: {frameIdx}s", 30, 50, font, paint);
                        }

                        // Debug: mostrar info de partículas
                        int totalParticulas = espacio.Particulas.Values.Sum(l => l.Count);
                        int electrons = espacio.Particulas.Values.Sum(l => l.Count(p => p as Electron != null));
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal))
                        using (var font = new SKFont(typeface, 12f))
                        using (var paint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
                        {
                            canvas.DrawText($"Total: {totalParticulas}, Fotones: {fotones.Count}, Electrons: {electrons}", 30, 80, font, paint);
                        }
                    }

                    // Guardar frame como PNG
                    var framePath = Path.Combine(framesDir, $"frame_{frameIdx:D3}.png");
                    framePaths.Add(framePath);
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var file = File.Create(framePath))
                    {
                        encoded.SaveTo(file);
                    }

                    bitmap.Dispose();
                    espacio.MoverParticulas(deltaTimePorFrame); // Mover partículas para el siguiente frame
                }

                return framePaths;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generando frames: {ex.Message}", ex);
            }
        }
    }
}




