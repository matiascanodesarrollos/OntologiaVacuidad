using DomainLogic.Services;

public class AmbienteConfigTests
{
    [Fact]
    public void CrearAmbiente_ConvierteTextoEnUnaDesignacionSinNombresNulos()
    {
        var ambiente = AmbienteConfig.CrearAmbiente("ser humano. ser lenguaje. pensar humano.");
        var nombres = ambiente.Nombres.ToList();

        Assert.Equal(4, nombres.Count);
        Assert.Equal(0d, nombres[0].Fase, 10);
        Assert.Equal(2.0943951024, nombres[1].Fase, 10);
        Assert.Equal(4.1887902048, nombres[2].Fase, 10);
        Assert.Equal("pensar humano", nombres[2].Texto);
        Assert.Equal("pensar humano", nombres[3].Texto);
        Assert.DoesNotContain(nombres, n => n.Texto is null);
        Assert.Equal((1d, 0d), nombres[0].Esencia.Valor(0d));
        Assert.Equal((1d, 0d), nombres[1].Esencia.Valor(0d));
        Assert.Equal((1d, 0d), nombres[2].Esencia.Valor(0d));
        Assert.Equal(1d, ambiente.VelocidadGrupo(nombres[0]), 10);
    }
}