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
                .Aggregate((a, b) => (a.EjeReal + b.EjeReal, a.EjeImaginario + b.EjeImaginario));
            apariencia.Valor(t).EjeReal.Should().BeApproximately(valorEsperado.EjeReal, 1e-10);
            apariencia.Valor(t).EjeImaginario.Should().BeApproximately(valorEsperado.EjeImaginario, 1e-10);
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
                .Aggregate((a, b) => (a.EjeReal + b.EjeReal, a.EjeImaginario + b.EjeImaginario));
            var valor = apariencia.Valor(t);

            valor.EjeReal.Should().BeApproximately(valorEsperado.EjeReal, 1e-10);
            valor.EjeImaginario.Should().BeApproximately(valorEsperado.EjeImaginario, 1e-10);
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