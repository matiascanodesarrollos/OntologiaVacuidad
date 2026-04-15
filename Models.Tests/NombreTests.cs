public class NombreTests
{
    [Fact]
    public void Mostrarse_ConDesignacionDevuelveNuevaDesignacionConNombreAgregadoYEfectoCalculado()
    {
        var fuente = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser humano", "decir verdad" },
            texto => texto == "ser humano" ? (0d, 1d) : (Math.PI / 2, 4d),
            frecuencia => frecuencia + 1);
        var nombre = fuente.Nombres.Last();

        var resultado = nombre.Mostrarse(fuente);
        var nombres = resultado.Nombres.ToList();
        var efecto = resultado.Efectos((1d, 2d));
        var amplitudEsperada = fuente.Amplitud * Math.Cos(2d + nombre.Fase)
            + nombre.Amplitud(fuente.Nombres.Count()) * Math.Sin(2d + nombre.Fase);

        Assert.Equal(3, nombres.Count);
        Assert.Same(nombre, nombres.Last());
        Assert.Equal(fuente.VelocidadGrupo(3d), resultado.VelocidadGrupo(3d), 10);
        Assert.Equal(amplitudEsperada, efecto.Amplitud, 10);
        Assert.Equal(1d, efecto.Fase, 10);
    }

    [Fact]
    public void Mostrarse_ConAparienciaCreadaPublicamenteDevuelveNuevaDesignacion()
    {
        var fuenteNombre = (Designacion)Apariencia.Aparecer(
            new List<string> { "logos" },
            _ => (Math.PI / 3, 2.5d),
            _ => 1d);
        var nombre = fuenteNombre.Nombres.Single();
        var apariencia = Apariencia.Aparecer(
            new List<string> { "base" },
            _ => (0d, 1d),
            _ => 1d);

        var resultado = nombre.Mostrarse(apariencia);

        Assert.Equal(2, resultado.Nombres.Count());
        Assert.Same(nombre, resultado.Nombres.Last());
        Assert.Equal(1.7267418003999999d, resultado.Efectos((5d, 9d)).Amplitud, 10);
        Assert.Equal(1d, resultado.VelocidadGrupo(10d), 10);
    }

    [Fact]
    public void NombreYCopia_ConservanValoresYRepresentacion()
    {
        var fuente = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser" },
            _ => (-Math.PI / 2, 2d),
            _ => 1d);
        var otro = ((Designacion)Apariencia.Aparecer(
            new List<string> { "otro" },
            _ => (0d, 1d),
            _ => 1d)).Nombres.Single();
        var nombre = fuente.Nombres.Single();
        var copia = new Nombre(nombre);

        Assert.Equal(Math.PI / 2, nombre.Fase, 10);
        Assert.Equal(nombre.Id, copia.Id);
        Assert.Equal(nombre.Texto, copia.Texto);
        Assert.Equal(nombre.Esencia, copia.Esencia);
        Assert.Equal(2d, nombre.Amplitud(123d), 10);
        Assert.Equal("ser (90.00º, 2.00 A)", nombre.ToString());
        Assert.True(nombre.Equals(copia));
        Assert.False(nombre.Equals(otro));
        Assert.False(nombre.Equals("no-nombre"));
        Assert.Equal(nombre.Id.GetHashCode(), nombre.GetHashCode());
    }

    [Fact]
    public void DesignacionYPalabra_ComparanPorIdYNormalizanFase()
    {
        var designacion = (Designacion)Apariencia.Aparecer(
            new List<string> { "vacio" },
            _ => (-7d, 1d),
            frecuencia => frecuencia * 3);
        var otraDesignacion = (Designacion)Apariencia.Aparecer(
            new List<string> { "vacio" },
            _ => (1d, 1d),
            _ => 1d);
        var palabra = designacion.Nombres.Single();
        var otraPalabra = otraDesignacion.Nombres.Single();
        var mismaDesignacion = designacion;

        Assert.InRange(palabra.Fase, 0d, 2 * Math.PI);
        Assert.True(palabra.Equals(palabra));
        Assert.False(palabra.Equals(otraPalabra));
        Assert.False(palabra.Equals("no-palabra"));
        Assert.Equal(palabra.Id.GetHashCode(), palabra.GetHashCode());
        Assert.True(designacion.Equals(mismaDesignacion));
        Assert.False(designacion.Equals(otraDesignacion));
        Assert.False(designacion.Equals("no-designacion"));
        Assert.Equal(designacion.Id.GetHashCode(), designacion.GetHashCode());
        Assert.Equal(6d, designacion.VelocidadGrupo(2d), 10);
    }
}