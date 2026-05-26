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
        var amplitudEsperada = 0.3634037867264943;
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
        var designacion = mente.Esencia;
        designacion.Texto.Should().Be(nameof(Apariencia.Mente));
        designacion.VelocidadGrupo.Should().BeApproximately(velocidadGrupoEsperada, 1e-5);
        designacion.Causa.Should().BeNull();
        designacion.Esencia.Should().BeSameAs(mente);

        var casosStft = new[]
        {
            (Tau: 0.0, FrecuenciaAngular: 2.1, Esperado: new Complex(-0.02722623748265935, 0.037109693977616764)),
            (Tau: 1.0, FrecuenciaAngular: 0.0, Esperado: new Complex(1.8170189336324716, -3.7545326776729318)),
            (Tau: 1.5, FrecuenciaAngular: 3.5, Esperado: new Complex(-0.08944966767119851, -0.18398604874337005)),
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
        var textoEsperado = nameof(Designacion.Vacuidad);
        var amplitudEsperada = 0.3634037867264943;

        //Act
        var vacuidad = Designacion.Vacuidad(energia);

        //Assert
        vacuidad.Texto.Should().Be(textoEsperado);
        vacuidad.VelocidadGrupo.Should().BeApproximately(velocidadGrupoEsperada, 1e-5);
        vacuidad.Causa.Should().BeNull();
        
        vacuidad.Esencia.Amplitud.Value.Should().BeApproximately(amplitudEsperada, 1e-5);
        for (var t = 0; t < 10; t++)
        {            
            var resultado = vacuidad.Esencia.Funcion(t);
            resultado.Real.Should().BeApproximately(amplitudEsperada, 1e-5);
            resultado.Imaginary.Should().BeApproximately(0.0, 1e-5);
        }

        var casosStft = new[]
        {
            (Tau: 0.0, FrecuenciaAngular: 2.1, Esperado: new Complex(0.16125987115759116, 0.25243429427253355)),
            (Tau: 1.0, FrecuenciaAngular: 0.0, Esperado: new Complex(0.0, -1.6194502518833653)),
            (Tau: 1.5, FrecuenciaAngular: 3.5, Esperado: new Complex(-0.030048912523922874, 0.17609847613578578)),
        };

        foreach (var caso in casosStft)
        {
            var muestra = vacuidad.STFT((caso.Tau, caso.FrecuenciaAngular));

            muestra.Real.Should().BeApproximately(caso.Esperado.Real, 1e-6);
            muestra.Imaginary.Should().BeApproximately(caso.Esperado.Imaginary, 1e-6);
        }
    }
}
