using DomainLogic.Services;

public class AmbienteConfigTests
{
    [Fact]
    public void CrearAmbiente_GeneraFrecuenciasFasesYAmplitudesEsperadas()
    {
        var ambiente = AmbienteConfig.CrearAmbiente("ser humano. ser lenguaje. pensar humano.");
        var nombres = ambiente.Apariencias.Skip(1).ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal(0, nombres[0].Fase);
        Assert.Equal(2 * Math.PI / 3, nombres[1].Fase, 10);
        Assert.Equal(4 * Math.PI / 3, nombres[2].Fase, 10);
        Assert.Equal(2, nombres[0].Frecuencia);
        Assert.Equal(2, nombres[1].Frecuencia);
        Assert.Equal(1, nombres[2].Frecuencia);
        Assert.Equal(2, nombres[0].ObtenerValor(2).Amplitud);
        Assert.Equal(1, nombres[1].ObtenerValor(2).Amplitud);
        Assert.Equal(2, nombres[2].ObtenerValor(1).Amplitud);
    }
}