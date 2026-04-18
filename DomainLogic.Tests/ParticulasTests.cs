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
    public void Espacio_ConstruyeParticulasYOndasDesdeUnaDesignacionPublica()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");

        var espacio = new Espacio(designacion);

        espacio.Tiempo.Should().BeApproximately(0d, 1e-10);
        espacio.Particulas.Should().HaveCount(3);
        espacio.Ondas.Should().HaveCount(3);
        espacio.Particulas.All(particula => particula.Texto is not null).Should().BeTrue();
        espacio.Particulas.Should().OnlyContain(particula => espacio.Ondas.ContainsKey(particula));
        espacio.Ondas.Values.Should().OnlyContain(ondas => ondas.Count == 2);
    }

    [Fact]
    public void MoverParticulas_ActualizaTiempoPosicionesYOndas()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");
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
        espacio.Ondas.Values.Should().OnlyContain(ondas => ondas.Count == 2);
    }

    [Fact]
    public void CalcularOndas_RecalculaDespuesDeModificarParticulasPublicamente()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");
        var espacio = new Espacio(designacion);

        espacio.Particulas.RemoveAt(1);

        espacio.CalcularOndas();

        espacio.Particulas.Should().HaveCount(2);
        espacio.Ondas.Should().HaveCount(2);
    }
}