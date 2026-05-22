using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class OverridingTests
{
    [Fact]
    public void Apariencia_PermiteSobrescribirCalculoDeTransformada()
    {
        var aparienciaBase = CreateAparienciaBase();
        var derivada = new AparienciaConTransformadaFija(aparienciaBase);

        var valor = derivada.InvocarCalculo(1.25, _ => Complex.One);

        valor.Should().Be(new Complex(42.0, -7.0));
    }

    [Fact]
    public void Designacion_PermiteSobrescribirEstimadorDeFrecuenciaAngular()
    {
        var derivada = new DesignacionConEstimadorFijo(CreateDesignacionBase());

        var frecuenciaAngular = derivada.InvocarEstimador((x => new Complex(x.FrecuenciaAngular * x.FrecuenciaAngular, 0.0)));

        frecuenciaAngular.Should().Be(3.5);
    }

    [Fact]
    public void Helpers_DeConstruccion_CreanInstanciasValidas()
    {
        var apariencia = CreateAparienciaBase();
        var designacion = CreateDesignacionBase();

        apariencia.Should().NotBeNull();
        designacion.Should().NotBeNull();
    }

    [Fact]
    public void UnitStepTransform_CubreAmbasRamas()
    {
        var positivo = UnitStepTransform(0.25);
        var negativo = UnitStepTransform(-0.25);

        positivo.Should().Be(Complex.One);
        negativo.Should().Be(Complex.Zero);
    }

    [Fact]
    public void GaussianWindow_EntregaValorFinito()
    {
        var valor = GaussianWindow(0.5);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    private sealed class AparienciaConTransformadaFija : Apariencia
    {
        public AparienciaConTransformadaFija(Apariencia otra)
            : base(otra)
        {
        }

        protected override Complex CalcularTransformadaFourier(double frecuenciaAngular, Func<double, Complex> ventana)
        {
            return new Complex(42.0, -7.0);
        }

        public Complex InvocarCalculo(double frecuenciaAngular, Func<double, Complex> ventana)
        {
            return CalcularTransformadaFourier(frecuenciaAngular, ventana);
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

    private static Apariencia CreateAparienciaBase()
    {
        return Apariencia.Aparecer(new[]
        {
            new Palabra("Mente", 0.0, GaussianWindow),
            new Palabra("Mente", 1.0, GaussianWindow)
        });
    }

    private static Designacion CreateDesignacionBase()
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Cuerpo", 1.0, GaussianWindow) });
        var nombre = new Nombre("Cuerpo", 0.0, UnitStepTransform);
        return Designacion.Designar(apariencia, nombre);
    }

    private static Complex GaussianWindow(double t)
    {
        return new Complex(Math.Exp(-(t * t) / 2.0), 0.0);
    }

    private static Complex UnitStepTransform(double omega)
    {
        return omega >= 0.0 ? Complex.One : Complex.Zero;
    }
}
