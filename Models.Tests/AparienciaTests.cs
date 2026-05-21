using FluentAssertions;

namespace Models.Tests;

public class AparienciaTests
{
    [Fact]
    public void Mente_SeInicializaSinExcepciones()
    {
        var mente = Apariencia.Mente;

        mente.Should().NotBeNull();
        mente.Texto.Should().Be("Mente");
        mente.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Aparecer_CombinaTextoYCalculaFuncionCompleja()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0), Palabra.Yo(2.0) });
        var valor = apariencia.Funcion(0.25);

        apariencia.Should().NotBeNull();
        apariencia.Texto.Should().Be("Yo Yo");
        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
        valor.Magnitude.Should().BePositive();
    }

    [Fact]
    public void Amplitud_CoincideConTransformadaFourierDeVentanaGaussianaEnFrecuenciaAngularCero()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(0.0) });
        var esperado = Math.Sqrt(2.0 * Math.PI);

        apariencia.Amplitud.Should().BeApproximately(esperado, 1e-3);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.1)]
    [InlineData(0.37)]
    [InlineData(1.25)]
    public void Funcion_MagnitudCoincideConAmplitud(double t)
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var valor = apariencia.Funcion(t);

        valor.Magnitude.Should().BeApproximately(apariencia.Amplitud, 1e-9);
    }

    [Fact]
    public void Funcion_CambiaFaseConTiempoSinCambiarMagnitud()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(2.0) });
        var valor1 = apariencia.Funcion(0.1);
        var valor2 = apariencia.Funcion(0.35);

        valor1.Magnitude.Should().BeApproximately(valor2.Magnitude, 1e-9);
        (valor1 - valor2).Magnitude.Should().BeGreaterThan(1e-8);
    }

    [Fact]
    public void Aparecer_ListaVacia_NoFallaYEntregaSenalDefinida()
    {
        var apariencia = Apariencia.Aparecer(Array.Empty<Palabra>());
        var valor = apariencia.Funcion(0.2);

        apariencia.Texto.Should().BeEmpty();
        apariencia.FrecuenciaAngular.Should().Be(0.0);
        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
        apariencia.Amplitud.Should().BePositive();
    }

    [Fact]
    public void Aparecer_PalabraUnica_PreservaTextoYFrecuenciaAngular()
    {
        var palabra = Palabra.Yo(3.0);

        var apariencia = Apariencia.Aparecer(new[] { palabra });

        apariencia.Texto.Should().Be("Yo");
        apariencia.FrecuenciaAngular.Should().BeApproximately(3.0, 1e-12);
    }

    [Theory]
    [InlineData(-5.0)]
    [InlineData(-0.5)]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(5.0)]
    public void Funcion_ProduceValoresFinitosEnRangoAmplioDeTiempo(double t)
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.2), Palabra.Yo(-0.8) });
        var valor = apariencia.Funcion(t);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void Equals_YHashCode_SiguenSemanticaPorId()
    {
        var a1 = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var a2 = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });

        a1.Should().NotBeNull();
        a1.Equals(a1).Should().BeTrue();
        a1.Equals(a2).Should().BeFalse();
        a1.Equals(null).Should().BeFalse();
        var hash = a1?.GetHashCode();
        var idHash = a1?.Id.GetHashCode();
        hash.Should().Be(idHash);
    }

    [Fact]
    public void Funcion_ParaFrecuenciaAngularCero_ConservaFaseConstante()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(0.0) });
        var valor1 = apariencia.Funcion(0.0);
        var valor2 = apariencia.Funcion(0.73);

        (valor1 - valor2).Magnitude.Should().BeLessThan(1e-9);
    }
}