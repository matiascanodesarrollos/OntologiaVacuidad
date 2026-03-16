using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using DomainLogic.Services.Particulas;
using SkiaSharp;

namespace ConsoleApp
{
    public class GifGenerator
    {
        private const int ANCHO = 1200;
        private const int ALTO = 900;
        private const int PADDING = 75;
        private readonly ILogger<GifGenerator> _logger;

        public GifGenerator(ILogger<GifGenerator> logger)
        {
            _logger = logger;
        }

        public void GenerarGifAnimado(List<Electron> electrons, Dictionary<Electron, List<Vector2D>> trayectorias, string rutaSalida)
        {
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
            
            // Calcular rango global
            var todasLasPosiciones = trayectorias.Values.SelectMany(t => t).ToList();
            var minX = todasLasPosiciones.Min(p => p.X);
            var maxX = todasLasPosiciones.Max(p => p.X);
            var minY = todasLasPosiciones.Min(p => p.Y);
            var maxY = todasLasPosiciones.Max(p => p.Y);

            var rangoX = maxX - minX > 0 ? maxX - minX : 1;
            var rangoY = maxY - minY > 0 ? maxY - minY : 1;

            Func<double, double> mapearX = x => PADDING + ((x - minX) / rangoX) * (ANCHO - 2 * PADDING);
            Func<double, double> mapearY = y => ALTO - PADDING - ((y - minY) / rangoY) * (ALTO - 2 * PADDING);

            var maxPuntos = trayectorias.Values.Max(t => t.Count);
            var framesDir = Path.Combine(Path.GetDirectoryName(rutaSalida), "frames");
            Directory.CreateDirectory(framesDir);

            try
            {
                int frameNum = 0;
                // Generar frames como PNG
                for (int frameIdx = 0; frameIdx <= maxPuntos; frameIdx += 5)
                {
                    var indice = Math.Min(frameIdx, maxPuntos - 1);
                    var bitmap = new SKBitmap(ANCHO, ALTO, SKColorType.Rgba8888, SKAlphaType.Opaque);
                    
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.White);

                        // Grid
                        using (var paint = new SKPaint { Color = new SKColor(224, 224, 224), StrokeWidth = 0.5f })
                        {
                            for (double x = PADDING; x < ANCHO - PADDING; x += 75)
                            {
                                canvas.DrawLine((float)x, PADDING, (float)x, ALTO - PADDING, paint);
                            }
                            for (double y = PADDING; y < ALTO - PADDING; y += 75)
                            {
                                canvas.DrawLine(PADDING, (float)y, ANCHO - PADDING, (float)y, paint);
                            }
                        }

                        // Dibujar trayectorias
                        int colorIdx = 0;
                        foreach (var electron in electrons)
                        {
                            if (trayectorias.TryGetValue(electron, out var puntos) && puntos.Count > 1)
                            {
                                var color = frecuenciaAColor(electron.Causa.Frecuencia);
                                var puntosHastaNow = puntos.Take(indice + 1).ToList();

                                // Dibujar línea de trayectoria
                                if (puntosHastaNow.Count > 1)
                                {
                                    using (var paint = new SKPaint
                                    {
                                        Color = color,
                                        StrokeWidth = 3f,
                                        IsAntialias = true,
                                        Style = SKPaintStyle.Stroke,
                                        StrokeCap = SKStrokeCap.Round,
                                        StrokeJoin = SKStrokeJoin.Round
                                    })
                                    {
                                        var path = new SKPath();
                                        var primero = true;
                                        foreach (var punto in puntosHastaNow)
                                        {
                                            var x = (float)mapearX(punto.X);
                                            var y = (float)mapearY(punto.Y);
                                            if (primero)
                                            {
                                                path.MoveTo(x, y);
                                                primero = false;
                                            }
                                            else
                                            {
                                                path.LineTo(x, y);
                                            }
                                        }
                                        canvas.DrawPath(path, paint);
                                    }
                                }

                                // Dibujar posición actual
                                if (puntosHastaNow.Count > 0)
                                {
                                    var ultimo = puntosHastaNow[puntosHastaNow.Count - 1];
                                    var x = (float)mapearX(ultimo.X);
                                    var y = (float)mapearY(ultimo.Y);
                                    
                                    // Pequeño offset para separar visualmente electrones con la misma trayectoria
                                    var offsetAngle = colorIdx * (2 * Math.PI / 7); // Distribuir en círculo
                                    var offsetDistance = 12f;
                                    var offsetX = x + (float)(Math.Cos(offsetAngle) * offsetDistance);
                                    var offsetY = y + (float)(Math.Sin(offsetAngle) * offsetDistance);
                                    
                                    // Offset mayor para el texto para evitar que quede cortado en esquinas
                                    var textOffsetDistance = 22f;
                                    var textOffsetX = x + (float)(Math.Cos(offsetAngle) * textOffsetDistance);
                                    var textOffsetY = y + (float)(Math.Abs(Math.Sin(offsetAngle)) * textOffsetDistance);

                                    using (var paint = new SKPaint
                                    {
                                        Color = color,
                                        Style = SKPaintStyle.Fill,
                                        IsAntialias = true
                                    })
                                    {
                                        canvas.DrawCircle(offsetX, offsetY, 8f, paint);
                                    }

                                    using (var paint = new SKPaint
                                    {
                                        Color = SKColors.Black,
                                        Style = SKPaintStyle.Stroke,
                                        StrokeWidth = 2f,
                                        IsAntialias = true
                                    })
                                    {
                                        canvas.DrawCircle(offsetX, offsetY, 8f, paint);
                                    }

                                    // Etiqueta en negrita para mejor legibilidad
                                    using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                                    using (var font = new SKFont(typeface, 14f))
                                    using (var paint = new SKPaint
                                    {
                                        Color = SKColors.Black,
                                        IsAntialias = true
                                    })
                                    {
                                        var label = electron.Naturaleza.Texto ?? "e-";
                                        canvas.DrawText(label, textOffsetX, textOffsetY, font, paint);
                                    }
                                }

                                colorIdx++;
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
                            canvas.DrawText("X", ANCHO - 60, ALTO - 50, font, paint);
                            canvas.DrawText("Y", PADDING + 20, PADDING - 30, font, paint);
                        }

                        // Tiempo simulado en negrita
                        var tiempoSimulado = indice * 0.1;
                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                        using (var font = new SKFont(typeface, 20f))
                        using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                        {
                            canvas.DrawText($"Tiempo: {tiempoSimulado:F1}s", 30, 50, font, paint);
                        }
                    }

                    // Guardar frame como PNG
                    var framePath = Path.Combine(framesDir, $"frame_{frameNum:D3}.png");
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var file = File.Create(framePath))
                    {
                        encoded.SaveTo(file);
                    }

                    bitmap.Dispose();
                    frameNum++;
                }

                _logger.LogInformation($"✓ {frameNum} frames PNG generados en: {framesDir}");
                _logger.LogInformation($"✓ Para crear un GIF, ejecute: ffmpeg -i frames/frame_%03d.png -r 5 {rutaSalida}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generando frames: {ex.Message}", ex);
            }
        }
    }
}




