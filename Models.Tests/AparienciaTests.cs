public class AparienciaTests
{
    [Fact]
    public void Aparecer_UsaComoEsenciaLosNombresDeLaDesignacion()
    {
        var designacion = new Nombre("observador", 0d, 1d)
            .Mostrarse(Apariencia.Vacuidad, new List<string> { "ser humano", "pensar humano" });

        var apariencia = Apariencia.Aparecer(designacion);

        Assert.Equal(designacion.Nombres, apariencia.Esencia);
    }

    [Fact]
    public void EqualsYGetHashCode_ComparanPorId()
    {
        var designacion = new Nombre("observador", 0d, 1d)
            .Mostrarse(Apariencia.Vacuidad, new List<string> { "ser humano", "pensar humano" });
        var apariencia = Apariencia.Aparecer(designacion);
        var mismaReferencia = apariencia;
        var otra = Apariencia.Aparecer(designacion);

        Assert.True(apariencia.Equals(mismaReferencia));
        Assert.False(apariencia.Equals(otra));
        Assert.False(apariencia.Equals("no-apariencia"));
        Assert.Equal(apariencia.Id.GetHashCode(), apariencia.GetHashCode());
    }

    [Fact]
    public void Vacuidad_DevuelveMaximoEnTiempoCeroYCeroFueraDeEseInstante()
    {
        Assert.Equal((double.MaxValue, double.MaxValue), Apariencia.Vacuidad.Valor(0d));
        Assert.Equal((0d, 0d), Apariencia.Vacuidad.Valor(1d));
        Assert.Single(Apariencia.Vacuidad.Esencia);
        Assert.False(string.IsNullOrWhiteSpace(Apariencia.Vacuidad.Esencia.Single().Texto));
    }
}