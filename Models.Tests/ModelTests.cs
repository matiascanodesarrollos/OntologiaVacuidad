using System.Numerics;
using FluentAssertions;

namespace Models.Tests;

public class ModelTests
{
    [Fact]
    public void Mente_ConEnergia_CreaApariencia()
    {
        //Arrange
        const double energia = 10.0;
        var amplitudEsperada = 11.550913984334404;
        var velocidadGrupoEsperada = 0.0;
        
        //Act
        var mente = Apariencia.Mente(energia);
        var amplitud = mente.Amplitud.Value;

        //Assert
        amplitud.Should().BeApproximately(amplitudEsperada, 1e-5);
        for (var t = 0; t < 10; t++)
        {            
            var resultado = mente.Funcion(t);
            resultado.Real.Should().BeApproximately(amplitud, 1e-5);
            resultado.Imaginary.Should().BeApproximately(0.0, 1e-5);
        }
        mente.Texto.Should().Be(nameof(Apariencia.Mente));
        mente.VelocidadGrupo.Should().BeApproximately(velocidadGrupoEsperada, 1e-5);
        mente.Causa.Should().BeNull();

        var designacion = mente.Esencia;
        designacion.Esencia.Should().BeSameAs(mente);

        var casosStft = new[]
        {
            (Tau: 0.0, FrecuenciaAngular: 2.1, Esperado: new Complex(56.65675713184272, -114.49568639596335)),
            (Tau: 1.0, FrecuenciaAngular: 0.0, Esperado: new Complex(57.754569921672015, -118.1747981772064)),
            (Tau: 1.5, FrecuenciaAngular: 3.5, Esperado: new Complex(-2.9873249651848965, -4.66824962197985)),
        };

        foreach (var caso in casosStft)
        {
            var muestra = designacion.STFT((caso.Tau, caso.FrecuenciaAngular));

            muestra.Real.Should().BeApproximately(caso.Esperado.Real, 1e-6);
            muestra.Imaginary.Should().BeApproximately(caso.Esperado.Imaginary, 1e-6);
        }
    }

    [Fact]
    public void Vacuidad_CreaDesignacion()
    {
        //Arrange
        const double energia = 10.0;
        var velocidadGrupoEsperada = 0.0;
        var textoEsperado = nameof(Apariencia.Mente);
        var amplitudEsperada = 11.550913984334404;

        //Act
        var vacuidad = Designacion.Vacuidad(energia);

        //Assert
        vacuidad.Esencia.Texto.Should().Be(textoEsperado);
        vacuidad.Esencia.VelocidadGrupo.Should().BeApproximately(velocidadGrupoEsperada, 1e-6);
        vacuidad.Esencia.Causa.Should().BeNull();

        vacuidad.Esencia.Amplitud.Value.Should().BeApproximately(amplitudEsperada, 1e-6);
        for (var t = 0; t < 10; t++)
        {            
            var resultado = vacuidad.Esencia.Funcion(t);
            resultado.Real.Should().BeApproximately(amplitudEsperada, 1e-6);
            resultado.Imaginary.Should().BeApproximately(0.0, 1e-6);
        }

        var casosStft = new[]
        {
            (Tau: 0.0, FrecuenciaAngular: 2.1, Esperado: new Complex(-7.927819472801528, 1.248473559062469)),
            (Tau: 1.0, FrecuenciaAngular: 0.0, Esperado: new Complex(0.0, -36.767701156722396)),
            (Tau: 1.5, FrecuenciaAngular: 3.5, Esperado: new Complex(2.282074507148873, -3.147733527157076)),
        };

        foreach (var caso in casosStft)
        {
            var muestra = vacuidad.STFT((caso.Tau, caso.FrecuenciaAngular));

            muestra.Real.Should().BeApproximately(caso.Esperado.Real, 1e-6);
            muestra.Imaginary.Should().BeApproximately(caso.Esperado.Imaginary, 1e-6);
        }
    }
}
