using System;
using System.Numerics;

public class Designacion : Nombre
{
    public Guid Id { get; }
    private readonly Lazy<double> frecuenciaAngular;
    private readonly Lazy<Apariencia> causa;
    public double FrecuenciaAngular => frecuenciaAngular.Value;
    public Func<(double tau, double FrecuenciaAngular), Complex> STFT { get; }
    public Apariencia Causa => causa.Value;

    /// <summary>
    /// Constructor de copia para crear una nueva designación a partir de otra para herencia.
    /// <param name="otra">La designación de la cual se copiarán las propiedades.</param>
    /// </summary>   
    public Designacion(Designacion otra)
        : base(otra.Texto, otra.VelocidadGrupo, otra.TransformadaFourier)
    {
        Id = Guid.NewGuid();
        frecuenciaAngular = new Lazy<double>(() => otra.FrecuenciaAngular);
        STFT = otra.STFT;
        causa = new Lazy<Apariencia>(() => otra.Causa);
    }

    internal Designacion(
        Nombre nombre, 
        Func<(double tau, double FrecuenciaAngular), Complex> funcion)
        : base(nombre.Texto, nombre.VelocidadGrupo, nombre.TransformadaFourier)
    {
        Id = Guid.NewGuid();
        STFT = funcion;
        frecuenciaAngular = new Lazy<double>(() => EstimarFrecuenciaAngular(STFT));
        causa = new Lazy<Apariencia>(() =>
            new Apariencia(
                new Palabra(
                    nombre.Texto,
                    FrecuenciaAngular,
                    t => STFT((t, FrecuenciaAngular))),
                this));
    }

    internal Designacion(
        Nombre nombre,
        Apariencia causa)
        : base(nombre.Texto, nombre.VelocidadGrupo, nombre.TransformadaFourier)
    {
        Id = Guid.NewGuid();
        frecuenciaAngular = new Lazy<double>(() => causa.FrecuenciaAngular);
        STFT = x => nombre.TransformadaFourier(x.FrecuenciaAngular);
        this.causa = new Lazy<Apariencia>(() => causa);
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
            nombre,
            apariencia
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
    /// Designación base. Vacuidad.
    /// </summary>
    public static Designacion Vacuidad = new Designacion(
        Cuerpo, 
        x => new Complex(
            x.tau == 0 ? 0.5 * double.PositiveInfinity : 0.0,
            1 / (2 * Math.PI * x.tau)) //Transformada inversa de u(ω)
    );

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
