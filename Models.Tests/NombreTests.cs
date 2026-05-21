using FluentAssertions;

namespace Models.Tests;

public class NombreTests
{
    [Fact]
    public void Cuerpo_ExponeValoresEsperados()
    {
        var nombre = Nombre.Cuerpo;
        var tf = nombre.TransformadaFourier(2.0);

        nombre.Texto.Should().Be(nameof(Designacion.Vacuidad));
        nombre.VelocidadGrupo.Should().Be(0.0);
        tf.Real.Should().Be(0.0);
        tf.Imaginary.Should().BeApproximately(Math.PI, 1e-12);
    }

    [Fact]
    public void Cuerpo_CreaNuevasInstanciasConMismaSemantica()
    {
        var primero = Nombre.Cuerpo;
        var segundo = Nombre.Cuerpo;

        primero.Should().NotBeSameAs(segundo);
        primero.Texto.Should().Be(segundo.Texto);
        primero.VelocidadGrupo.Should().Be(segundo.VelocidadGrupo);
        primero.TransformadaFourier(1.5).Should().Be(segundo.TransformadaFourier(1.5));
    }

    [Theory]
    [InlineData(-2.0)]
    [InlineData(-0.5)]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(2.0)]
    public void Cuerpo_TransformadaEsPuramenteImaginaria(double omega)
    {
        var tf = Nombre.Cuerpo.TransformadaFourier(omega);

        tf.Real.Should().BeApproximately(0.0, 1e-12);
        tf.Imaginary.Should().BeApproximately((omega / 2.0) * Math.PI, 1e-12);
    }

    [Fact]
    public void ToString_ContieneTextoYVelocidadDeGrupo()
    {
        var nombre = Nombre.Cuerpo;

        nombre.ToString().Should().Be($"{nombre.Texto} (VelocidadGrupo: {nombre.VelocidadGrupo})");
    }

    [Fact]
    public void Equals_ComparaPorTexto()
    {
        var primero = Nombre.Cuerpo;
        var segundo = Nombre.Cuerpo;

        primero.Equals(segundo).Should().BeTrue();
        primero.Equals("otro").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_UsaTextoComoBase()
    {
        var nombre = Nombre.Cuerpo;

        nombre.GetHashCode().Should().Be(nombre.Texto.GetHashCode());
    }

    [Fact]
    public void Equals_ConNullYTiposDistintos_DevuelveFalse()
    {
        var nombre = Nombre.Cuerpo;

        nombre.Equals(null).Should().BeFalse();
        nombre.Equals(new object()).Should().BeFalse();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.25)]
    [InlineData(1.0)]
    [InlineData(3.0)]
    public void Mostrarse_CreaAparienciaConFuncionComplejaFinita(double t)
    {
        var apariencia = Nombre.Cuerpo.Mostrarse(1.2);
        var valor = apariencia.Funcion(t);

        apariencia.Texto.Should().Be(Nombre.Cuerpo.Texto);
        apariencia.Frecuencia.Should().BeApproximately(1.2, 1e-12);
        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void Mostrarse_ConFrecuenciaCero_MantieneMagnitudEstableConTiempo()
    {
        var apariencia = Nombre.Cuerpo.Mostrarse(0.0);
        var v1 = apariencia.Funcion(0.0);
        var v2 = apariencia.Funcion(2.0);

        v1.Magnitude.Should().BeApproximately(v2.Magnitude, 1e-9);
    }

    [Fact]
    public void Mostrarse_CreaNuevasInstanciasEnCadaInvocacion()
    {
        var nombre = Nombre.Cuerpo;

        var a1 = nombre.Mostrarse(0.3);
        var a2 = nombre.Mostrarse(0.3);

        a1.Should().NotBeSameAs(a2);
        a1.Id.Should().NotBe(a2.Id);
    }
}
