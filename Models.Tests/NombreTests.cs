using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class NombreTests
{
    [Fact]
    public void Constructor_ExponeValoresEsperados()
    {
        var nombre = new Nombre("Vacuidad", 0.0, UnitStepTransform);
        var tf = nombre.Ventana(2.0);

        nombre.Texto.Should().Be("Vacuidad");
        nombre.VelocidadGrupo.Should().Be(0.0);
        tf.Real.Should().Be(1.0);
        tf.Imaginary.Should().Be(0.0);
    }

    [Fact]
    public void Constructor_CreaNuevasInstanciasConMismaSemantica()
    {
        var primero = new Nombre("Vacuidad", 0.0, UnitStepTransform);
        var segundo = new Nombre("Vacuidad", 0.0, UnitStepTransform);

        primero.Should().NotBeSameAs(segundo);
        primero.Texto.Should().Be(segundo.Texto);
        primero.VelocidadGrupo.Should().Be(segundo.VelocidadGrupo);
        primero.Ventana(1.5).Should().Be(segundo.Ventana(1.5));
    }

    [Theory]
    [InlineData(-2.0)]
    [InlineData(-0.5)]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(2.0)]
    public void Constructor_VentanaEsEscalonUnitario(double omega)
    {
        var nombre = new Nombre("Vacuidad", 0.0, UnitStepTransform);
        var tf = nombre.Ventana(omega);

        tf.Real.Should().Be(omega >= 0.0 ? 1.0 : 0.0);
        tf.Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Fact]
    public void ToString_ContieneTextoYVelocidadDeGrupo()
    {
        var nombre = new Nombre("Vacuidad", 0.0, UnitStepTransform);

        nombre.ToString().Should().Be($"{nombre.Texto} (VelocidadGrupo: {nombre.VelocidadGrupo})");
    }

    [Fact]
    public void Equals_ComparaPorTexto()
    {
        var primero = new Nombre("Vacuidad", 0.0, UnitStepTransform);
        var segundo = new Nombre("Vacuidad", 0.0, UnitStepTransform);

        primero.Equals(segundo).Should().BeTrue();
        primero.Equals("otro").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_UsaTextoComoBase()
    {
        var nombre = new Nombre("Vacuidad", 0.0, UnitStepTransform);

        nombre.GetHashCode().Should().Be(nombre.Texto.GetHashCode());
    }

    [Fact]
    public void Equals_ConNullYTiposDistintos_DevuelveFalse()
    {
        var nombre = new Nombre("Vacuidad", 0.0, UnitStepTransform);
        object? nulo = null;
        var otro = new object();

        var esNulo = nombre.Equals(nulo);
        var esOtro = nombre.Equals(otro);

        esNulo.Should().BeFalse();
        esOtro.Should().BeFalse();
    }

    [Fact]
    public void Mostrarse_CreaAparienciaConEsenciaConsistente()
    {
        var nombre = new Nombre("Vacuidad", 0.0, SmoothSpectrum);
        var apariencia = nombre.Mostrarse(1.2);

        apariencia.Esencia.Should().NotBeNull();
        apariencia.Esencia.Texto.Should().Be(nombre.Texto);
    }

    [Fact]
    public void Mostrarse_CreaNuevasInstanciasEnCadaInvocacion()
    {
        var nombre = new Nombre("Vacuidad", 0.0, SmoothSpectrum);

        var a1 = nombre.Mostrarse(0.3);
        var a2 = nombre.Mostrarse(0.3);

        a1.Should().NotBeSameAs(a2);
        a1.Id.Should().NotBe(a2.Id);
    }

    private static Complex UnitStepTransform(double omega)
    {
        return omega >= 0.0 ? Complex.One : Complex.Zero;
    }

    private static Complex SmoothSpectrum(double omega)
    {
        return new Complex(Math.Exp(-(omega * omega)), 0.0);
    }
}
