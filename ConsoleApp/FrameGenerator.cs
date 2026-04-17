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
        private const float RADIO_RAYO_MAXIMO = 200f;
        private const float RADIO_RAYO_MINIMO = 12f;

        public static Func<double, SKColor> FuncionAmplitudAColor = (amp) =>
        {
            return amp switch
            {
                <= 0.5 => SKColor.Parse("#cc0000"),
                <= 1.5 => SKColor.Parse("#0011ff"),
                <= 4 => SKColor.Parse("#036603"),                
                <= 6 => SKColor.Parse("#ffe60a"),
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
                            var valor = particula.Esencia.Valor(espacio.Tiempo);
                            var color = FuncionAmplitudAColor(valor.Amplitud);                     
                            var x = CENTROX + (float) particula.Posicion2D.X;
                            var y = CENTROY - (float) particula.Posicion2D.Y;

                            // Dibujar círculo
                            using (var paint = new SKPaint { Color = color, IsAntialias = true })
                            {
                                canvas.DrawCircle(x, y, 6f, paint);
                            }

                            var ondasParticula = espacio.Ondas.TryGetValue(particula, out var ondas)
                                ? ondas
                                : Enumerable.Empty<(double Amplitud, double Fase)>().ToList();
                            var amplitudMaxima = CalcularAmplitudMaxima(ondasParticula);

                            // Dibujar rayos cuya longitud depende de la suma de ondas de la partícula.
                            for (int i = 0; i < CANTIDAD_RAYOS; i++)
                            {
                                double angulo = 2 * Math.PI * i / CANTIDAD_RAYOS;
                                var intensidad = CalcularIntensidadRayo(ondasParticula, angulo);

                                if (intensidad <= 0)
                                {
                                    continue;
                                }

                                var radioRayo = CalcularLongitudRayo(intensidad, amplitudMaxima);
                                float x1 = x + radioRayo * (float)Math.Cos(angulo);
                                float y1 = y - radioRayo * (float)Math.Sin(angulo);
                                byte alpha = CalcularAlphaRayo(intensidad, amplitudMaxima);

                                using (var paint = new SKPaint
                                {
                                    Color = new SKColor(color.Red, color.Green, color.Blue, alpha),
                                    StrokeWidth = 2f,
                                    IsAntialias = true
                                })
                                {
                                    canvas.DrawLine(x, y, x1, y1, paint);
                                }
                            }

                             // Dibujar los textos
                            using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                            using (var font = new SKFont(typeface, 12f))
                            using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                            {
                                canvas.DrawText(particula.ToString(), x, y, font, paint);
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

        private static double CalcularAmplitudMaxima(IEnumerable<(double Amplitud, double Fase)> ondas)
        {
            var amplitudMaxima = ondas.Sum(onda => Math.Abs(onda.Amplitud));
            return amplitudMaxima <= 0 ? 1d : amplitudMaxima;
        }

        private static float CalcularLongitudRayo(double intensidad, double amplitudMaxima)
        {
            var proporcion = Math.Clamp(intensidad / amplitudMaxima, 0d, 1d);
            return RADIO_RAYO_MINIMO + (float)((RADIO_RAYO_MAXIMO - RADIO_RAYO_MINIMO) * proporcion);
        }

        private static byte CalcularAlphaRayo(double intensidad, double amplitudMaxima)
        {
            var proporcion = Math.Clamp(intensidad / amplitudMaxima, 0d, 1d);
            return (byte)(40 + (160 * proporcion));
        }
    }
}




