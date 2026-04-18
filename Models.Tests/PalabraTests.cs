using FluentAssertions;

namespace Models.Tests;

public class PalabraTests
{
    [Fact]
    public void Constructor_ConTexto_Crea()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 1.0);
        var palabra = nombre as Palabra;

        palabra.Should().NotBeNull();
        palabra!.Texto.Should().Be("logos");
        palabra.Fase.Should().Be(Math.PI / 3);
        palabra.FuncionFaseInstanea.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_SinTexto_Crea()
    {
        var nombre = new Nombre(null, Math.PI / 3, 1.0);
        var palabra = nombre as Palabra;

        palabra.Should().NotBeNull();
        palabra.Texto.Should().Be("Vacuidad");
    }

    [Fact]
    public void Constructor_ConFaseMayor360_NormalizaFase()
    {
        var fase = 400.0;
        var nombre = new Nombre("logos", fase, 1.0);
        var palabra = nombre as Palabra;
        var faseEsperada = fase % (2 * Math.PI);

        palabra.Should().NotBeNull();
        palabra!.Fase.Should().Be(faseEsperada);
        palabra.FuncionFaseInstanea.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ConFaseNegativa_NormalizaFaseUsandoValorAbsoluto()
    {
        var fase = -Math.PI / 3;
        var nombre = new Nombre("logos", fase, 1.0);
        var palabra = nombre as Palabra;

        palabra.Should().NotBeNull();
        palabra!.Fase.Should().BeApproximately(Math.PI / 3, 1e-10);
    }
}
