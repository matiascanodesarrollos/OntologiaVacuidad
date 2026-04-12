using Moq;

public class AparienciaTests
{
    [Fact]
    public void Aparecer_CreaDesignacionConNombresYAmplitudTotal()
    {
        var predicados = new List<string> { "ser humano", "ser lenguaje" };

        var apariencia = Apariencia.Aparecer(predicados, texto =>
            texto switch
            {
                "ser humano" => (Math.PI * 3, 2d, 1.5d),
                "ser lenguaje" => (Math.PI / 2, 2d, 2.5d),
                _ => (0d, 0d, 0d)
            });

        var designacion = Assert.IsType<Designacion>(apariencia);
        var nombres = designacion.Nombres.Skip(1).ToList();

        Assert.Equal(4.0, designacion.Amplitud);
        Assert.Equal(2, nombres.Count);
        Assert.Equal(Math.PI, nombres[0].Fase);
        Assert.Equal(2, nombres[0].Frecuencia);
        Assert.Equal(1.5, nombres[0].ObtenerValor(2).Amplitud);
        Assert.Equal(2.5, nombres[1].ObtenerValor(2).Amplitud);
    }

    [Fact]
    public void Designar_ConAparienciaMockeada_NoModificaLaFuenteYRetornaSoloElNombreProyectado()
    {
        var nombre = ((Designacion)Apariencia.Aparecer(new List<string> { "decir verdad" }, _ => (0d, 1d, 3d)))
            .Nombres
            .Last();
        var apariencia = new Mock<Apariencia>(nombre, 1d) { CallBase = true };

        var resultado = new Designacion(new List<Nombre> { nombre }).Designar(apariencia.Object, nombre);

        var nombres = resultado.Nombres.ToList();
        Assert.Single(nombres);
        Assert.Same(nombre, nombres[0]);
    }
}