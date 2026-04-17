using DomainLogic.Services;
using DomainLogic.Services.Particulas;

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

        Assert.Equal(4, suma.X);
        Assert.Equal(6, suma.Y);
        Assert.Equal(6, escalado.X);
        Assert.Equal(8, escalado.Y);
        Assert.Equal(0.6, normalizado.X, 10);
        Assert.Equal(0.8, normalizado.Y, 10);
        Assert.Equal(0, normalizadoCero.X);
        Assert.Equal(0, normalizadoCero.Y);
        Assert.Equal(1, normalizado.Magnitud, 10);
        Assert.Equal(11, vector.ProductoPunto(otro), 10);
        Assert.True(vector.Equals(new Vector2D(3, 4)));
        Assert.False(vector.Equals(otro));
        Assert.False(vector.Equals("no-vector"));
        Assert.Equal(vector.GetHashCode(), new Vector2D(3, 4).GetHashCode());
        Assert.Equal("(3, 4)", vector.ToString());
    }

    [Fact]
    public void Espacio_ConstruyeParticulasYOndasDesdeUnaDesignacionPublica()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");

        var espacio = new Espacio(designacion);

        Assert.Equal(0d, espacio.Tiempo, 10);
        Assert.Equal(3, espacio.Particulas.Count);
        Assert.Equal(3, espacio.Ondas.Count);
        Assert.DoesNotContain(espacio.Particulas, particula => particula.Texto is null);
        Assert.All(espacio.Particulas, particula => Assert.True(espacio.Ondas.ContainsKey(particula)));
        Assert.All(espacio.Ondas.Values, ondas => Assert.Equal(2, ondas.Count));
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

        Assert.Equal(0.5d, espacio.Tiempo, 10);
        Assert.Equal(posicionInicial.X + particula.Velocidad2D.X * 0.5d, particula.Posicion2D.X, 10);
        Assert.Equal(posicionInicial.Y + particula.Velocidad2D.Y * 0.5d, particula.Posicion2D.Y, 10);
        Assert.Equal(0.5d, particula.Tiempo, 10);
        Assert.True(particula.Equals(particula));
        Assert.False(particula.Equals(otraParticula));
        Assert.Equal(particula.Id.GetHashCode(), particula.GetHashCode());
        Assert.All(espacio.Ondas.Values, ondas => Assert.Equal(2, ondas.Count));
    }

    [Fact]
    public void CalcularOndas_RecalculaDespuesDeModificarParticulasPublicamente()
    {
        var designacion = AmbienteConfig.CrearAmbiente("ser humano. pensar lenguaje");
        var espacio = new Espacio(designacion);

        espacio.Particulas.RemoveAt(1);

        espacio.CalcularOndas();

        Assert.Equal(2, espacio.Particulas.Count);
        Assert.Equal(2, espacio.Ondas.Count);
    }
}