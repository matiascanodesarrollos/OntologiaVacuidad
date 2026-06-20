using System;
using System.Numerics;

public class Designacion : Nombre
{
    public new Guid Id { get; }
    internal new Apariencia Esencia { get; }
    private Func<Complex, Complex> Laplace { get; }
    
    internal Designacion(Apariencia apariencia, Nombre nombre)
        : base(nombre)
    {
        Id = Guid.NewGuid();
        Esencia = apariencia;
        Laplace = CalcularLaplace;
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
    /// Calcula la transformada de Laplace evaluando la función de la esencia en la ventana del nombre 
    /// con un paso temporal de 1 por caracter del contexto.
    /// Sobreescribir para implementar diferentes formas de análisis o pasos temporales.
    /// </summary>
    /// <param name="s">Parámetro complejo de la transformada de Laplace.</param>
    /// <returns>Valor complejo de la transformada de Laplace en el punto s.</returns>
    protected virtual Complex CalcularLaplace(Complex s)
    {        
        var totalMuestras = Math.Max(1, Contexto.Length);
        var suma = Complex.Zero;

        // Paso temporal de 1 por caracter del contexto  
        for (var n = 0; n < totalMuestras; n++)
        {
            var muestra = Esencia.Funcion(n);
            var factor = Complex.Exp(-s * n);
            suma += muestra * factor; 
        }

        return suma;
    }

    /// <summary>
    /// Crea una nueva palabra a partir de z y el periodo de muestreo T.
    /// Sobreescribir para implementar diferentes formas de aparición o análisis de la respuesta.
    /// </summary>    
    /// <param name="z">Variable z, representación de la intención de quien aparenta.</param>
    /// <param name="T">Periodo de muestreo.</param>
    /// <param name="texto">Texto que se desea que aparezca.</param>
    /// <returns>La nueva palabra.</returns>
    public virtual Palabra Aparecer(
        Complex z, 
        double T,
        string texto)
    {        
        var muestras = Math.Max(1, Contexto.Length);
        var paso = 0.01;
        var X = Complex.Zero;
        var derivada = Complex.Zero;
        var periodoMuestreo = Math.Abs(T);

        for (int n = 0; n < muestras; n++)
        {
            var t = n * periodoMuestreo;
            var valor = Esencia.Funcion(t);
            var factor = Complex.Pow(z, -n);
            X += valor * factor;

            var valorPasoPositivo = Esencia.Funcion(t + paso);
            var valorPasoNegativo = Esencia.Funcion(t - paso);            
            derivada += (valorPasoPositivo - valorPasoNegativo) * factor / (2.0 * paso);
        }

        var s = 2 / periodoMuestreo 
            * ((z - Complex.One) / (z + Complex.One)); //Aproximación bilineal para convertir z a s
        var valorLaplace = Laplace(s);
        var flujo = valorLaplace * Complex.Conjugate(X); //Producto punto entre la designación y como se desea que aparezca
        Func<double, Complex> funcion = t => 
            flujo.Magnitude
            * Math.Cos(
                z.Phase * t 
                + flujo.Phase);
        var apariencia = new Apariencia(funcion)
        {
            Esencia = this,
        };

        var velocidadGrupo = X.Magnitude <= 1e-12 ? X.Magnitude : (derivada / X).Imaginary;
        var nombre = new Nombre(
            texto,
            Texto,
            Admitancia)
        {
            VelocidadGrupo = velocidadGrupo,
        };
        var palabra = new Palabra(texto, nombre, apariencia)
        {
            Efecto = nombre,
        };
        nombre.Causa = palabra;
        return palabra;
    }
}
