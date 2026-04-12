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

        public static Func<Particula, SKColor> FuncionAmplitudAColor = (part) =>
        {
            return part.ObtenerAmplitudTotal() switch
            {
                <= 1 => SKColor.Parse("#0011ff"),
                <= 2 => SKColor.Parse("#036603"),
                <= 3 => SKColor.Parse("#cc0000"),
                <= 4 => SKColor.Parse("#ffe60a"),
                _ => SKColor.Parse("#FFFFFF")
            };
        };
        public static List<string> GenerarFramesPng(this Espacio espacio, string rutaSalida, int cantidadFrames, double deltaTimePorFrame)
        {
            var framePaths = new List<string>();
            Directory.CreateDirectory(rutaSalida);
            try
            {
                // Generar frames como PNG
                for (int frameIdx = 0; frameIdx < cantidadFrames; frameIdx++)
                {
                    var bitmap = new SKBitmap(ANCHO, ALTO, SKColorType.Rgba8888, SKAlphaType.Opaque);
                    
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.LightGray);

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

                        var posicionesTexto = new List<(float x, float y, string texto)>();
                        const int cantidadRayos = 36;
                        const float radioMaximo = 200f;
                        var particulas = espacio
                            .Particulas
                            .OrderByDescending(p => p.ObtenerAmplitudTotal())
                            .GroupBy(p => p.Posicion2D)
                            .Select(g => g.First())
                            .ToList();
                        var fasesEncontradas = new List<double>();

                        foreach (var particula in particulas)
                        {                            
                            var color = FuncionAmplitudAColor(particula);                     
                            var x = CENTROX + (float) particula.Posicion2D.X;
                            var y = CENTROY - (float) particula.Posicion2D.Y;

                            // Dibujar círculo
                            using (var paint = new SKPaint { Color = color, IsAntialias = true })
                            {
                                canvas.DrawCircle(x, y, 6f, paint);
                            }

                            // Dibujar múltiples rayos irradiados desde la posición de la partícula
                            for (int i = 0; i < cantidadRayos; i++)
                            {
                                float angulo = (float)(2 * Math.PI * i / cantidadRayos);
                                float x1 = x + radioMaximo * (float)Math.Cos(angulo);
                                float y1 = y - radioMaximo * (float)Math.Sin(angulo);
                                
                                using (var paint = new SKPaint 
                                { 
                                    Color = new SKColor(color.Red, color.Green, color.Blue, 50),
                                    StrokeWidth = 2f,
                                    IsAntialias = true 
                                })
                                {
                                    canvas.DrawLine(x, y, x1, y1, paint);
                                }
                            }

                            if(!fasesEncontradas.Any(f => Math.Abs(f - particula.Fase) < 0.01))
                            {
                                // Dibujar nombre de la partícula
                                float xText = x;
                                float yText = y - 20;
                                var predicado = particula.ToString();
                                
                                posicionesTexto.Add((xText, yText, predicado));
                                fasesEncontradas.Add(particula.Fase);
                                continue;
                            }                
                        }

                        var textosAgrupados = posicionesTexto
                            .GroupBy(t => (int) t.y)
                            .Where(g => g.Count() > 1)
                            .ToList();
                        
                        foreach (var grupo in textosAgrupados)
                        {
                            var textos = grupo.OrderBy(t => t.x).ToList();
                            var separacion = 350 / textos.Count;
                            var primerX = textos.First().x - 200;
                            for (int i = 0; i < textos.Count; i++)
                            {
                                var (x, y, texto) = textos[i];
                                int idx = posicionesTexto.FindIndex(t => t.x == x && t.y == y && t.texto == texto);
                                posicionesTexto[idx] = (separacion * (i + 1) + primerX, y, texto);
                            }
                        }

                        // Dibujar los textos
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                        using (var font = new SKFont(typeface, 12f))
                        using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                        {
                            foreach (var (x, y, texto) in posicionesTexto)
                            {
                                canvas.DrawText(texto, x, y, font, paint);
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
                        int totalParticulas = espacio.Particulas.Count;
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal))
                        using (var font = new SKFont(typeface, 12f))
                        using (var paint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
                        {
                            canvas.DrawText($"Cantidad de partículas: {totalParticulas}.", 30, 80, font, paint);
                        }
                    }

                    // Guardar frame como PNG
                    var framePath = Path.Combine(rutaSalida, $"frame_{frameIdx:D3}.png");
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

        private static bool RectsOverlap(SKRect rect1, SKRect rect2)
        {
            const float margen = 5f;
            return !(rect1.Right + margen < rect2.Left || 
                     rect1.Left - margen > rect2.Right || 
                     rect1.Bottom + margen < rect2.Top || 
                     rect1.Top - margen > rect2.Bottom);
        }
    }
}




