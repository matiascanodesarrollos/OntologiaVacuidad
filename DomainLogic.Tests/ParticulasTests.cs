using DomainLogic.Services.Particulas;
using Moq;
using System.Linq;

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
    public void Particula_MoverActualizaPosicionTiempoYComparacion()
    {
        var designacion = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser" },
            _ => (0d, 2d),
            _ => 1d);
        var nombre = designacion.Nombres.Single();
        var otroNombre = ((Designacion)Apariencia.Aparecer(
            new List<string> { "otro" },
            _ => (0d, 1d),
            _ => 1d)).Nombres.Single();
        var particula = new Particula(nombre);

        particula.Mover(2d);

        Assert.Equal(2d, particula.Posicion2D.X, 10);
        Assert.Equal(0d, particula.Posicion2D.Y, 10);
        Assert.Equal(2d, particula.Tiempo, 10);
        Assert.Equal(1d, particula.Velocidad2D.X, 10);
        Assert.Equal(0d, particula.Velocidad2D.Y, 10);
        Assert.True(particula.Equals(particula));
        Assert.False(particula.Equals(new Particula(otroNombre)));
        Assert.Equal(particula.Id.GetHashCode(), particula.GetHashCode());
    }

    [Fact]
    public void MoverParticulas_LlamaMoverYRecalculaOndas()
    {
        var designacion = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser humano" },
            _ => (0d, 1d),
            frecuencia => frecuencia + 10);
        var espacio = new Espacio(designacion, new List<double> { 0d, 1d });
        espacio.Particulas.Clear();

        var particula = new Mock<Particula>(designacion.Nombres.Last()) { CallBase = true };
        espacio.Particulas.Add(particula.Object);

        espacio.MoverParticulas(0.5d);

        particula.Verify(p => p.Mover(0.5d), Times.Once);
        Assert.Equal(0.5d, espacio.Tiempo, 10);
        Assert.Single(espacio.Ondas);
        Assert.Equal(2, espacio.Ondas[particula.Object].Count);
    }

    [Fact]
    public void MoverParticulas_CuandoHayColisionLanzaErrorPorClavesDuplicadas()
    {
        var designacion = (Designacion)Apariencia.Aparecer(
            new List<string> { "ser humano", "ser lenguaje" },
            _ => (0d, 1d),
            _ => 1d);
        var espacio = new Espacio(designacion, new List<double> { 0d });

        var error = Assert.Throws<ArgumentException>(() => espacio.MoverParticulas(1d));

        Assert.Contains("same key", error.Message, StringComparison.OrdinalIgnoreCase);
    }
}