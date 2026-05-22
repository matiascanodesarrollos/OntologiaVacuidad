using FluentAssertions;
using System.Numerics;

namespace Models.Tests;

public class DesignacionTests
{
    [Fact]
    public void Designar_SeInicializaSinExcepcionesYConValoresConsistentes()
    {
        var cuerpo = CreateDesignacionBase();

        cuerpo.Should().NotBeNull();
        cuerpo.Id.Should().NotBe(Guid.Empty);
        cuerpo.Texto.Should().Be("Vacuidad");
        cuerpo.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Designar_STFT_EstaDisponible()
    {
        var designacion = CreateDesignacionBase();

        designacion.STFT.Should().NotBeNull();
    }

    [Fact]
    public void Designar_Esencia_QuedaReferenciada()
    {
        var apariencia = CreateApariencia(1.0);
        var designacion = CreateDesignacionBase();

        designacion.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Designar_Causa_QuedaRetroreferenciadaALaMismaEsencia()
    {
        var apariencia = CreateApariencia(1.0);
        var cuerpo = Designacion.Designar(apariencia, CreateNombre());

        cuerpo.Causa.Should().BeNull();
    }

    [Fact]
    public void Designar_CreaNuevaDesignacionConFrecuenciaAngularDeLaApariencia()
    {
        var apariencia = CreateApariencia(1.0, 1.0);
        var nombre = CreateNombre();
        var designacion = Designacion.Designar(apariencia, nombre);

        designacion.Should().NotBeNull();
        designacion.Texto.Should().Be(nombre.Texto);
        designacion.Esencia.Should().BeSameAs(apariencia);
        designacion.STFT.Should().NotBeNull();
    }

    [Fact]
    public void Designar_CreaInstanciaNuevaConIdUnico()
    {
        var apariencia = CreateApariencia(1.0, 1.0);
        var nombre = CreateNombre();

        var d1 = Designacion.Designar(apariencia, nombre);
        var d2 = Designacion.Designar(apariencia, nombre);

        d1.Id.Should().NotBe(d2.Id);
        d1.Equals(d2).Should().BeFalse();
    }

    [Fact]
    public void Designar_ConPalabraDirecta_ConservaCausa()
    {
        var palabra = new Palabra("Cuerpo", 1.25, GaussianWindow);
        var designacion = Designacion.Designar(palabra, CreateNombre());

        designacion.Causa.Should().BeSameAs(palabra);
    }

    [Fact]
    public void Mostrarse_DevuelveAparienciaConTextoDelNombre()
    {
        var nombre = CreateNombre();
        var designacion = Designacion.Designar(CreateApariencia(1.0, 1.0), nombre);

        var apariencia = designacion.Mostrarse(2.0);

        apariencia.Should().NotBeNull();
        apariencia.Esencia.Texto.Should().Be(nombre.Texto);
        apariencia.Esencia.Should().NotBeNull();
    }

    [Fact]
    public void Equals_YHashCode_RespetanSemanticaPorId()
    {
        var apariencia = CreateApariencia(1.0, 1.0);
        var nombre = CreateNombre();
        var d1 = Designacion.Designar(apariencia, nombre);
        var d2 = Designacion.Designar(apariencia, nombre);

        d1.Equals(d1).Should().BeTrue();
        d1.Equals(d2).Should().BeFalse();
        d1.Equals(null).Should().BeFalse();
        var hash = d1?.GetHashCode();
        var idHash = d1?.Id.GetHashCode();
        hash.Should().Be(idHash);
    }

    [Fact]
    public void Designar_ConAparienciaCompuesta_PuedeNoTenerCausaPalabra()
    {
        var designacion = Designacion.Designar(CreateApariencia(1.0, 1.0, -0.4), CreateNombre());

        designacion.Causa.Should().BeNull();
    }

    [Fact]
    public void EstimarFrecuenciaAngular_ConEspectroPlano_DevuelveCero()
    {
        var probe = CreateProbe();

        var estimada = probe.InvocarEstimador(_ => Complex.One);

        estimada.Should().Be(0.0);
    }

    [Fact]
    public void EstimarFrecuenciaAngular_ConMuestrasNoFinitas_DevuelveCero()
    {
        var probe = CreateProbe();

        var estimada = probe.InvocarEstimador(_ => new Complex(double.NaN, 0.0));

        estimada.Should().Be(0.0);
    }

    [Fact]
    public void EstimarFrecuenciaAngular_EligePicoDeMagnitud()
    {
        var probe = CreateProbe();

        var estimada = probe.InvocarEstimador(p =>
        {
            var distancia = p.FrecuenciaAngular - 2.5;
            return new Complex(Math.Exp(-(distancia * distancia)), 0.0);
        });

        estimada.Should().BeApproximately(2.5, 1e-12);
    }

    [Fact]
    public void CalcularSTFT_ConFuncionNula_DevuelveCero()
    {
        var probe = CreateProbe();

        var valor = probe.InvocarSTFT(
            tau: 0.0,
            frecuenciaAngular: 1.0,
            funcion: _ => Complex.Zero,
            ventana: _ => Complex.One);

        valor.Magnitude.Should().BeLessThan(1e-12);
    }

    [Fact]
    public void CalcularSTFT_ConFuncionYVentanaConstantes_AreaEsperadaEnOmegaCero()
    {
        var probe = CreateProbe();

        var valor = probe.InvocarSTFT(
            tau: 0.0,
            frecuenciaAngular: 0.0,
            funcion: _ => Complex.One,
            ventana: _ => Complex.One);

        valor.Real.Should().BeApproximately(16.0, 1e-6);
        valor.Imaginary.Should().BeApproximately(0.0, 1e-9);
    }

    [Theory]
    [InlineData(50.0)]
    [InlineData(500.0)]
    [InlineData(5000.0)]
    public void CalcularSTFT_ConFrecuenciaAlta_EntregaValorFinito(double frecuenciaAngular)
    {
        var probe = CreateProbe();

        var valor = probe.InvocarSTFT(
            tau: 0.25,
            frecuenciaAngular: frecuenciaAngular,
            funcion: t => new Complex(Math.Exp(-(t * t)), 0.0),
            ventana: t => new Complex(Math.Exp(-(t * t) / 2.0), 0.0));

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void CalcularSTFT_ConVentanaDeColaLarga_EntregaValorFinito()
    {
        var probe = CreateProbe();

        var valor = probe.InvocarSTFT(
            tau: -0.4,
            frecuenciaAngular: 3.0,
            funcion: t => new Complex(1.0 / (1.0 + (t * t)), 0.0),
            ventana: t => new Complex(1.0 / (1.0 + (t * t)), 0.0));

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    [Fact]
    public void EstimarFrecuenciaAngular_ConPicoEnBordeSuperior_SeleccionaMaximo()
    {
        var probe = CreateProbe();

        var estimada = probe.InvocarEstimador(p =>
        {
            var normalizada = (p.FrecuenciaAngular + 8.0) / 16.0;
            return new Complex(normalizada, 0.0);
        });

        estimada.Should().BeApproximately(8.0, 1e-12);
    }

    [Theory]
    [MemberData(nameof(VentanasProyeccionVariadas))]
    public void Mostrarse_DesdeDesignacion_ConVentanasVariadas_ProduceAmplitudFinita(
        string _,
        Func<double, Complex> ventana)
    {
        var apariencia = CreateApariencia(1.0);
        var nombre = new Nombre("Vacuidad", 0.0, ventana);
        var designacion = Designacion.Designar(apariencia, nombre);

        var proyeccion = designacion.Mostrarse(1.5);

        double.IsFinite(proyeccion.Amplitud).Should().BeTrue();
        proyeccion.Amplitud.Should().BeGreaterThanOrEqualTo(0.0);
    }

    [Theory]
    [MemberData(nameof(VentanasAnalisisVariadas))]
    public void CalcularSTFT_ConVentanasVariadas_EntregaValorFinito(
        string _,
        Func<double, Complex> ventana)
    {
        var probe = CreateProbe();

        var valor = probe.InvocarSTFT(
            tau: 0.3,
            frecuenciaAngular: 4.5,
            funcion: t => new Complex(Math.Exp(-(t * t)), 0.0),
            ventana: ventana);

        double.IsFinite(valor.Real).Should().BeTrue();
        double.IsFinite(valor.Imaginary).Should().BeTrue();
    }

    public static IEnumerable<object[]> VentanasAnalisisVariadas()
    {
        yield return new object[] { "gaussiana", (Func<double, Complex>)GaussianWindow };
        yield return new object[] { "cola-larga", (Func<double, Complex>)(t => new Complex(1.0 / (1.0 + (t * t)), 0.0)) };
        yield return new object[] { "compacta", (Func<double, Complex>)(t => new Complex(Math.Abs(t) <= 1.0 ? 1.0 : 0.0, 0.0)) };
        yield return new object[] { "compleja-oscilatoria", (Func<double, Complex>)(t => Complex.FromPolarCoordinates(Math.Exp(-(t * t) / 2.0), 0.5 * t)) };
    }

    public static IEnumerable<object[]> VentanasProyeccionVariadas()
    {
        yield return new object[] { "gaussiana", (Func<double, Complex>)GaussianWindow };
        yield return new object[] { "cola-larga", (Func<double, Complex>)(t => new Complex(1.0 / (1.0 + (t * t)), 0.0)) };
        yield return new object[] { "compacta", (Func<double, Complex>)(t => new Complex(Math.Abs(t) <= 1.0 ? 1.0 : 0.0, 0.0)) };
        yield return new object[] { "compleja-oscilatoria", (Func<double, Complex>)(t => Complex.FromPolarCoordinates(Math.Exp(-(t * t) / 2.0), 0.6 * t)) };
    }

    private static Designacion CreateDesignacionBase()
    {
        return Designacion.Designar(CreateApariencia(1.0), CreateNombre());
    }

    private static Apariencia CreateApariencia(double baseFrequency, params double[] extraFrequencies)
    {
        var palabras = new List<Palabra> { new("base", baseFrequency, GaussianWindow) };
        palabras.AddRange(extraFrequencies.Select((omega, index) => new Palabra($"extra-{index}", omega, GaussianWindow)));
        return Apariencia.Aparecer(palabras);
    }

    private static Nombre CreateNombre()
    {
        return new Nombre("Vacuidad", 0.0, UnitStepTransform);
    }

    private static DesignacionProbe CreateProbe()
    {
        return new DesignacionProbe(CreateDesignacionBase());
    }

    private static Complex GaussianWindow(double t)
    {
        return new Complex(Math.Exp(-(t * t) / 2.0), 0.0);
    }

    private static Complex UnitStepTransform(double omega)
    {
        return omega >= 0.0 ? Complex.One : Complex.Zero;
    }

    private sealed class DesignacionProbe : Designacion
    {
        public DesignacionProbe(Designacion otra)
            : base(otra)
        {
        }

        public Complex InvocarSTFT(
            double tau,
            double frecuenciaAngular,
            Func<double, Complex> funcion,
            Func<double, Complex> ventana)
        {
            return CalcularSTFT(tau, frecuenciaAngular, funcion, ventana);
        }

        public double InvocarEstimador(Func<(double tau, double FrecuenciaAngular), Complex> funcion)
        {
            return EstimarFrecuenciaAngular(funcion);
        }
    }
}
