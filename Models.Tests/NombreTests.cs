using System.Linq;

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
            + nombre.Amplitud(nombre.Fase) * Math.Sin(2d + nombre.Fase);

        Assert.Equal(3, nombres.Count);
        Assert.Same(nombre, nombres.Last());
        Assert.Equal(fuente.VelocidadGrupo(3d), resultado.VelocidadGrupo(3d), 10);
        Assert.Equal(amplitudEsperada, efecto.Amplitud, 10);
        Assert.Equal(1d, efecto.Fase, 10);
    }

    [Fact]
    public void Mostrarse_ConAparienciaBaseDevuelveDesignacionBasica()
    {
        var nombre = new Nombre("logos", Math.PI / 3, _ => 2.5d);
        var apariencia = new Apariencia(1d);

        var resultado = nombre.Mostrarse(apariencia);

        Assert.Single(resultado.Nombres);
        Assert.Same(nombre, resultado.Nombres.Single());
        Assert.Equal((0d, 0d), resultado.Efectos((5d, 9d)));
        Assert.Equal(1d, resultado.VelocidadGrupo(10d), 10);
    }

    [Fact]
    public void NombreYCopia_ConservanValoresYRepresentacion()
    {
        var nombre = new Nombre("ser", -Math.PI / 2, _ => 2d);
        var copia = new Nombre(nombre);

        Assert.Equal(Math.PI / 2, nombre.Fase, 10);
        Assert.Equal(nombre.Id, copia.Id);
        Assert.Equal(nombre.Texto, copia.Texto);
        Assert.Equal(nombre.Esencia, copia.Esencia);
        Assert.Equal(2d, nombre.Amplitud(123d), 10);
        Assert.Equal("ser (90.00º, 2.00 A)", nombre.ToString());
        Assert.True(nombre.Equals(copia));
        Assert.False(nombre.Equals(new Nombre("otro", 0d, _ => 1d)));
        Assert.False(nombre.Equals("no-nombre"));
        Assert.Equal(nombre.Id.GetHashCode(), nombre.GetHashCode());
    }

    [Fact]
    public void DesignacionYPalabra_ComparanPorIdYNormalizanFase()
    {
        var palabra = new Palabra("vacio", -7d);
        var otraPalabra = new Palabra("vacio", 1d);
        var designacion = new Designacion(
            new List<Nombre> { new Nombre("ser", 0d, _ => 1d) },
            x => (x.Tiempo + x.Frecuencia, x.Tiempo),
            frecuencia => frecuencia * 3);
        var mismaDesignacion = designacion;
        var otraDesignacion = new Designacion(
            new List<Nombre> { new Nombre("ser", 0d, _ => 1d) },
            x => (0d, 0d),
            _ => 1d);

        Assert.InRange(palabra.Fase, 0d, 2 * Math.PI);
        Assert.True(palabra.Equals(palabra));
        Assert.False(palabra.Equals(otraPalabra));
        Assert.False(palabra.Equals("no-palabra"));
        Assert.Equal(palabra.Id.GetHashCode(), palabra.GetHashCode());
        Assert.True(designacion.Equals(mismaDesignacion));
        Assert.False(designacion.Equals(otraDesignacion));
        Assert.False(designacion.Equals("no-designacion"));
        Assert.Equal(designacion.Id.GetHashCode(), designacion.GetHashCode());
        Assert.Equal(6d, designacion.Efectos((2d, 4d)).Amplitud, 10);
    }
}