using DomainLogic.Services;

public class AmbienteConfigTests
{
    [Fact]
    public void CrearAmbiente_ConvierteTextoEnUnaDesignacionConFasesYAmplitudesEsperadas()
    {
        var ambiente = AmbienteConfig.CrearAmbiente("ser humano. ser lenguaje. pensar humano.");
        var nombres = ambiente.Nombres.ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal(0d, nombres[0].Fase, 10);
        Assert.Equal(2 * Math.PI / 3, nombres[1].Fase, 10);
        Assert.Equal(0d, nombres[2].Fase, 10);
        Assert.Equal("ser humano. ser lenguaje. pensar humano.", nombres[2].Texto);
        Assert.Equal(2d, nombres[0].Esencia.Amplitud(0d), 10);
        Assert.Equal(0.3660254038d, nombres[1].Esencia.Amplitud(0d), 10);
        Assert.Equal(double.MaxValue, nombres[2].Esencia.Amplitud(0d));
        Assert.Equal(1d, ambiente.VelocidadGrupo(nombres[0]), 10);
    }
}