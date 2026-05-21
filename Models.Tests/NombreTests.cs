using FluentAssertions;

namespace Models.Tests;

public class NombreTests
{
    [Fact]
    public void Vacuidad_ExponeValoresEsperados()
    {
        var nombre = Nombre.Vacuidad;
        var tf = nombre.TransformadaFourier(2.0);

        nombre.Texto.Should().Be(nameof(Nombre.Vacuidad));
        nombre.VelocidadGrupo.Should().Be(0.0);
        tf.Real.Should().Be(1.0);
        tf.Imaginary.Should().Be(0.0);
    }

    [Fact]
    public void Vacuidad_CreaNuevasInstanciasConMismaSemantica()
    {
        var primero = Nombre.Vacuidad;
        var segundo = Nombre.Vacuidad;

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
    public void Vacuidad_TransformadaEsEscalonUnitario(double omega)
    {
        var tf = Nombre.Vacuidad.TransformadaFourier(omega);

        tf.Real.Should().Be(omega >= 0.0 ? 1.0 : 0.0);
        tf.Imaginary.Should().BeApproximately(0.0, 1e-12);
    }

    [Fact]
    public void ToString_ContieneTextoYVelocidadDeGrupo()
    {
        var nombre = Nombre.Vacuidad;

        nombre.ToString().Should().Be($"{nombre.Texto} (VelocidadGrupo: {nombre.VelocidadGrupo})");
    }

    [Fact]
    public void Equals_ComparaPorTexto()
    {
        var primero = Nombre.Vacuidad;
        var segundo = Nombre.Vacuidad;

        primero.Equals(segundo).Should().BeTrue();
        primero.Equals("otro").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_UsaTextoComoBase()
    {
        var nombre = Nombre.Vacuidad;

        nombre.GetHashCode().Should().Be(nombre.Texto.GetHashCode());
    }

    [Fact]
    public void Equals_ConNullYTiposDistintos_DevuelveFalse()
    {
        var nombre = Nombre.Vacuidad;

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
        var apariencia = Nombre.Vacuidad.Mostrarse(1.2);
        var valor = apariencia.Funcion(t);

        apariencia.Texto.Should().Be(Nombre.Vacuidad.Texto);
        apariencia.FrecuenciaAngular.Should().BeApproximately(1.2, 1e-12);
        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void Mostrarse_ConFrecuenciaAngularCero_MantieneMagnitudEstableConTiempo()
    {
        var apariencia = Nombre.Vacuidad.Mostrarse(0.0);
        var v1 = apariencia.Funcion(0.0);
        var v2 = apariencia.Funcion(2.0);

        v1.Magnitude.Should().BeApproximately(v2.Magnitude, 1e-9);
    }

    [Fact]
    public void Mostrarse_CreaNuevasInstanciasEnCadaInvocacion()
    {
        var nombre = Nombre.Vacuidad;

        var a1 = nombre.Mostrarse(0.3);
        var a2 = nombre.Mostrarse(0.3);

        a1.Should().NotBeSameAs(a2);
        a1.Id.Should().NotBe(a2.Id);
    }
}
