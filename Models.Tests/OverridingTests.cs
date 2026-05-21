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
    public void Designacion_PermiteSobrescribirEstimadorDeFrecuenciaAngular()
    {
        var derivada = new DesignacionConEstimadorFijo(Designacion.Vacuidad);

        var frecuenciaAngular = derivada.InvocarEstimador((x => new Complex(x.FrecuenciaAngular * x.FrecuenciaAngular, 0.0)));

        frecuenciaAngular.Should().Be(3.5);
    }

    private sealed class AparienciaConTransformadaFija : Apariencia
    {
        public AparienciaConTransformadaFija(Apariencia otra)
            : base(otra)
        {
        }

        protected override Complex CalcularTransformadaFourier(double frecuenciaAngular)
        {
            return new Complex(42.0, -7.0);
        }

        public Complex InvocarCalculo(double frecuenciaAngular)
        {
            return CalcularTransformadaFourier(frecuenciaAngular);
        }
    }

    private sealed class DesignacionConEstimadorFijo : Designacion
    {
        public DesignacionConEstimadorFijo(Designacion otra)
            : base(otra)
        {
        }

        protected override double EstimarFrecuenciaAngular(Func<(double tau, double FrecuenciaAngular), Complex> funcion)
        {
            return 3.5;
        }

        public double InvocarEstimador(Func<(double tau, double FrecuenciaAngular), Complex> funcion)
        {
            return EstimarFrecuenciaAngular(funcion);
        }
    }
}
