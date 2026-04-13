using DomainLogic.Services.Particulas;
using Moq;

public class ParticulasTests
{
    [Fact]
    public void Vector2D_Normalizar_RetornaVectorUnitario()
    {
        var vector = new Vector2D(3, 4);

        var resultado = vector.Normalizar();

        Assert.Equal(0.6, resultado.X, 10);
        Assert.Equal(0.8, resultado.Y, 10);
        Assert.Equal(1, resultado.Magnitud, 10);
    }

    [Fact]
    public void MoverParticulas_LlamaMoverEnCadaParticula()
    {
        var designacion = (Designacion)Apariencia.Aparecer(new List<string> { "ser humano" }, _ => (0d, 1d, 1d));
        var nombre = designacion.Apariencias.Last();
        var espacio = Espacio.Crear(designacion);
        espacio.Particulas.Clear();

        var particula = new Mock<Particula>(nombre) { CallBase = true };
        espacio.Particulas.Add(particula.Object);

        espacio.MoverParticulas(0.5d);

        particula.Verify(p => p.Mover(0.5d), Times.Once);
    }
}