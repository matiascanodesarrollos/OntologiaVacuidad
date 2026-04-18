using FluentAssertions;

namespace Models.Tests;

public class DesignacionTests
{
    [Fact]
    public void Designar_SinFuncionObtenerVerboNucleo_CreaDesignacion()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var nombre = new Nombre("logos", fase, frecuencia);
        var predicadosApariencia = new List<string> { "gozo", "mente" };
        var texto = string.Join(". ", predicadosApariencia);
        var palabra = new Nombre("ser", 0, 0) as Palabra;
        var listaEsperada = new List<string> { "gozo", "mente", texto, "ser" };

        var designacionApariencia = nombre.Mostrarse(Apariencia.Mente, texto);        
        var designacion = designacionApariencia.Designar(nombre, palabra);

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(designacionApariencia.Nombres.Count() + 1);
        designacion.Nombres.Select(n => n.Texto).Should()
            .BeEquivalentTo(listaEsperada);
        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
        ultimoNombre.Fase.Should().Be(palabra.Fase);
        ultimoNombre.Texto.Should().Be(palabra.Texto);
        foreach(var nombreEnDesignacion in designacion.Nombres
            .Where(n => n.Texto != "Vacuidad")
            .SkipLast(1))
        {
            designacion.VelocidadGrupo(nombreEnDesignacion).Should().Be(1d);
        }
        for(int i = 0; i < 2; i++)
        {
            var nombreRandom = new Nombre($"random{i}", fase, frecuencia);
            designacion.VelocidadGrupo(nombreRandom).Should().Be(3d);
        }
    }

    [Fact]
    public void Equals_ConMismaReferencia_DevuelveTrueYConOtraDesignacion_DevuelveFalse()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var designacion = nombre.Mostrarse(Apariencia.Mente, "ser humano. pensar lenguaje");
        var mismaReferencia = designacion;
        var otra = nombre.Mostrarse(Apariencia.Mente, "ser mente. pensar vacuidad");

        designacion.Equals(mismaReferencia).Should().BeTrue();
        designacion.Equals(otra).Should().BeFalse();
        designacion.Equals("no-designacion").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SinParametros_GeneraPorId()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var designacion = nombre.Mostrarse(Apariencia.Mente, "ser humano. pensar lenguaje");

        designacion.GetHashCode().Should().Be(designacion.Id.GetHashCode());
    }

    [Fact]
    public void Valor_ConCualquierTiempo_ConservaLaSemanticaDeVacuidad()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var designacion = nombre.Mostrarse(Apariencia.Mente, "ser humano. pensar lenguaje");

        designacion.Funcion(0d).Should().Be((double.MaxValue, double.MaxValue));
        designacion.Funcion(0.5d).Should().Be((0d, 0d));
        designacion.Funcion(2d).Should().Be((0d, 0d));
    }

    [Fact]
    public void Designar_ConNombreIgualPorContenido_MantieneLaVelocidadDeGrupoExistente()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var nombre = new Nombre("logos", fase, frecuencia);
        var designacion = nombre.Mostrarse(Apariencia.Mente, "ser humano. pensar lenguaje");
        var nombreIgualPorContenido = new Nombre("ser humano", 0d, 1d);

        designacion.VelocidadGrupo(nombreIgualPorContenido).Should().Be(1d);
    }
}
