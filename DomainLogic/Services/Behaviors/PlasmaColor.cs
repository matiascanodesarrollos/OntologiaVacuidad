using System;
using System.Linq;

namespace DomainLogic.Services.Behaviors
{
    /// <summary>
    /// Representa un color RGB
    /// </summary>
    public struct RgbColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public override string ToString() => $"RGB({R},{G},{B})";

        /// <summary>
        /// Retorna el color en formato hexadecimal #RRGGBB
        /// </summary>
        public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";
    }
    
    /// <summary>
    /// Calcula el color visible del plasma basado en sus dinámicas
    /// Mapea amplitudes, frecuencias y estados a valores RGB
    /// </summary>
    public class PlasmaColor
    {       

        /// <summary>
        /// Calcula el color del plasma basado en sus características dinámicas
        /// - Amplitud promedio → brillo
        /// - Ratio Max/Prom → tono (estructura del plasma: uniforme=rojo, muy picos=azul)
        /// - Varianza de amplitudes → saturación del color
        /// </summary>
        public static RgbColor CalcularColor(double[] amplitudes, double[] frequencies)
        {
            if (amplitudes.Length == 0)
                return new RgbColor { R = 0, G = 0, B = 0 };

            return CalcularColorInterno(amplitudes, frequencies);
        }

        /// <summary>
        /// Calcula el color de una sección del plasma (subconjunto de subportadoras)
        /// Útil para análisis local de dinámicas
        /// </summary>
        public static RgbColor CalcularColorSeccion(double[] amplitudes, double[] frequencies, int inicio, int fin)
        {
            if (amplitudes.Length == 0 || inicio >= fin || inicio < 0 || fin > amplitudes.Length)
                return new RgbColor { R = 0, G = 0, B = 0 };

            var seccion = amplitudes.Skip(inicio).Take(fin - inicio).ToArray();
            return CalcularColorInterno(seccion, frequencies);
        }

        /// <summary>
        /// Cálculo interno del color basado en amplitudes
        /// </summary>
        private static RgbColor CalcularColorInterno(double[] amplitudes, double[] frequencies)
        {
            if (amplitudes.Length == 0)
                return new RgbColor { R = 0, G = 0, B = 0 };

            // Calcular estadísticas
            double amplitudPromedio = amplitudes.Average();
            double amplitudMaxima = amplitudes.Max();
            double varianzaAmplitudes = amplitudes.Length > 1 
                ? Math.Sqrt(amplitudes.Sum(a => Math.Pow(a - amplitudPromedio, 2)) / amplitudes.Length)
                : 0;

            // Ratio Max/Promedio → controla el tono (hue)
            // Ratio ~1.0 (uniforme) → Rojo (hue=0)
            // Ratio ~10.0 (muy picos) → Azul (hue=1)
            double ratio = amplitudPromedio > 0.01 ? amplitudMaxima / amplitudPromedio : 1.0;
            double hue = Math.Min(1.0, Math.Max(0.0, (ratio - 1.0) / 9.0));

            // Saturation basada en varianza (más varianza = colores más puros)
            double saturation = Math.Min(1.0, varianzaAmplitudes * 2);

            // Value (brillo) basado en amplitud máxima
            double value = amplitudMaxima;

            // Convertir HSV a RGB
            return HsvToRgb(hue, saturation, value);
        }

        /// <summary>
        /// Convierte HSV (Hue, Saturation, Value) a RGB
        /// Hue: [0, 1] donde 0=rojo, 0.33=verde, 0.67=azul
        /// Saturation: [0, 1]
        /// Value: [0, 1]
        /// </summary>
        private static RgbColor HsvToRgb(double hue, double saturation, double value)
        {
            // Normalizar hue al rango [0, 6)
            double h = hue * 6.0;
            if (h >= 6.0) h = 0.0;
            
            int i = (int)Math.Floor(h);
            double f = h - i;
            double p = value * (1.0 - saturation);
            double q = value * (1.0 - saturation * f);
            double t = value * (1.0 - saturation * (1.0 - f));

            double r, g, b;
            switch (i)
            {
                case 0:
                    r = value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = value;
                    break;
                default:
                    r = value;
                    g = p;
                    b = q;
                    break;
            }

            return new RgbColor
            {
                R = (byte)Math.Round(r * 255),
                G = (byte)Math.Round(g * 255),
                B = (byte)Math.Round(b * 255)
            };
        }

        /// <summary>
        /// Calcula el color de estado del plasma
        /// Verde: normal, Amarillo: saturación, Rojo: desaparición
        /// </summary>
        public static RgbColor CalcularColorEstado(int saturaciones, int desapariciones, int numSubcarriers)
        {
            if (desapariciones > numSubcarriers * 0.3)
            {
                // Más de 30% desaparecido → Rojo oscuro
                return new RgbColor { R = 139, G = 0, B = 0 };
            }
            else if (saturaciones > numSubcarriers * 0.2)
            {
                // Más de 20% saturado → Amarillo/Naranja
                return new RgbColor { R = 255, G = 165, B = 0 };
            }
            else
            {
                // Normal → Verde
                return new RgbColor { R = 34, G = 139, B = 34 };
            }
        }

        /// <summary>
        /// Describe el color en lenguaje natural (Rojo, Cyan, Violeta, etc.)
        /// </summary>
        public static string DescribirColor(RgbColor color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            // Detectar escala de grises primero
            if (Math.Abs(r - g) < 30 && Math.Abs(g - b) < 30 && Math.Abs(r - b) < 30)
            {
                if (r < 50) return "Negro";
                if (r < 100) return "Gris oscuro";
                if (r < 150) return "Gris";
                if (r < 200) return "Gris claro";
                return "Blanco";
            }

            // Encontrar componente dominante
            int max = Math.Max(r, Math.Max(g, b));
            int min = Math.Min(r, Math.Min(g, b));
            int range = max - min;

            // Saturación: qué tan puro es el color
            double saturation = max == 0 ? 0 : (double)range / max;

            // Determinar tonalidad (hue)
            string tonalidad;
            if (r == max)
            {
                // Zona roja
                double hue = (g - b) / (double)range;
                if (hue < 0) hue += 6;
                tonalidad = hue < 1 ? "Rojo" : "Magenta";
            }
            else if (g == max)
            {
                // Zona verde
                double hue = 2 + (b - r) / (double)range;
                tonalidad = hue < 3 ? "Verde" : "Cian";
            }
            else
            {
                // Zona azul
                double hue = 4 + (r - g) / (double)range;
                tonalidad = hue < 5 ? "Cian" : "Azul";
            }

            // Agregar intensidad
            string intensidad = max < 100 ? "oscuro" : max < 200 ? "normal" : "brillante";

            // Agregar saturación
            string pureza = saturation < 0.3 ? "desaturado" : 
                           saturation < 0.7 ? "moderado" : "altamente saturado";

            return $"{tonalidad} {intensidad} {pureza}";
        }

        /// <summary>
        /// Categoriza el color en etapas de plasma (Frío → Caliente)
        /// </summary>
        public static string CategoriaPlasma(RgbColor color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            // Escala de frío a caliente
            if (r < 50 && g < 50 && b < 50) return "PLASMA INERTE (Oscuro/Transparente)";
            if (b > r + 50) return "PLASMA FRÍO (Azul)";
            if (g > r && g > b && g > 100) return "PLASMA ESTABLE (Verde)";
            if (r > g + 50 && b < 100 && r < 200) return "PLASMA CALIENTE (Rojo)";
            if (r > 200 && g > 150 && g > 0.8 * r && b < 100) return "PLASMA AMARILLO (Transición)";
            if (r > 200 && g > 100 && g < r - 50) return "PLASMA EXTREMO (Naranja)";            
            if (r > 245 && g > 245 && b > 245) return "PLASMA BLANCO (Muy caliente)";            
            
            return "PLASMA MIXTO";
        }
    }
}
