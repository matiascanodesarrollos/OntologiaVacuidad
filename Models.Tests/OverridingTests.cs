using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class OverridingTests
{
    [Fact]
    public void Apariencia_PermiteSobrescribirCalculoDeTransformada()
    {
        var aparienciaBase = Apariencia.Mente;
        var derivada = new AparienciaConTransformadaFija(aparienciaBase);

        var valor = derivada.InvocarCalculo(1.25);

        valor.Should().Be(new Complex(42.0, -7.0));
    }

    [Fact]
    public void Designacion_PermiteSobrescribirEstimadorDeFrecuencia()
    {
        var derivada = new DesignacionConEstimadorFijo(Designacion.Vacuidad);

        var frecuencia = derivada.InvocarEstimador((x => new Complex(x.Frecuencia * x.Frecuencia, 0.0)));

        frecuencia.Should().Be(3.5);
    }

    private sealed class AparienciaConTransformadaFija : Apariencia
    {
        public AparienciaConTransformadaFija(Apariencia otra)
            : base(otra)
        {
        }

        protected override Complex CalcularTransformadaFourier(double frecuencia)
        {
            return new Complex(42.0, -7.0);
        }

        public Complex InvocarCalculo(double frecuencia)
        {
            return CalcularTransformadaFourier(frecuencia);
        }
    }

    private sealed class DesignacionConEstimadorFijo : Designacion
    {
        public DesignacionConEstimadorFijo(Designacion otra)
            : base(otra)
        {
        }

        protected override double EstimarFrecuencia(Func<(double tau, double Frecuencia), Complex> funcion)
        {
            return 3.5;
        }

        public double InvocarEstimador(Func<(double tau, double Frecuencia), Complex> funcion)
        {
            return EstimarFrecuencia(funcion);
        }
    }
}
