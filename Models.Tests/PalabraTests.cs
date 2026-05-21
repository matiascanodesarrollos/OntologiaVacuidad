using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class PalabraTests
{
    [Fact]
    public void Yo_CreaPalabraConTextoEsperado()
    {
        var palabra = Palabra.Yo(2.0);

        palabra.Texto.Should().Be("Yo");
        palabra.Id.Should().NotBe(Guid.Empty);
        palabra.FrecuenciaAngular.Should().Be(2.0);
    }

    [Theory]
    [InlineData(2.0, 0.25)]
    [InlineData(1.5, 0.2)]
    [InlineData(-3.0, 0.125)]
    [InlineData(0.0, 10.0)]
    public void Yo_UsaFaseComplejaUnitariaConFrecuenciaAngularIndicada(double frecuenciaAngular, double t)
    {
        var palabra = Palabra.Yo(frecuenciaAngular);
        var esperado = Complex.FromPolarCoordinates(1.0, frecuenciaAngular * t);

        palabra.Fase(t).Magnitude.Should().BeApproximately(1.0, 1e-12);
        palabra.Fase(t).Real.Should().BeApproximately(esperado.Real, 1e-12);
        palabra.Fase(t).Imaginary.Should().BeApproximately(esperado.Imaginary, 1e-12);
    }

    [Theory]
    [InlineData(0.25)]
    [InlineData(0.75)]
    [InlineData(10.0)]
    public void Yo_ConFrecuenciaAngularCero_DevuelveFaseConstante(double t)
    {
        var palabra = Palabra.Yo(0.0);

        palabra.Fase(t).Real.Should().BeApproximately(1.0, 1e-12);
        palabra.Fase(t).Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Theory]
    [InlineData(1.0, 0.25)]
    [InlineData(2.5, 0.1)]
    [InlineData(-0.75, 0.4)]
    public void Yo_RespetaPeriodicidadDeLaFase(double frecuenciaAngular, double t)
    {
        var palabra = Palabra.Yo(frecuenciaAngular);
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
    public void Yo_UsaVentanaGaussiana_ParteRealEsperada(double t, double esperado)
    {
        var palabra = Palabra.Yo(1.0);

        palabra.Ventana(t).Real.Should().BeApproximately(esperado, 1e-9);
        palabra.Ventana(t).Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.5)]
    [InlineData(3.25)]
    public void Yo_VentanaGaussiana_EsPar(double t)
    {
        var palabra = Palabra.Yo(1.0);

        var positiva = palabra.Ventana(t);
        var negativa = palabra.Ventana(-t);

        (positiva - negativa).Magnitude.Should().BeLessThan(1e-12);
    }

    [Theory]
    [InlineData(20.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Yo_VentanaGaussiana_SeAtenuaFuertementeEnColas(double t)
    {
        var palabra = Palabra.Yo(1.0);

        palabra.Ventana(t).Magnitude.Should().BeLessThan(1e-20);
    }

    [Theory]
    [InlineData(1e3)]
    [InlineData(-1e3)]
    public void Yo_FaseEsNumericamenteEstableEnTiemposGrandes(double t)
    {
        var palabra = Palabra.Yo(3.7);
        var fase = palabra.Fase(t);

        double.IsFinite(fase.Real).Should().BeTrue();
        double.IsFinite(fase.Imaginary).Should().BeTrue();
    }
}
