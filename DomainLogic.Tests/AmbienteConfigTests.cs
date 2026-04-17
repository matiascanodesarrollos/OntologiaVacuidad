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
        Assert.Equal(2.0943951024, nombres[1].Fase, 10);
        Assert.Equal(4.1887902048, nombres[2].Fase, 10);
        Assert.Equal("pensar humano", nombres[2].Texto);
        Assert.Equal((2d, 0d), nombres[0].Esencia.Amplitud(0d));
        Assert.Equal((-0.49999999999999978, 1.7320508075688774), nombres[1].Esencia.Amplitud(0d));
        Assert.Equal((1.7976931348623157E+308, 1.7976931348623157E+308), nombres[2].Esencia.Amplitud(0d));
        Assert.Equal(1d, ambiente.VelocidadGrupo(nombres[0]), 10);
    }
}