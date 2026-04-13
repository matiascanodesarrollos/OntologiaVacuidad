using DomainLogic.Services;
using System.Linq;

public class AmbienteConfigTests
{
    [Fact]
    public void CrearAmbiente_GeneraFasesAmplitudesYVelocidadEsperadas()
    {
        var ambiente = AmbienteConfig.CrearAmbiente("ser humano. ser lenguaje. pensar humano.");
        var nombres = ambiente.Nombres.ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal(2 * Math.PI / 3, nombres[0].Fase, 10);
        Assert.Equal(4 * Math.PI / 3, nombres[1].Fase, 10);
        Assert.Equal(0, nombres[2].Fase, 10);
        Assert.Equal(2d, nombres[0].Esencia.Amplitud, 10);
        Assert.Equal(1d, nombres[1].Esencia.Amplitud, 10);
        Assert.Equal(2d, nombres[2].Esencia.Amplitud, 10);
        Assert.Equal(1d, ambiente.VelocidadGrupo(99d), 10);
        Assert.Equal(2d, ambiente.Efectos((2 * Math.PI / 3, 5d)).Amplitud, 10);
    }
}