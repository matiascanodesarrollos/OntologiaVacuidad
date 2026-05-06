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
        private const int CANTIDAD_RAYOS = 36;
        private const float RADIO_RAYO_MAXIMO = 220f;
        private const float RADIO_RAYO_MINIMO = 18f;

        public static Func<double, SKColor> FuncionAmplitudAColor = (amp) =>
        {
            return amp switch
            {
                <= 1 => SKColor.Parse("#cc0000"),
                <= 2 => SKColor.Parse("#0011ff"),
                <= 3 => SKColor.Parse("#036603"),                
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

                        foreach (var particula in espacio.Particulas)
                        {
                            var valor = particula.Causa.Esencia.Apariencia.Funcion(particula.Tiempo);
                            var amplitud = Math.Sqrt(valor.EjeReal * valor.EjeReal + valor.EjeImaginario * valor.EjeImaginario);
                            var color = FuncionAmplitudAColor(amplitud);
                            var faseActual = (particula.Tiempo * particula.Frecuencia) + particula.Fase;
                            var amplitudRayo = 0.85d + (0.35d * ((Math.Sin(faseActual) + 1d) / 2d));
                            var x = CENTROX + (float) particula.Posicion2D.X;
                            var y = CENTROY - (float) particula.Posicion2D.Y;

                            DibujarRayosParticula(canvas, x, y, color, amplitudRayo, faseActual);

                            // Dibujar círculo
                            using (var paint = new SKPaint { Color = FuncionAmplitudAColor(particula.Amplitud), IsAntialias = true })
                            {
                                canvas.DrawCircle(x, y, 6f, paint);
                            }

                            if(particula.Texto != "Vacuidad")
                            {
                                // Dibujar los textos
                                using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                                using (var font = new SKFont(typeface, 12f))
                                using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                                {
                                    canvas.DrawText(particula.ToString(), x + 12f, y - 12f, font, paint);
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
        
        private static double CalcularIntensidadRayo(IEnumerable<(double Amplitud, double Fase)> ondas, double angulo)
        {
            return ondas.Sum(onda => Math.Abs(onda.Amplitud) * Math.Max(0d, Math.Cos(angulo - onda.Fase)));
        }

        private static void DibujarRayosParticula(SKCanvas canvas, float x, float y, SKColor color, double amplitud, double faseActual)
        {
            if (amplitud <= 0d)
            {
                return;
            }

            var ondas = new[]
            {
                (Amplitud: amplitud, Fase: faseActual),
                (Amplitud: amplitud * 0.35d, Fase: faseActual + Math.PI)
            };
            var amplitudMaxima = ondas.Sum(onda => Math.Abs(onda.Amplitud));

            using var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round
            };

            using var nucleoPaint = new SKPaint
            {
                Color = color.WithAlpha(180),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawCircle(x, y, 10f, nucleoPaint);

            for (int i = 0; i < CANTIDAD_RAYOS; i++)
            {
                var angulo = (Math.PI * 2d * i) / CANTIDAD_RAYOS;
                var intensidad = CalcularIntensidadRayo(ondas, angulo);

                if (intensidad <= 0.01d)
                {
                    continue;
                }

                var proporcion = Math.Clamp(intensidad / amplitudMaxima, 0d, 1d);
                var longitud = CalcularLongitudRayo(intensidad, amplitudMaxima);
                var alpha = CalcularAlphaRayo(intensidad, amplitudMaxima);
                paint.Color = color.WithAlpha(alpha);
                paint.StrokeWidth = 1.5f + (3.5f * (float)proporcion);

                var destinoX = x + (float)(Math.Cos(angulo) * longitud);
                var destinoY = y - (float)(Math.Sin(angulo) * longitud);
                canvas.DrawLine(x, y, destinoX, destinoY, paint);
            }
        }

        private static float CalcularLongitudRayo(double intensidad, double amplitudMaxima)
        {
            var proporcion = Math.Clamp(intensidad / amplitudMaxima, 0d, 1d);
            return RADIO_RAYO_MINIMO + (float)((RADIO_RAYO_MAXIMO - RADIO_RAYO_MINIMO) * proporcion);
        }

        private static byte CalcularAlphaRayo(double intensidad, double amplitudMaxima)
        {
            var proporcion = Math.Clamp(intensidad / amplitudMaxima, 0d, 1d);
            return (byte)(140 + (115 * proporcion));
        }
    }
}




