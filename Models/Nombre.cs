using System;
using System.Numerics;

public class Nombre
{
    public string Texto { get; }
    public Func<double, Complex> TransformadaFourier { get; }
    public double VelocidadGrupo { get; }

    /// <summary>
    /// Crea un nuevo nombre con texto, velocidad de grupo y su transformada de Fourier.
    /// </summary>
    /// <param name="texto">Texto del nombre.</param>
    /// <param name="velocidadGrupo">Velocidad de propagación asociada al nombre.</param>
    /// <param name="transformadaFourier">Función espectral que devuelve amplitud y fase para una frecuencia dada.</param>
    internal Nombre(string texto, 
        double velocidadGrupo,
        Func<double, Complex> transformadaFourier)
    {
        Texto = texto;
        VelocidadGrupo = velocidadGrupo;
        TransformadaFourier = transformadaFourier;
    }

    /// <summary>
    /// Proyecta esta designación en una nueva apariencia evaluando su función en una frecuencia angular dada.
    /// </summary>
    /// <param name="frecuenciaAngular">Frecuencia angular usada para evaluar la función de la designación.</param>
    /// <returns>Una apariencia construida a partir de la función de esta designación.</returns>
    public Apariencia Mostrarse(double frecuenciaAngular)
    {
        var apariencia = new Apariencia(
            new Palabra(
                Texto,
                frecuenciaAngular,
                t => CalcularTransformadaInversaFourier(t))
        );
        return apariencia;
    }

    /// <summary>
    /// Devuelve una representación textual simple del nombre.
    /// </summary>
    /// <returns>Una cadena con texto y velocidad de grupo.</returns>
    public override string ToString() => $"{Texto} (VelocidadGrupo: {VelocidadGrupo})";

    /// <summary>
    /// Compara nombres por su texto.
    /// </summary>
    /// <returns>True si ambos nombres tienen el mismo texto, false en caso contrario.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Nombre other)
        {
            return Texto == other.Texto;
        }
        return false;
    }

    /// <summary>
    /// Genera el hash code a partir del texto.
    /// </summary>
    /// <returns>El hash code del texto del nombre.</returns>
    public override int GetHashCode() => Texto.GetHashCode();

    /// <summary>
        /// Crea un nuevo nombre base "Vacuidad" con espectro escalón unitario u(ω).
    /// </summary>
    /// <param name="velocidadGrupo">La velocidad del grupo asociada al nuevo nombre.</param>
    /// <returns>Un nuevo nombre asociado a la causa Vacuidad.</returns>
        public static Nombre Vacuidad => new Nombre(
            nameof(Vacuidad),
            0.0,
            omega => omega >= 0.0 ? Complex.One : Complex.Zero); //u(ω)
    
    

    /// <summary>
    /// Calcula la transformada inversa de Fourier compleja del espectro definido por <see cref="TransformadaFourier"/>.
    /// </summary>
    /// <param name="t">Instante temporal de evaluación.</param>
    /// <returns>Valor complejo de la señal reconstruida en el tiempo indicado.</returns>
    /// <remarks>
    /// Al sobreescribir este método se altera la señal de ventana usada por <see cref="Mostrarse(double)"/>,
    /// y por lo tanto cambia la <see cref="Apariencia"/> resultante. La implementación derivada debería
    /// respetar coherencia de unidades entre tiempo y frecuencia angular para evitar reconstrucciones inestables.
    /// </remarks>
    protected virtual Complex CalcularTransformadaInversaFourier(double t)
    {
        const double limiteFrecuencia = 8.0;
        const int pasos = 4096;
        var dOmega = 2.0 * limiteFrecuencia / pasos;
        var suma = Complex.Zero;

        for (var i = 0; i <= pasos; i++)
        {
            var omega = -limiteFrecuencia + (i * dOmega);
            var valor = TransformadaFourier(omega);
            var peso = (i == 0 || i == pasos) ? 0.5 : 1.0;
            var exponente = Complex.FromPolarCoordinates(1.0, omega * t);
            suma += peso * valor * exponente;
        }

        return suma * dOmega;
    }
}
