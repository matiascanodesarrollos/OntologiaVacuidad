using FluentAssertions;

namespace Models.Tests;

public class DesignacionTests
{
    [Fact]
    public void Cuerpo_SeInicializaSinExcepcionesYConValoresConsistentes()
    {
        var cuerpo = Designacion.Cuerpo;

        cuerpo.Should().NotBeNull();
        cuerpo.Id.Should().NotBe(Guid.Empty);
        cuerpo.FrecuenciaAngular.Should().Be(-8.0);
        cuerpo.Causa.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(-2.0)]
    public void Cuerpo_STFT_RespetaFormulaDeDeltaPrimaParaFrecuenciaAngular(double tau)
    {
        var frecuenciaAngular = 2.0;
        var valor = Designacion.Cuerpo.STFT((tau, frecuenciaAngular));

        valor.Real.Should().BeApproximately(0.0, 1e-12);
        valor.Imaginary.Should().BeApproximately((frecuenciaAngular / 2.0) * Math.PI, 1e-12);
    }

    [Fact]
    public void Cuerpo_STFT_NoDependeDeTau()
    {
        var v1 = Designacion.Cuerpo.STFT((0.0, 1.25));
        var v2 = Designacion.Cuerpo.STFT((10.0, 1.25));

        (v1 - v2).Magnitude.Should().BeLessThan(1e-12);
    }

    [Fact]
    public void Cuerpo_Causa_QuedaRetroreferenciadaALaMismaEsencia()
    {
        var cuerpo = Designacion.Cuerpo;

        cuerpo.Causa.Esencia.Should().BeSameAs(cuerpo);
    }

    [Fact]
    public void Designar_CreaNuevaDesignacionConFrecuenciaAngularDeLaApariencia()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var designacion = Designacion.Designar(apariencia, Nombre.Vacuidad);

        designacion.Should().NotBeNull();
        designacion.Texto.Should().Be(Nombre.Vacuidad.Texto);
        designacion.FrecuenciaAngular.Should().Be(apariencia.Esencia.FrecuenciaAngular);
        designacion.Causa.Should().NotBeNull();
    }

    [Fact]
    public void Designar_CreaInstanciaNuevaConIdUnico()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });

        var d1 = Designacion.Designar(apariencia, Nombre.Vacuidad);
        var d2 = Designacion.Designar(apariencia, Nombre.Vacuidad);

        d1.Id.Should().NotBe(d2.Id);
        d1.Equals(d2).Should().BeFalse();
    }

    [Theory]
    [InlineData(-1.5, 0.5)]
    [InlineData(0.2, 2.0)]
    [InlineData(2.5, -3.0)]
    public void Designar_STFT_UsaTransformadaDelNombreConFrecuenciaAngularSolicitada(double tau, double frecuenciaAngular)
    {
        var designacion = Designacion.Designar(
            Apariencia.Aparecer(new[] { Palabra.Yo(1.7) }),
            Nombre.Vacuidad);

        var esperado = Nombre.Vacuidad.TransformadaFourier(frecuenciaAngular);
        var actual = designacion.STFT((tau, frecuenciaAngular));

        (actual - esperado).Magnitude.Should().BeLessThan(1e-10);
    }

    [Fact]
    public void Designar_STFT_NoDependeDeTau()
    {
        var designacion = Designacion.Designar(
            Apariencia.Aparecer(new[] { Palabra.Yo(1.7) }),
            Nombre.Vacuidad);

        var v1 = designacion.STFT((-10.0, 1.25));
        var v2 = designacion.STFT((10.0, 1.25));

        (v1 - v2).Magnitude.Should().BeLessThan(1e-10);
    }

    [Fact]
    public void Mostrarse_DevuelveAparienciaConTextoDelNombre()
    {
        var designacion = Designacion.Designar(Apariencia.Aparecer(new[] { Palabra.Yo(1.0) }), Nombre.Vacuidad);

        var apariencia = designacion.Mostrarse(2.0);

        apariencia.Should().NotBeNull();
        apariencia.Texto.Should().Be(Nombre.Vacuidad.Texto);
        apariencia.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Equals_YHashCode_RespetanSemanticaPorId()
    {
        var apariencia = Apariencia.Aparecer(new[] { Palabra.Yo(1.0) });
        var d1 = Designacion.Designar(apariencia, Nombre.Vacuidad);
        var d2 = Designacion.Designar(apariencia, Nombre.Vacuidad);

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
            Nombre.Vacuidad);

        var valor = designacion.Causa.Funcion(t);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }
}
