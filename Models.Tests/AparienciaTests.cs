using System.Linq;

public class AparienciaTests
{
    [Fact]
    public void Aparecer_CreaDesignacionConNombresYEfectosSegunFaseMasCercana()
    {
        var apariencia = Apariencia.Aparecer(
            new List<string> { "ser humano", "ser lenguaje" },
            texto => texto == "ser humano" ? (-Math.PI / 2, 1.5d) : (Math.PI, 2.5d),
            frecuencia => frecuencia * 2);

        var designacion = Assert.IsType<Designacion>(apariencia);
        var nombres = designacion.Nombres.ToList();
        var efectoPrimerNombre = designacion.Efectos((Math.PI / 2, 3d));
        var efectoSegundoNombre = designacion.Efectos((Math.PI * 0.95, 3d));

        Assert.Equal(2, nombres.Count);
        Assert.Equal(Math.PI / 2, nombres[0].Fase, 10);
        Assert.Equal(Math.PI, nombres[1].Fase, 10);
        Assert.Equal(1.5d, designacion.Amplitud, 10);
        Assert.Equal(6d, designacion.VelocidadGrupo(3d), 10);
        Assert.Equal(1.5d, efectoPrimerNombre.Amplitud, 10);
        Assert.Equal(2.5d, efectoSegundoNombre.Amplitud, 10);
        Assert.Equal(Math.PI / 2, efectoPrimerNombre.Fase, 10);
    }

    [Fact]
    public void EqualsYGetHashCode_ComparanPorId()
    {
        var apariencia = new Apariencia(2d);
        var mismaReferencia = apariencia;
        var otra = new Apariencia(2d);

        Assert.True(apariencia.Equals(mismaReferencia));
        Assert.False(apariencia.Equals(otra));
        Assert.False(apariencia.Equals("no-apariencia"));
        Assert.Equal(apariencia.Id.GetHashCode(), apariencia.GetHashCode());
    }
}