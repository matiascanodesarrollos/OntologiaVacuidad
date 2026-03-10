using System;
using System.Linq;

namespace DomainLogic.Services.Plasma
{
    public readonly struct RgbColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public RgbColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";
    }

    public static class PlasmaColor
    {
        public static RgbColor FromFieldEnergy(double fieldEnergy)
        {
            // Paleta dirigida para visualizar progresión del campo:
            // Naranja -> Amarillo -> Verde -> Blanco -> Azul.
            double energy = NormalizeFieldEnergy(fieldEnergy);

            var naranja = new RgbColor(255, 140, 0);
            var amarillo = new RgbColor(255, 215, 0);
            var verde = new RgbColor(50, 205, 50);
            var blanco = new RgbColor(255, 255, 255);
            var azul = new RgbColor(30, 144, 255);

            if (energy < 0.25)
            {
                return LerpColor(naranja, amarillo, energy / 0.25);
            }

            if (energy < 0.50)
            {
                return LerpColor(amarillo, verde, (energy - 0.25) / 0.25);
            }

            if (energy < 0.75)
            {
                return LerpColor(verde, blanco, (energy - 0.50) / 0.25);
            }

            return LerpColor(blanco, azul, (energy - 0.75) / 0.25);
        }

        public static double GetFieldProgress(double fieldEnergy)
        {
            return NormalizeFieldEnergy(fieldEnergy);
        }

        public static RgbColor FromDynamics(double[] amplitudes, double[] frequencies, double fieldStrength)
        {
            if (amplitudes.Length == 0)
            {
                return new RgbColor(0, 0, 0);
            }

            double avgAmp = amplitudes.Average();
            double maxAmp = amplitudes.Max();
            double variance = amplitudes.Select(a => (a - avgAmp) * (a - avgAmp)).Average();
            double freqSignature = frequencies.Length == 0 ? 0.0 : Math.Abs(frequencies.Average()) % 360.0;

            // Mapeo HSV -> RGB para obtener paleta continua y estable.
            double hue = (freqSignature + fieldStrength * 120.0) % 360.0;
            double saturation = Clamp01(0.35 + variance * 2.0);
            double value = Clamp01(0.25 + avgAmp * 0.75 + maxAmp * 0.15);

            return HsvToRgb(hue, saturation, value);
        }

        public static string Describe(RgbColor color)
        {
            var (h, s, v) = RgbToHsv(color);
            string familia = GetColorFamily(color);

            string tono;
            if (v >= 0.9 && s <= 0.2)
            {
                tono = "brillante";
            }
            else if (v >= 0.78 && s < 0.35)
            {
                tono = "pastel";
            }
            else if (v >= 0.72)
            {
                tono = "claro";
            }
            else if (v >= 0.45)
            {
                tono = s >= 0.65 ? "intenso" : "medio";
            }
            else
            {
                tono = s >= 0.6 ? "profundo" : "oscuro";
            }

            return $"{familia} {tono}";
        }

        public static string GetColorFamily(RgbColor color)
        {
            var (h, s, v) = RgbToHsv(color);

            if (v >= 0.86 && s <= 0.18)
            {
                return "Blanco";
            }

            // Rangos ampliados para no perder Amarillo/Verde en interpolaciones RGB.
            if (h < 42)
            {
                return "Naranja";
            }

            if (h < 82)
            {
                return "Amarillo";
            }

            if (h < 170)
            {
                return "Verde";
            }

            if (h < 265)
            {
                return "Azul";
            }

            return "Violeta";
        }

        private static (double h, double s, double v) RgbToHsv(RgbColor color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h;
            if (delta == 0)
            {
                h = 0;
            }
            else if (max == r)
            {
                h = 60 * (((g - b) / delta) % 6);
            }
            else if (max == g)
            {
                h = 60 * (((b - r) / delta) + 2);
            }
            else
            {
                h = 60 * (((r - g) / delta) + 4);
            }

            if (h < 0)
            {
                h += 360;
            }

            double s = max == 0 ? 0 : delta / max;
            double v = max;
            return (h, s, v);
        }

        private static RgbColor HsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;

            double r1;
            double g1;
            double b1;

            if (h < 60)
            {
                r1 = c; g1 = x; b1 = 0;
            }
            else if (h < 120)
            {
                r1 = x; g1 = c; b1 = 0;
            }
            else if (h < 180)
            {
                r1 = 0; g1 = c; b1 = x;
            }
            else if (h < 240)
            {
                r1 = 0; g1 = x; b1 = c;
            }
            else if (h < 300)
            {
                r1 = x; g1 = 0; b1 = c;
            }
            else
            {
                r1 = c; g1 = 0; b1 = x;
            }

            return new RgbColor(
                (byte)Math.Round((r1 + m) * 255),
                (byte)Math.Round((g1 + m) * 255),
                (byte)Math.Round((b1 + m) * 255));
        }

        private static RgbColor LerpColor(RgbColor from, RgbColor to, double t)
        {
            double k = Clamp01(t);
            byte r = (byte)Math.Round(from.R + (to.R - from.R) * k);
            byte g = (byte)Math.Round(from.G + (to.G - from.G) * k);
            byte b = (byte)Math.Round(from.B + (to.B - from.B) * k);
            return new RgbColor(r, g, b);
        }

        private static double NormalizeFieldEnergy(double fieldEnergy)
        {
            // Mapeo lineal: energía 0.0 -> progreso 0, energía 1.0 -> progreso 1.0
            // Esto permite visualizar la gama completa de colores con energías realistas.
            return Clamp01(fieldEnergy);
        }

        private static double Clamp01(double value)
        {
            if (value < 0.0) return 0.0;
            if (value > 1.0) return 1.0;
            return value;
        }
    }
}
