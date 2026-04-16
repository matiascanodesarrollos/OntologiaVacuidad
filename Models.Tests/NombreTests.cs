public class NombreTests
{
    [Fact]
    public void Mostrarse_ConPredicadosCreaDesignacionYAgregaElNombreActual()
    {
        var nombre = new Nombre("logos", Math.PI / 3, null);

        var resultado = nombre.Mostrarse(null, new List<string> { "ser humano", "ser lenguaje" });
        var nombres = resultado.Nombres.ToList();

        Assert.Equal(2, nombres.Count);
        Assert.Equal("ser humano", nombres[0].Texto);
        Assert.Same(nombre, nombres[1]);
        Assert.Equal(1d, resultado.VelocidadGrupo(nombre), 10);
    }

    [Fact]
    public void Mostrarse_ConDesignacionYSinPredicadosReutilizaLaDesignacionExistente()
    {
        var baseDesignacion = new Nombre("origen", 0d, null)
            .Mostrarse(null, new List<string> { "ser humano", "pensar humano" });
        var nombre = new Nombre("logos", Math.PI / 2, null);

        var resultado = nombre.Mostrarse(baseDesignacion, null);
        var nombres = resultado.Nombres.ToList();

        Assert.Equal(2, nombres.Count);
        Assert.Equal(baseDesignacion.Nombres.First().Texto, nombres[0].Texto);
        Assert.Same(nombre, nombres[1]);
    }

    [Fact]
    public void Mostrarse_SinPredicadosNiDesignacionLanzaErrorClaro()
    {
        var nombre = new Nombre("logos", 0d, null);

        var error = Assert.Throws<ArgumentNullException>(() => nombre.Mostrarse(Apariencia.Aparecer(nombre.Mostrarse(null, new List<string> { "ser humano" })), null));

        Assert.Equal("predicados", error.ParamName);
    }

    [Fact]
    public void ConstructoresYComparaciones_ConservanValoresEsperados()
    {
        var esencia = Apariencia.Aparecer(new Nombre("base", 0d, null)
            .Mostrarse(null, new List<string> { "ser humano", "pensar humano" }));
        var nombre = new Nombre("ser", -Math.PI / 2, esencia);
        var copia = new Nombre(nombre);
        var otro = new Nombre("otro", 0d, null);
        var conEsenciaNula = new Nombre("vacio", 0d, null);

        Assert.Equal(Math.PI / 2, nombre.Fase, 10);
        Assert.Equal(nombre.Id, copia.Id);
        Assert.Equal(nombre.Texto, copia.Texto);
        Assert.Same(esencia, nombre.Esencia);
        Assert.Same(nombre.Esencia, copia.Esencia);
        Assert.Equal(double.MaxValue, conEsenciaNula.Esencia.Amplitud(0d));
        Assert.Equal(Math.Cos(1d) + Math.Sin(1d), conEsenciaNula.Esencia.Amplitud(1d), 10);
        Assert.Equal($"ser ({nombre.Fase * (180 / Math.PI):F2}º, {esencia.Amplitud(1d):F2} A)", nombre.ToString());
        Assert.True(nombre.Equals(copia));
        Assert.False(nombre.Equals(otro));
        Assert.False(nombre.Equals("no-nombre"));
        Assert.Equal(nombre.Id.GetHashCode(), nombre.GetHashCode());
    }

    [Fact]
    public void Designacion_ExponeMatematicaEsperadaYComparaPorId()
    {
        var designacion = new Nombre("origen", 0d, null)
            .Mostrarse(null, new List<string> { "ser humano", "ser lenguaje", "pensar humano" });
        var otra = new Nombre("origen", 0d, null)
            .Mostrarse(null, new List<string> { "ser humano", "ser lenguaje", "pensar humano" });
        var nombres = designacion.Nombres.ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal(0d, nombres[0].Fase, 10);
        Assert.Equal(2 * Math.PI / 3, nombres[1].Fase, 10);
        Assert.Equal(0d, nombres[2].Fase, 10);
        Assert.Equal("origen", nombres[2].Texto);
        Assert.Equal(2d, nombres[0].Esencia.Amplitud(0d), 10);
        Assert.Equal(0.3660254038d, nombres[1].Esencia.Amplitud(0d), 10);
        Assert.Equal(double.MaxValue, nombres[2].Esencia.Amplitud(0d));
        Assert.Equal(double.MaxValue, designacion.Amplitud(0d));
        Assert.True(designacion.Equals(designacion));
        Assert.False(designacion.Equals(otra));
        Assert.False(designacion.Equals("no-designacion"));
        Assert.Equal(designacion.Id.GetHashCode(), designacion.GetHashCode());
    }
}