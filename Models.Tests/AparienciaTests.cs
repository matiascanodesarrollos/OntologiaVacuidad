using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class AparienciaTests
{
    [Fact]
    public void Aparecer_SeInicializaSinExcepciones()
    {
        var mente = Apariencia.Aparecer(new[] { new Palabra("Mente", 0.0, GaussianWindow) });

        mente.Should().NotBeNull();
        mente.Esencia.Should().NotBeNull();
        mente.Esencia.Texto.Should().Be("Mente");
    }

    [Fact]
    public void Aparecer_CombinaTextoEnEsencia()
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow), new Palabra("Yo", 2.0, GaussianWindow) });

        apariencia.Should().NotBeNull();
        apariencia.Esencia.Should().NotBeNull();
        apariencia.Esencia.Texto.Should().Be("Yo Yo");
    }

    [Fact]
    public void Amplitud_CoincideConTransformadaFourierDeVentanaGaussianaEnFrecuenciaAngularCero()
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", 0.0, GaussianWindow) });
        var esperado = Math.Sqrt(2.0 * Math.PI);

        apariencia.Amplitud.Should().BeApproximately(esperado, 1e-3);
    }

    [Fact]
    public void Aparecer_ListaVacia_NoFallaYEntregaEsenciaDefinida()
    {
        var apariencia = Apariencia.Aparecer(Array.Empty<Palabra>());

        apariencia.Esencia.Should().NotBeNull();
        apariencia.Esencia.Texto.Should().BeEmpty();
        apariencia.Amplitud.Should().BePositive();
    }

    [Fact]
    public void Aparecer_PalabraUnica_PreservaTextoEnEsencia()
    {
        var palabra = new Palabra("Yo", 3.0, GaussianWindow);

        var apariencia = Apariencia.Aparecer(new[] { palabra });

        apariencia.Esencia.Texto.Should().Be("Yo");
        apariencia.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Equals_YHashCode_SiguenSemanticaPorId()
    {
        var a1 = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow) });
        var a2 = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow) });

        a1.Should().NotBeNull();
        a1.Equals(a1).Should().BeTrue();
        a1.Equals(a2).Should().BeFalse();
        a1.Equals(null).Should().BeFalse();
        var hash = a1?.GetHashCode();
        var idHash = a1?.Id.GetHashCode();
        hash.Should().Be(idHash);
    }

    [Fact]
    public void ConstructorDeCopia_PreservaIdYEsencia()
    {
        var original = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow) });

        var copia = new Apariencia(original);

        copia.Id.Should().Be(original.Id);
        copia.Esencia.Should().BeSameAs(original.Esencia);
        copia.Amplitud.Should().BeApproximately(original.Amplitud, 1e-12);
    }

    [Fact]
    public void Mente_EstaDisponibleYTieneEsenciaAsociada()
    {
        var mente = Apariencia.Mente;

        mente.Should().NotBeNull();
        mente.Esencia.Should().NotBeNull();
        mente.Esencia.Texto.Should().Be(nameof(Apariencia.Mente));
    }

    [Fact]
    public void Funcion_EstaAsignadaEnAparienciaBase()
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow) });

        apariencia.Funcion.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.25)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    public void Funcion_ProduceValoresFinitosConMagnitudIgualAAmplitud(double t)
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, GaussianWindow) });

        var valor = apariencia.Funcion(t);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
        valor.Magnitude.Should().BeApproximately(apariencia.Amplitud, 1e-9);
    }

    [Theory]
    [MemberData(nameof(VentanasVariadas))]
    public void Aparecer_ConVentanasVariadas_ProduceAmplitudFinita(
        string _,
        Func<double, Complex> ventana)
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", 1.0, ventana) });

        double.IsFinite(apariencia.Amplitud).Should().BeTrue();
        apariencia.Amplitud.Should().BeGreaterThanOrEqualTo(0.0);
    }

    [Theory]
    [InlineData(-12.0)]
    [InlineData(-4.0)]
    [InlineData(0.0)]
    [InlineData(4.0)]
    [InlineData(12.0)]
    public void Aparecer_ConFrecuenciasAmplias_MantieneAmplitudFinita(double frecuenciaAngular)
    {
        var apariencia = Apariencia.Aparecer(new[] { new Palabra("Yo", frecuenciaAngular, GaussianWindow) });

        double.IsFinite(apariencia.Amplitud).Should().BeTrue();
        apariencia.Amplitud.Should().BeGreaterThanOrEqualTo(0.0);
    }

    [Fact]
    public void Aparecer_EscalaLinealConGananciaDeVentana()
    {
        var baseApariencia = Apariencia.Aparecer(new[]
        {
            new Palabra("Yo", 0.5, GaussianWindow)
        });

        var escalada = Apariencia.Aparecer(new[]
        {
            new Palabra("Yo", 0.5, t => 2.0 * GaussianWindow(t))
        });

        escalada.Amplitud.Should().BeApproximately(2.0 * baseApariencia.Amplitud, 1e-3);
    }

    public static IEnumerable<object[]> VentanasVariadas()
    {
        yield return new object[] { "gaussiana", (Func<double, Complex>)GaussianWindow };
        yield return new object[] { "cola-larga", (Func<double, Complex>)(t => new Complex(1.0 / (1.0 + (t * t)), 0.0)) };
        yield return new object[] { "compacta", (Func<double, Complex>)(t => new Complex(Math.Abs(t) <= 1.0 ? 1.0 : 0.0, 0.0)) };
        yield return new object[] { "compleja-oscilatoria", (Func<double, Complex>)(t => Complex.FromPolarCoordinates(Math.Exp(-(t * t) / 2.0), 0.7 * t)) };
    }

    private static Complex GaussianWindow(double t)
    {
        return new Complex(Math.Exp(-(t * t) / 2.0), 0.0);
    }
}