using FluentAssertions;

public class AparienciaTests
{
    [Fact]
    public void Aparecer_ConUnaDesignacionDeUnSoloNombre_ConservaLosNombresYLaSemanticaDeDirac()
    {
        var nombre = Nombre.Imaginar("Vacuidad", 0d, 2d, 3d);
        var designacion = Designacion.Designar(nombre, Apariencia.Mente);

        var apariencia = Apariencia.Aparecer(designacion.Nombres.ToList());

        apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
        apariencia.Funcion(0d).Should().Be((double.PositiveInfinity, double.PositiveInfinity));
        apariencia.Funcion(1d).Should().Be((0d, 0d));
    }

    [Fact]
    public void Aparecer_ConMultiplesNombres_SumaLasAmplitudesDeTodosLosNombres()
    {
        var nombre = Nombre.Imaginar("Vacuidad", 0d, 2d, 3d);
        var designacion = Designacion.Designar(nombre, Apariencia.Aparecer(new List<Nombre>
        {
            Nombre.Imaginar("ser humano", 0d, 2d, 1d),
            Nombre.Imaginar("ser lenguaje", 2d * Math.PI / 3d, 2d, 1d),
            Nombre.Imaginar("pensar mente", 4d * Math.PI / 3d, 1d, 1d)
        }));

        var apariencia = Apariencia.Aparecer(designacion.Nombres.ToList());

        apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
        for (var t = 0d; t <= 2d; t += 0.25d)
        {
            var valorEsperado = designacion.Nombres
                .Select(n =>
                {
                    var fase = n.Fase;
                    var frecuencia = n.Frecuencia;
                    var amplitud = n.Amplitud;
                    return (
                        EjeReal: amplitud * Math.Cos(frecuencia * t + fase),
                        EjeImaginario: amplitud * Math.Sin(frecuencia * t + fase));
                })
                .Aggregate((a, b) => (a.EjeReal + b.EjeReal, a.EjeImaginario + b.EjeImaginario));
            var valor = apariencia.Funcion(t);

            valor.EjeReal.Should().BeApproximately(valorEsperado.EjeReal, 1e-10);
            valor.EjeImaginario.Should().BeApproximately(valorEsperado.EjeImaginario, 1e-10);
        }
    }

    [Fact]
    public void EqualsYGetHashCode_ComparanPorId()
    {
        var apariencia = Apariencia.Aparecer(new List<Nombre> { Nombre.Imaginar("ser humano", 0d, 1d, 1d) });
        var mismaReferencia = apariencia;
        var otra = Apariencia.Aparecer(new List<Nombre> { Nombre.Imaginar("ser mente", Math.PI / 3d, 1d, 2d) });

        apariencia.Equals(mismaReferencia).Should().BeTrue();
        apariencia.Equals(otra).Should().BeFalse();
        apariencia.Equals("no-apariencia").Should().BeFalse();
        apariencia.GetHashCode().Should().Be(apariencia.Id.GetHashCode());
    }

    [Fact]
    public void Vacuidad_DevuelveMaximoEnTiempoCeroYCeroFueraDeEseInstante()
    {
        Apariencia.Mente.Funcion(0d).Should().Be((double.PositiveInfinity, double.PositiveInfinity));
        Apariencia.Mente.Funcion(1d).Should().Be((0d, 0d));
    }
}