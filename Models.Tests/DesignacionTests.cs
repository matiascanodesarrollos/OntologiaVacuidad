using FluentAssertions;

namespace Models.Tests;

public class DesignacionTests
{
    [Fact]
    public void Vacuidad_SeInicializaSinExcepcionesYConValoresConsistentes()
    {
        var vacuidad = Designacion.Vacuidad;

        vacuidad.Should().NotBeNull();
        vacuidad.Id.Should().NotBe(Guid.Empty);
        vacuidad.Frecuencia.Should().Be(0.0);
        vacuidad.Causa.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(-2.0)]
    public void Vacuidad_STFT_RespetaFormulaParaTauNoCero(double tau)
    {
        var valor = Designacion.Vacuidad.STFT((tau, 999.0));

        valor.Real.Should().BeApproximately(0.0, 1e-12);
        valor.Imaginary.Should().BeApproximately(1.0 / (2.0 * Math.PI * tau), 1e-12);
    }

    [Fact]
    public void Vacuidad_STFT_EnTauCero_TieneComponenteSingular()
    {
        var valor = Designacion.Vacuidad.STFT((0.0, 0.0));

        double.IsPositiveInfinity(valor.Real).Should().BeTrue();
        double.IsInfinity(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void Vacuidad_Causa_QuedaRetroreferenciadaALaMismaEsencia()
    {
        var vacuidad = Designacion.Vacuidad;

        vacuidad.Causa.Esencia.Should().BeSameAs(vacuidad);
    }

    [Fact]
    public void Designar_CreaNuevaDesignacionConFrecuenciaDeLaApariencia()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var designacion = Designacion.Designar(apariencia, Nombre.Cuerpo);

        designacion.Should().NotBeNull();
        designacion.Texto.Should().Be(Nombre.Cuerpo.Texto);
        designacion.Frecuencia.Should().Be(apariencia.Esencia.Frecuencia);
        designacion.Causa.Should().NotBeNull();
    }

    [Fact]
    public void Designar_CreaInstanciaNuevaConIdUnico()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });

        var d1 = Designacion.Designar(apariencia, Nombre.Cuerpo);
        var d2 = Designacion.Designar(apariencia, Nombre.Cuerpo);

        d1.Id.Should().NotBe(d2.Id);
        d1.Equals(d2).Should().BeFalse();
    }

    [Theory]
    [InlineData(-1.5, 0.5)]
    [InlineData(0.2, 2.0)]
    [InlineData(2.5, -3.0)]
    public void Designar_STFT_UsaTransformadaDelNombreConFrecuenciaSolicitada(double tau, double frecuencia)
    {
        var designacion = Designacion.Designar(
            Apariencia.Aparecer(new[] { Palabra.Yo(1.7) }),
            Nombre.Cuerpo);

        var esperado = Nombre.Cuerpo.TransformadaFourier(frecuencia);
        var actual = designacion.STFT((tau, frecuencia));

        (actual - esperado).Magnitude.Should().BeLessThan(1e-10);
    }

    [Fact]
    public void Designar_STFT_NoDependeDeTau()
    {
        var designacion = Designacion.Designar(
            Apariencia.Aparecer(new[] { Palabra.Yo(1.7) }),
            Nombre.Cuerpo);

        var v1 = designacion.STFT((-10.0, 1.25));
        var v2 = designacion.STFT((10.0, 1.25));

        (v1 - v2).Magnitude.Should().BeLessThan(1e-10);
    }

    [Fact]
    public void Mostrarse_DevuelveAparienciaConTextoDelNombre()
    {
        var designacion = Designacion.Designar(Apariencia.Aparecer(new[] { Palabra.Yo(1.0) }), Nombre.Cuerpo);

        var apariencia = designacion.Mostrarse(2.0);

        apariencia.Should().NotBeNull();
        apariencia.Texto.Should().Be(Nombre.Cuerpo.Texto);
        apariencia.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Equals_YHashCode_RespetanSemanticaPorId()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var d1 = Designacion.Designar(apariencia, Nombre.Cuerpo);
        var d2 = Designacion.Designar(apariencia, Nombre.Cuerpo);

        d1.Equals(d1).Should().BeTrue();
        d1.Equals(d2).Should().BeFalse();
        d1.Equals(null).Should().BeFalse();
        var hash = d1?.GetHashCode();
        var idHash = d1?.Id.GetHashCode();
        hash.Should().Be(idHash);
    }

    [Theory]
    [InlineData(-2.0)]
    [InlineData(0.0)]
    [InlineData(3.0)]
    public void Causa_DeDesignacionGenerada_EntregaFuncionFinita(double t)
    {
        var designacion = Designacion.Designar(
            Apariencia.Aparecer(new[] { Palabra.Yo(1.0), Palabra.Yo(-0.4) }),
            Nombre.Cuerpo);

        var valor = designacion.Causa.Funcion(t);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }
}
