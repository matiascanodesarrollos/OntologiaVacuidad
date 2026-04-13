public class NombreTests
{
    [Fact]
    public void ObtenerValor_RetornaCeroCuandoLaFrecuenciaNoExiste()
    {
        var nombre = ((Designacion)Apariencia.Aparecer(new List<string> { "pensar mundo" }, _ => (0.5d, 3d, 2d)))
            .Apariencias
            .Last();

        var valor = nombre.ObtenerValor(99);

        Assert.Equal(0, valor.Amplitud);
        Assert.Equal(nombre.Fase, valor.Fase);
    }

    [Fact]
    public void Mostrarse_ConDesignacionDevuelveNuevaDesignacionYAgregaNuevaFrecuencia()
    {
        var fuente = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser humano", "decir verdad" },
            texto => texto == "ser humano" ? (0d, 1d, 1d) : (1d, 2d, 4d));
        var nombre = fuente.Apariencias.Last();

        var resultado = nombre.Mostrarse(fuente);

        Assert.Equal(3, nombre.Frecuencia);
        Assert.Equal(fuente, nombre.Efecto[3].Single());
        Assert.Contains(nombre, resultado.Nombres);
        Assert.Equal(5, resultado.Nombres.Count());
    }
}