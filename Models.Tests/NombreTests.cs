public class NombreTests
{
    [Fact]
    public void Mostrarse_ConPredicadosCreaDesignacionYAgregaElNombreActual()
    {
        var frecuencia = 1d;
        var nombre = new Nombre("logos", Math.PI / 3, frecuencia);

        var resultado = nombre.Mostrarse(Apariencia.Vacuidad, new List<string> { "ser humano", "ser lenguaje" });
        var nombres = resultado.Nombres.ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal("ser humano", nombres[0].Texto);
        Assert.Equal(nombre.Frecuencia, nombres.Last().Frecuencia);
        Assert.Equal(1d, resultado.VelocidadGrupo(nombre), 10);
        Assert.Equal(frecuencia, nombre.Frecuencia);
    }

    [Fact]
    public void Mostrarse_ConAparienciaExistenteProyectaSuEsenciaConLaFrecuenciaDelNombre()
    {
        var predicados = new List<string> { "ser humano", "pensar humano" };
        var baseDesignacion = new Nombre("origen", 0d, 1d)
            .Mostrarse(Apariencia.Vacuidad, predicados);
        var nombre = new Nombre("logos", Math.PI / 2, 1d);

        var resultado = nombre.Mostrarse(baseDesignacion, predicados);
        var nombres = resultado.Nombres.ToList();

        Assert.Equal(3, nombres.Count);
        Assert.Equal(baseDesignacion.Nombres.First().Texto, nombres[0].Texto);
        Assert.Equal(baseDesignacion.Esencia.Last().Texto, nombres.Last().Texto);
        Assert.Equal(nombre.Frecuencia, nombres.Last().Frecuencia);
    }

    [Fact]
    public void Mostrarse_SinPredicadosLanzaExcepcion()
    {
        var nombre = new Nombre("logos", 0d, 1d);

        Assert.ThrowsAny<Exception>(() => nombre.Mostrarse(Apariencia.Vacuidad, null));
    }

    [Fact]
    public void ConstructoresYComparaciones_ConservanValoresEsperados()
    {
        var nombre = new Nombre("ser", -Math.PI / 2, 1d);
        var copia = new Nombre(nombre);
        var otro = new Nombre("otro", 0d, 1d);

        Assert.Equal(Math.PI / 2, nombre.Fase, 10);
        Assert.Equal(nombre.Texto, copia.Texto);
        Assert.Same(nombre.Esencia, copia.Esencia);
        Assert.Equal((1d, 0d), nombre.Esencia.Valor(0d));
        Assert.Equal((Math.Cos(1d), Math.Sin(1d)), nombre.Esencia.Valor(1d));
        Assert.Equal($"ser ({nombre.Fase * (180 / Math.PI):F2}º, {nombre.Frecuencia:F2} Hz)", nombre.ToString());
        Assert.True(nombre.Equals(copia));
        Assert.False(nombre.Equals(otro));
        Assert.False(nombre.Equals("no-nombre"));
        Assert.Equal(nombre.Id.GetHashCode(), nombre.GetHashCode());
    }

    [Fact]
    public void Designacion_CreaPredicadosYAgregaElNombreProyectado()
    {
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var designacion = new Nombre("origen", 0d, 1d)
            .Mostrarse(Apariencia.Vacuidad, predicados);
        var otra = new Nombre("origen", 0d, 1d)
            .Mostrarse(Apariencia.Vacuidad, predicados);
        var nombres = designacion.Nombres.ToList();

        Assert.Equal(4, nombres.Count);
        Assert.Equal(0d, nombres[0].Fase, 10);
        Assert.Equal(2 * Math.PI / 3, nombres[1].Fase, 10);
        Assert.Equal(4 * Math.PI / 3, nombres[2].Fase, 10);
        Assert.Equal("pensar humano", nombres[2].Texto);
        Assert.Equal((1d, 0d), nombres[0].Esencia.Valor(0d));
        Assert.Equal((1d, 0d), nombres[1].Esencia.Valor(0d));
        Assert.Equal((1d, 0d), nombres[2].Esencia.Valor(0d));
        Assert.Equal((double.MaxValue, double.MaxValue), designacion.Valor(0d));
        Assert.True(designacion.Equals(designacion));
        Assert.False(designacion.Equals(otra));
        Assert.False(designacion.Equals("no-designacion"));
        Assert.Equal(designacion.Id.GetHashCode(), designacion.GetHashCode());
    }
}