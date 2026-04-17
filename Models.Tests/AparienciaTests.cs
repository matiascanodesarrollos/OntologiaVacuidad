public class AparienciaTests
{
    [Fact]
    public void Aparecer_SumaLasAmplitudesDeLaDesignacionEnElTiempoDado()
    {
        var designacion = new Nombre("observador", 0d, 1d, null)
            .Mostrarse(null, new List<string> { "ser humano", "pensar humano" });

        var apariencia = Apariencia.Aparecer(designacion);
        var amplitudEsperada = designacion.Nombres
            .Select(n => n.Esencia.Amplitud(0.5d))
            .Aggregate((a, b) => (a.Item1 + b.Item1, a.Item2 + b.Item2));
        
        Assert.Equal(amplitudEsperada, apariencia.Amplitud(0.5d));
    }

    [Fact]
    public void EqualsYGetHashCode_ComparanPorId()
    {
        var designacion = new Nombre("observador", 0d, 1d, null)
            .Mostrarse(null, new List<string> { "ser humano", "pensar humano" });
        var apariencia = Apariencia.Aparecer(designacion);
        var mismaReferencia = apariencia;
        var otra = Apariencia.Aparecer(designacion);

        Assert.True(apariencia.Equals(mismaReferencia));
        Assert.False(apariencia.Equals(otra));
        Assert.False(apariencia.Equals("no-apariencia"));
        Assert.Equal(apariencia.Id.GetHashCode(), apariencia.GetHashCode());
    }
}