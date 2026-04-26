using FluentAssertions;

namespace Models.Tests;

public class DesignacionTests
{
    [Fact]
    public void Designar_SobreApariencia_AgregaElNombreALaDesignacionBase()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var amplitud = 2.0;
        var nombre = new Nombre("logos", fase, frecuencia, amplitud, Designacion.Vacuidad);
        var texto = "gozo. mente";
        var designacionApariencia = nombre.Mostrarse(texto);

        var designacion = Designacion.Designar(nombre, Apariencia.Aparecer(designacionApariencia));

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(designacionApariencia.Nombres.Count() + 1);
        designacion.Nombres.Last().Should().BeEquivalentTo(nombre);
    }

    [Fact]
    public void Equals_ConMismaReferencia_DevuelveTrueYConOtraDesignacion_DevuelveFalse()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0, 1.0, Designacion.Vacuidad);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje");
        var mismaReferencia = designacion;
        var otra = nombre.Mostrarse("ser mente. pensar vacuidad");

        designacion.Equals(mismaReferencia).Should().BeTrue();
        designacion.Equals(otra).Should().BeFalse();
        designacion.Equals("no-designacion").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SinParametros_GeneraPorId()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0, 1.0, Designacion.Vacuidad);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje");

        designacion.GetHashCode().Should().Be(designacion.Id.GetHashCode());
    }

    [Fact]
    public void Efecto_ExponeNombreAgregadoYLaAparienciaDeLaDesignacion()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0, 2.0, Designacion.Vacuidad);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje");

        var efecto = designacion.Efecto;

        efecto.Nombre.Texto.Should().Be("Vacuidad");
        efecto.Nombre.Frecuencia.Should().Be(0d);
        efecto.Nombre.Causa.Should().Be(designacion);
        efecto.Nombre.Amplitud.Should().BeApproximately(designacion.Nombres.Sum(n => n.Amplitud), 1e-10);
        efecto.Apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
    }

    [Fact]
    public void VelocidadGrupo_UsaLaCantidadDeNombresConLaMismaFrecuencia()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0, 1.0, Designacion.Vacuidad);
        var designacion = nombre.Mostrarse("ser humano. ser lenguaje. pensar mente");

        var frecuencias = designacion.Nombres.Select(n => designacion.VelocidadGrupo(n)).ToList();

        frecuencias.Should().Equal(2d, 2d, 1d, 1d);
        designacion.VelocidadGrupo(new Nombre("ajeno", 0d, 99d, 1d, Designacion.Vacuidad)).Should().Be(0d);
    }
}
