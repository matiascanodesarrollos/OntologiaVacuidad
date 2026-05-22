using System;
using System.Numerics;

public class Designacion : Nombre
{
    public Guid Id { get; }
    public Palabra Causa { get; }
    public Apariencia Esencia { get; }
    public Func<(double tau, double FrecuenciaAngular), Complex> STFT { get; }
    private readonly Lazy<double> frecuenciaAngular;
    public double FrecuenciaAngular => frecuenciaAngular.Value;
    
    /// <summary>
    /// Constructor de copia para crear una nueva designación a partir de otra para herencia.
    /// <param name="otra">La designación de la cual se copiarán las propiedades.</param>
    /// </summary>   
    public Designacion(Designacion otra)
        : base(otra.Texto, otra.VelocidadGrupo, otra.Ventana)
    {
        Id = Guid.NewGuid();
        Causa = otra.Causa;
        Esencia = otra.Esencia;
        STFT = otra.STFT;
        frecuenciaAngular = new Lazy<double>(() => otra.FrecuenciaAngular);
              
    }

    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre.Texto, nombre.VelocidadGrupo, nombre.Ventana)
    {
        Id = Guid.NewGuid();
        Causa = apariencia as Palabra;
        Esencia = apariencia;
        STFT = p => CalcularSTFT(p.tau, p.FrecuenciaAngular, apariencia.Funcion, nombre.Ventana);
        frecuenciaAngular = new Lazy<double>(() => EstimarFrecuenciaAngular(STFT));
    }    

    /// <summary>
    /// Crea una designación usando la frecuencia angular de la esencia de la apariencia y el espectro del nombre.
    /// </summary>
    /// <param name="apariencia">Apariencia desde la que se toma la frecuencia angular base.</param>
    /// <param name="nombre">Nombre que aporta texto, velocidad de grupo y transformada.</param>
    /// <returns>Una nueva designación vinculada a la apariencia de entrada.</returns>
    public static Designacion Designar(Apariencia apariencia, Nombre nombre)
    {
        var nuevaDesignacion = new Designacion(
            apariencia,
            nombre
        );
        return nuevaDesignacion;
    }

    /// <summary>
    /// Sobreescribe Equals para comparar designaciones por su Id.
    /// </summary>
    /// <returns>True si las designaciones son iguales, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Designacion other)
        {
            return Id == other.Id;
        }
        return false;
    }

    /// <summary>
    /// Sobreescribe GetHashCode para comparar designaciones por su Id.
    /// </summary>
    /// <returns>El hash code de la designación.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Calcula la STFT de una funcion de apariencia para un desplazamiento temporal y una frecuencia angular.
    /// </summary>
    /// <param name="tau">Desplazamiento temporal de la ventana.</param>
    /// <param name="frecuenciaAngular">Frecuencia angular de analisis.</param>
    /// <param name="funcion">Funcion temporal de la apariencia a analizar.</param>
    /// <param name="ventanaAnalisis">Función de ventana a utilizar en el cálculo de la STFT.</param>
    /// <returns>Valor complejo de la STFT en el punto (tau, frecuenciaAngular).</returns>
    protected virtual Complex CalcularSTFT(double tau, double frecuenciaAngular, Func<double, Complex> funcion, Func<double, Complex> ventanaAnalisis)
    {
        const double limiteIntegracion = 8.0;
        const int pasos = 2048;
        var dt = 2.0 * limiteIntegracion / pasos;

        var suma = Complex.Zero;

        for (var i = 0; i <= pasos; i++)
        {
            var t = -limiteIntegracion + (i * dt);
            var peso = (i == 0 || i == pasos) ? 0.5 : 1.0;
            var x = funcion(t);
            var w = Complex.Conjugate(ventanaAnalisis(t - tau));
            var exponente = Complex.FromPolarCoordinates(1.0, -frecuenciaAngular * t);

            suma += peso * x * w * exponente;
        }

        return suma * dt;
    }

    /// <summary>
    /// Estima la frecuencia angular característica de la designación a partir de su función espectral.
    /// </summary>
    /// <param name="funcion">Función espectral compleja usada para muestrear magnitudes.</param>
    /// <returns>Frecuencia angular estimada; devuelve 0 cuando el espectro es plano o no identificable.</returns>
    /// <remarks>
    /// Este método se invoca de forma diferida por <see cref="FrecuenciaAngular"/> (lazy). Si se sobreescribe,
    /// la nueva estrategia impacta la frecuencia angular materializada en el primer acceso y la construcción de
    /// <see cref="Causa"/>, que depende de dicha frecuencia angular.
    /// </remarks>
    protected virtual double EstimarFrecuenciaAngular(Func<(double tau, double FrecuenciaAngular), Complex> funcion)
    {
        const double minFrecuenciaAngular = -8.0;
        const double maxFrecuenciaAngular = 8.0;
        const int pasos = 256;
        const double tauMuestreo = 1.0;

        var delta = (maxFrecuenciaAngular - minFrecuenciaAngular) / pasos;
        var mejorFrecuenciaAngular = 0.0;
        var maxMagnitud = double.NegativeInfinity;
        var minMagnitud = double.PositiveInfinity;

        for (var i = 0; i <= pasos; i++)
        {
            var frecuenciaAngular = minFrecuenciaAngular + (i * delta);
            var valor = funcion((tauMuestreo, frecuenciaAngular));
            var magnitud = valor.Magnitude;

            if (!double.IsFinite(magnitud))
            {
                continue;
            }

            if (magnitud > maxMagnitud)
            {
                maxMagnitud = magnitud;
                mejorFrecuenciaAngular = frecuenciaAngular;
            }

            if (magnitud < minMagnitud)
            {
                minMagnitud = magnitud;
            }
        }

        if (!double.IsFinite(maxMagnitud))
        {
            return 0.0;
        }

        var espectroPlano = Math.Abs(maxMagnitud - minMagnitud) <= 1e-9 * (1.0 + Math.Abs(maxMagnitud));
        return espectroPlano ? 0.0 : mejorFrecuenciaAngular;
    }
}
