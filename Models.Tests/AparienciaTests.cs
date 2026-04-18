using FluentAssertions;

public class AparienciaTests
{
    [Fact]
    public void Aparecer_ConUnaDesignacionDeUnSoloNombre_ConservaLaEsenciaDelNombre()
    {
        var nombre = new Nombre("logos", 0d, 2d);
        var designacion = nombre.Mostrarse(Apariencia.Vacuidad, "ser humano");

        var apariencia = Apariencia.Aparecer(designacion);

        apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
        for(var t = 0d; t <= 2d; t += 0.25d)
        {
            var valorEsperado = designacion.Nombres
                .Select(n => n.Esencia.Valor(t))
                .Aggregate((a, b) => (a.Amplitud + b.Amplitud, a.Fase + b.Fase));
            apariencia.Valor(t).Amplitud.Should().BeApproximately(valorEsperado.Amplitud, 1e-10);
            apariencia.Valor(t).Fase.Should().BeApproximately(valorEsperado.Fase, 1e-10);
        }
    }

    [Fact]
    public void Aparecer_ConMultiplesNombres_SumaLasEsenciasDeTodosLosNombres()
    {
        var nombre = new Nombre("logos", 0d, 2d);
        var designacion = nombre.Mostrarse(Apariencia.Vacuidad, "ser humano. ser lenguaje. pensar mente");

        var apariencia = Apariencia.Aparecer(designacion);

        apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
        for(var t = 0d; t <= 2d; t += 0.25d)
        {
            var valorEsperado = designacion.Nombres
                .Select(n => n.Esencia.Valor(t))
                .Aggregate((a, b) => (a.Amplitud + b.Amplitud, a.Fase + b.Fase));
            var valor = apariencia.Valor(t);

            valor.Amplitud.Should().BeApproximately(valorEsperado.Amplitud, 1e-10);
            valor.Fase.Should().BeApproximately(valorEsperado.Fase, 1e-10);
        }
    }

    [Fact]
    public void EqualsYGetHashCode_ComparanPorId()
    {
        var apariencia = new Nombre("logos", 0d, 1d).Esencia;
        var mismaReferencia = apariencia;
        var otra = new Nombre("ethos", Math.PI / 3, 1d).Esencia;

        apariencia.Equals(mismaReferencia).Should().BeTrue();
        apariencia.Equals(otra).Should().BeFalse();
        apariencia.Equals("no-apariencia").Should().BeFalse();
        apariencia.GetHashCode().Should().Be(apariencia.Id.GetHashCode());
    }

    [Fact]
    public void Vacuidad_DevuelveMaximoEnTiempoCeroYCeroFueraDeEseInstante()
    {
        Apariencia.Vacuidad.Valor(0d).Should().Be((double.MaxValue, double.MaxValue));
        Apariencia.Vacuidad.Valor(1d).Should().Be((0d, 0d));
    }
}