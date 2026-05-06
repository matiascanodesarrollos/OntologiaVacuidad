using DomainLogic.Services;
using DomainLogic.Services.Particulas;
using FluentAssertions;

public class ParticulasTests
{
    [Fact]
    public void Vector2D_OperaCorrectamenteEnTodosSusMetodos()
    {
        var vector = new Vector2D(3, 4);
        var otro = new Vector2D(1, 2);

        var suma = vector.Suma(otro);
        var escalado = vector.Escala(2);
        var normalizado = vector.Normalizar();
        var normalizadoCero = new Vector2D(0, 0).Normalizar();

        suma.X.Should().Be(4);
        suma.Y.Should().Be(6);
        escalado.X.Should().Be(6);
        escalado.Y.Should().Be(8);
        normalizado.X.Should().BeApproximately(0.6, 1e-10);
        normalizado.Y.Should().BeApproximately(0.8, 1e-10);
        normalizadoCero.X.Should().Be(0);
        normalizadoCero.Y.Should().Be(0);
        normalizado.Magnitud.Should().BeApproximately(1, 1e-10);
        vector.ProductoPunto(otro).Should().BeApproximately(11, 1e-10);
        vector.Equals(new Vector2D(3, 4)).Should().BeTrue();
        vector.Equals(otro).Should().BeFalse();
        vector.Equals("no-vector").Should().BeFalse();
        vector.GetHashCode().Should().Be(new Vector2D(3, 4).GetHashCode());
        vector.ToString().Should().Be("(3, 4)");
    }

    [Fact]
    public void Espacio_ConstruyeParticulasDesdeUnaDesignacionPublica()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");

        var espacio = new Espacio(designacion);

        espacio.Tiempo.Should().BeApproximately(0d, 1e-10);
        espacio.Particulas.Should().HaveCount(3);
        espacio.Particulas.All(particula => particula.Texto is not null).Should().BeTrue();
        espacio.Designacion.Should().Be(designacion);
        espacio.Particulas.Select(particula => particula.Texto).Should().Equal("ser humano. pensar lenguaje", "ser humano", "pensar lenguaje");
    }

    [Fact]
    public void MoverParticulas_ActualizaTiempoYPosicionesSinCrearParticulasCuandoNoHayColision()
    {
        var primera = Nombre.Imaginar("primera", 0d, 1d, 2d);
        var segunda = Nombre.Imaginar("segunda", Math.PI / 2d, 2d, 3d);
        var designacion = Designacion.Designar(segunda, Apariencia.Aparecer(new List<Nombre> { primera }));
        var espacio = new Espacio(designacion);
        var particula = espacio.Particulas.First();
        var posicionInicial = particula.Posicion2D;
        var otraParticula = espacio.Particulas.Last();

        espacio.MoverParticulas(0.5d);

        espacio.Tiempo.Should().BeApproximately(0.5d, 1e-10);
        particula.Posicion2D.X.Should().BeApproximately(posicionInicial.X + particula.Velocidad2D.X * 0.5d, 1e-10);
        particula.Posicion2D.Y.Should().BeApproximately(posicionInicial.Y + particula.Velocidad2D.Y * 0.5d, 1e-10);
        particula.Tiempo.Should().BeApproximately(0.5d, 1e-10);
        particula.Equals(particula).Should().BeTrue();
        particula.Equals(otraParticula).Should().BeFalse();
        particula.GetHashCode().Should().Be(particula.Id.GetHashCode());
        espacio.Particulas.Should().HaveCount(2);
    }

    [Fact]
    public void MoverParticulas_CuandoDosParticulasCoinciden_CreaUnaNuevaParticula()
    {
        var primera = Nombre.Imaginar("primera", 0d, 1d, 2d);
        var segunda = Nombre.Imaginar("segunda", 0d, 2d, 3d);
        var designacion = Designacion.Designar(segunda, Apariencia.Aparecer(new List<Nombre> { primera }));
        var espacio = new Espacio(designacion);

        espacio.MoverParticulas(1d);

        espacio.Particulas.Should().HaveCount(3);
        espacio.Particulas.Last().Texto.Should().Be("Vacuidad");
        espacio.Particulas.Last().Amplitud.Should().BeApproximately(2d, 1e-10);
        espacio.Particulas.Last().Posicion2D.X.Should().Be(0d);
        espacio.Particulas.Last().Posicion2D.Y.Should().Be(0d);
    }

    [Fact]
    public void AmbienteConfig_CrearAmbiente_IgnoraOracionesVaciasYCalculaAmplitudSegunCantidadDePredicados()
    {
        var designacion = AmbienteConfig.CrearAmbiente(" ser humano .. pensar lenguaje . ");

        designacion.Nombres.Should().HaveCount(3);
        designacion.Nombres.Select(nombre => nombre.Texto).Should().Equal(" ser humano .. pensar lenguaje . ","ser humano", "pensar lenguaje");
        designacion.Nombres.Select(nombre => nombre.Amplitud).Should().Equal(2d,1d, 1d);
    }
}