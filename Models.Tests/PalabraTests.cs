using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class PalabraTests
{
    [Fact]
    public void Constructor_CreaPalabraConTextoEsperado()
    {
        var palabra = new Palabra("Yo", 2.0, GaussianWindow);

        palabra.Texto.Should().Be("Yo");
        palabra.Id.Should().NotBe(Guid.Empty);
        palabra.FrecuenciaAngular.Should().Be(2.0);
    }

    [Theory]
    [InlineData(2.0, 0.25)]
    [InlineData(1.5, 0.2)]
    [InlineData(-3.0, 0.125)]
    [InlineData(0.0, 10.0)]
    public void Constructor_UsaFaseComplejaUnitariaConFrecuenciaAngularIndicada(double frecuenciaAngular, double t)
    {
        var palabra = new Palabra("Yo", frecuenciaAngular, GaussianWindow);
        var esperado = Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);

        palabra.Fase(t).Magnitude.Should().BeApproximately(1.0, 1e-12);
        palabra.Fase(t).Real.Should().BeApproximately(esperado.Real, 1e-12);
        palabra.Fase(t).Imaginary.Should().BeApproximately(esperado.Imaginary, 1e-12);
    }

    [Theory]
    [InlineData(0.25)]
    [InlineData(0.75)]
    [InlineData(10.0)]
    public void Constructor_ConFrecuenciaAngularCero_DevuelveFaseConstante(double t)
    {
        var palabra = new Palabra("Yo", 0.0, GaussianWindow);

        palabra.Fase(t).Real.Should().BeApproximately(1.0, 1e-12);
        palabra.Fase(t).Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Theory]
    [InlineData(1.0, 0.25)]
    [InlineData(2.5, 0.1)]
    [InlineData(-0.75, 0.4)]
    public void Constructor_RespetaPeriodicidadDeLaFase(double frecuenciaAngular, double t)
    {
        var palabra = new Palabra("Yo", frecuenciaAngular, GaussianWindow);
        var periodo = (2.0 * Math.PI) / Math.Abs(frecuenciaAngular);

        var actual = palabra.Fase(t);
        var desplazada = palabra.Fase(t + periodo);

        (actual - desplazada).Magnitude.Should().BeLessThan(1e-9);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 0.6065306597)]
    [InlineData(2.0, 0.1353352832)]
    [InlineData(4.0, 0.0003354626)]
    public void Constructor_UsaVentanaGaussiana_ParteRealEsperada(double t, double esperado)
    {
        var palabra = new Palabra("Yo", 1.0, GaussianWindow);

        palabra.Nombre.Ventana(t).Real.Should().BeApproximately(esperado, 1e-9);
        palabra.Nombre.Ventana(t).Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.5)]
    [InlineData(3.25)]
    public void Constructor_VentanaGaussiana_EsPar(double t)
    {
        var palabra = new Palabra("Yo", 1.0, GaussianWindow);

        var positiva = palabra.Nombre.Ventana(t);
        var negativa = palabra.Nombre.Ventana(-t);

        (positiva - negativa).Magnitude.Should().BeLessThan(1e-12);
    }

    [Theory]
    [InlineData(20.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Constructor_VentanaGaussiana_SeAtenuaFuertementeEnColas(double t)
    {
        var palabra = new Palabra("Yo", 1.0, GaussianWindow);

        palabra.Nombre.Ventana(t).Magnitude.Should().BeLessThan(1e-20);
    }

    [Theory]
    [InlineData(1e3)]
    [InlineData(-1e3)]
    public void Constructor_FaseEsNumericamenteEstableEnTiemposGrandes(double t)
    {
        var palabra = new Palabra("Yo", 3.7, GaussianWindow);
        var fase = palabra.Fase(t);

        double.IsFinite(fase.Real).Should().BeTrue();
        double.IsFinite(fase.Imaginary).Should().BeTrue();
    }

    [Theory]
    [InlineData(-4.0)]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(6.5)]
    public void CalcularVelocidadGrupo_UsaFormulaBase(double frecuenciaAngular)
    {
        var palabra = new Palabra("Yo", frecuenciaAngular, GaussianWindow);
        var esperado = frecuenciaAngular / (2.0 * Math.PI);

        palabra.CalcularVelocidadGrupo(frecuenciaAngular).Should().BeApproximately(esperado, 1e-12);
        palabra.Nombre.VelocidadGrupo.Should().BeApproximately(esperado, 1e-12);
    }

    [Fact]
    public void CalcularVelocidadGrupo_FormulaSobrescrita_SeReflejaEnNombre()
    {
        const double frecuenciaAngular = 3.0;
        var palabra = new PalabraConVelocidadGrupoLineal("Yo", frecuenciaAngular, GaussianWindow);
        var esperado = (0.5 * frecuenciaAngular) + 1.25;

        palabra.CalcularVelocidadGrupo(frecuenciaAngular).Should().BeApproximately(esperado, 1e-12);
        palabra.Nombre.VelocidadGrupo.Should().BeApproximately(esperado, 1e-12);
    }

    private sealed class PalabraConVelocidadGrupoLineal : Palabra
    {
        public PalabraConVelocidadGrupoLineal(string texto, double frecuenciaAngular, Func<double, Complex> ventana)
            : base(texto, frecuenciaAngular, ventana)
        {
        }

        public override double CalcularVelocidadGrupo(double frecuenciaAngular)
        {
            return (0.5 * frecuenciaAngular) + 1.25;
        }
    }

    private static Complex GaussianWindow(double t)
    {
        return new Complex(Math.Exp(-(t * t) / 2.0), 0.0);
    }
}
