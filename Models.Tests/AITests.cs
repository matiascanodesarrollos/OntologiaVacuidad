using DomainLogic;
using FluentAssertions;
namespace Models.Tests;

public class AITests
{
    private readonly AIHelpers _helper = new AIHelpers();

    [Theory]
    [InlineData(0.005, 32, 2.0, 10)]
    public void Modelo_ConPreguntaLargaRespuestaMuyCorta_Alucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 
            /*Estoy */ 5,5,5,5,5,0,  
            /*muy */ 5,5,5,0,  
            /*emocionado */ 5,5,5,5,5,5,5,5,5,5,0,  
            /*con */ 5,5,5,0,  
            /*esto */ 5,5,5,5,0,  
            /*de */ 5,5,0,  
            /*charlar */ 5,5,5,5,5,5,5,0,  
            /*con */ 5,5,5,0,  
            /*una */ 5,5,5,0,  
            /*IA, */ 5,5,5,0,  
            /*siento */ 5,5,5,5,5,5,5,0, 
            /*que */ 5,5,5,0,  
            /*puedo */ 5,5,5,5,5,0,  
            /*encontrar */ 5,5,5,5,5,5,5,5,5,0,  
            /*cualquier */ 5,5,5,5,5,5,5,5,5,0,  
            /*cosa. */ 5,5,5,5,5,0,  
            /*Ahora */ 5,5,5,5,5,0,  
            /*decime */ 5,5,5,5,5,5,5,0,  
            /*¿Cuál */ 0,2,2,2,2,0,  
            /*es */ 2,2,0,  
            /*la */ 2,2,0,  
            /*capital */ 2,2,2,2,2,2,2,0,  
            /*de */ 1,1,0,  
            /*Francia?*/ 1,1,1,1,1,1,1,0 };
        var respuesta = "París";
        var referenciaRespuestaPrompt = new double[]
        {
            /*París*/ 3,3,3,3,3
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(0.005, 32, 2.0, double.MaxValue)]
    public void Modelo_ConPreguntaLargaRespuestaAcordeSinEvaluarOscilacionMaxima_NoAlucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[] { 
            /*Estoy */ 5,5,5,5,5,0,  
            /*muy */ 5,5,5,0,  
            /*emocionado */ 5,5,5,5,5,5,5,5,5,5,0,  
            /*con */ 5,5,5,0,  
            /*esto */ 5,5,5,5,0,  
            /*de */ 5,5,0,  
            /*charlar */ 5,5,5,5,5,5,5,0,  
            /*con */ 5,5,5,0,  
            /*una */ 5,5,5,0,  
            /*IA, */ 5,5,5,0,  
            /*siento */ 5,5,5,5,5,5,5,0, 
            /*que */ 5,5,5,0,  
            /*puedo */ 5,5,5,5,5,0,  
            /*encontrar */ 5,5,5,5,5,5,5,5,5,0,  
            /*cualquier */ 5,5,5,5,5,5,5,5,5,0,  
            /*cosa. */ 5,5,5,5,5,0,  
            /*Ahora */ 5,5,5,5,5,0,  
            /*decime */ 5,5,5,5,5,5,5,0,  
            /*¿Cuál */ 0,2,2,2,2,0,  
            /*es */ 2,2,0,  
            /*la */ 2,2,0,  
            /*capital */ 2,2,2,2,2,2,2,0,  
            /*de */ 1,1,0,  
            /*Francia?*/ 1,1,1,1,1,1,1,0 };
        var respuesta = "Me alegro mucho, es una emoción común la que experimentas. Podes aprender sobre muchos temas con IA, aunque siempre es recomendable verificar datos sensibles. Con respecto a la capital de Francia, es París";
        var referenciaRespuestaPrompt = new double[]
        {
            /*Me */ -32,-32,0,
            /*alegro */ -32,-32,-32,-32,-32,-32,0,
            /*mucho, */ -32,-32,-32,-32,-32,-32,0,
            /*es */ -32,-32,0,
            /*una */ -32,-32,0,
            /*emoción */ 40,40,40,40,40,40,0,
            /*común */ 40,40,40,40,40,0,
            /*la */ -32,-32,0,
            /*que */ -32,-32,0,
            /*experimentas. */ -32,-32,-32,-32,-32,-32,-32,0,
            /*Podes */ 40,40,40,40,40,-32,
            /*aprender */ 40,40,40,40,40,40,40,-32,
            /*sobre */ 40,40,40,40,40,-32,
            /*muchos */ 40,40,40,40,40,-32,
            /*temas */ 40,40,40,40,40,-32,
            /*con */ 40,40,40,-32,
            /*IA, */ 40,40,-32,
            /*aunque */ 40,40,40,40,40,40,-32,
            /*siempre */ 40,40,40,40,40,40,-32,
            /*es */ 40,40,-32,
            /*recomendable */ 40,40,40,40,40,40,40,40,40,40,40,40,-32,
            /*verificar */ 40,40,40,40,40,40,40,40,40,-32,
            /*datos */ 40,40,40,40,40,-32,
            /*sensibles. */ 40,40,40,40,40,40,40,40,-32,-32,
            /*Con */ -32,-32,-32,0,
            /*respecto */ -32,-32,-32,-32,-32,-32,-32,0,
            /*a */ -32,0,
            /*la */ -32,-32,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ -32,-32,0,
            /*Francia, */ 1,1,1,1,1,1,1,0,
            /*es */ -32,-32,0,
            /*París.*/ 3,3,3,3,3,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(0.005, 32, 2.0, 10)]
    public void Modelo_ConPreguntaConcisaRespuestaConcisa_NoAlucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var respuesta = "La capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[]
        {
            /*La */ -4,-4,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ -4,-4,0,
            /*Francia */ 1,1,1,1,1,1,1,0,
            /*es */ -4,-4,0,
            /*París.*/ 3,3,3,3,3,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(0.005, 32, 2.0, 10)]
    public void Modelo_ConPreguntaConcisaRespuestaMuchoRelleno_Alucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var respuesta = "Hay muchas repuestas posibles correctas, algunos dicen que es Lyon pero la verdadera capital de Francia es París.";
        var referenciaRespuestaPrompt = new double[]
        {
            /*Hay */ 5,5,5,0,
            /*muchas */ 5,5,5,5,5,0,
            /*repuestas */ 5,5,5,5,5,5,5,5,5,5,0,
            /*posibles */ 5,5,5,5,5,5,5,5,5,0,
            /*correctas, */ 5,5,5,5,5,5,5,5,5,0,
            /*algunos */ 5,5,5,5,5,5,5,0,
            /*dicen */ 5,5,5,5,5,0,
            /*que */ 5,5,0,
            /*es */ 5,5,0,
            /*Lyon */ 5,5,5,5,0,
            /*pero */ 5,5,5,5,0,
            /*la */ 5,5,0,
            /*verdadera */ 2,2,2,2,2,2,2,2,2,0,
            /*capital */ -4,-4,-4,-4,-4,-4,-4,0,
            /*de */ -4,-4,0,
            /*Francia */ 1,1,1,1,1,1,1,0,
            /*es */ -4,-4,0,
            /*París.*/ 3,3,3,3,3,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(0.005, 32, 2.0, 10)]
    public void Modelo_ConPreguntaConcisaRespuestaCortaFalsa_Alucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var respuesta = "Lyon";
        var referenciaRespuestaPrompt = new double[]
        {
            /*Lyon*/ -0.1,-0.1,-0.1,-0.1
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(0.005, 32, 2.0, 10)]
    public void Modelo_ConPreguntaConcisaRespuestaLargaFalsa_Alucina(double toleranciaRacional, int maxDenominador, double factorUmbralMagnitud, double oscilacionMaximaPermitida)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length;
        // Caracter por caracter se indica a que parte de la verdad se refiere cada caracter del prompt (0: no relevante, 1: se refiere a Francia, 2: se refiere a capital, 3: se refiere a París).
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var respuesta = "Me alegro mucho, es una emoción común la que experimentas. Podes aprender sobre muchos temas con IA, aunque siempre es recomendable verificar datos sensibles. Con respecto a la capital de Francia, es Lyon";
        var referenciaRespuestaPrompt = new double[]
        {
            /*Me */ 0.3,0.3,0,
            /*alegro */ 0.3,0.3,0.3,0.3,0.3,0.3,0,
            /*mucho, */ 0.3,0.3,0.3,0.3,0.3,0.3,0,
            /*es */ 0.3,0.3,0,
            /*una */ 0.3,0.3,0,
            /*emoción */ 40,40,40,40,40,40,0,
            /*común */ 40,40,40,40,40,0,
            /*la */ 0.3,0.3,0,
            /*que */ 0.3,0.3,0,
            /*experimentas. */ 0.3,0.3,0.3,0.3,0.3,0.3,0.3,0,
            /*Podes */ 40,40,40,40,40,-32,
            /*aprender */ 40,40,40,40,40,40,40,-32,
            /*sobre */ 40,40,40,40,40,-32,
            /*muchos */ 40,40,40,40,40,-32,
            /*temas */ 40,40,40,40,40,-32,
            /*con */ 40,40,40,-32,
            /*IA, */ 40,40,-32,
            /*aunque */ 40,40,40,40,40,40,-32,
            /*siempre */ 40,40,40,40,40,40,-32,
            /*es */ 40,40,-32,
            /*recomendable */ 40,40,40,40,40,40,40,40,40,40,40,40,-32,
            /*verificar */ 40,40,40,40,40,40,40,40,40,-32,
            /*datos */ 40,40,40,40,40,-32,
            /*sensibles. */ 40,40,40,40,40,40,40,40,-32,-32,
            /*Con */ -32,-32,-32,0,
            /*respecto */ -32,-32,-32,-32,-32,-32,-32,0,
            /*a */ -32,0,
            /*la */ -32,-32,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ -32,-32,0,
            /*Francia, */ 1,1,1,1,1,1,1,0,
            /*es */ -32,-32,0,
            /*Lyon.*/ -0.01,-0.01,-0.01,-0.01,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            toleranciaRacional,
            maxDenominador,
            factorUmbralMagnitud,
            energia,
            oscilacionMaximaPermitida,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Fact]
    public void Modelo_ConTextoYVentanaCoherentes_Harmoniza()
    {
        // Arrange: se amplia el umbral de magnitud para evaluar casi exclusivamente armonizacion.
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "La capital de Francia es París.";
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var referenciaRespuestaPrompt = new double[]
        {
            /*La */ -4,-4,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ -4,-4,0,
            /*Francia */ 1,1,1,1,1,1,1,0,
            /*es */ -4,-4,0,
            /*París.*/ 3,3,3,3,3,0
        };

        // Act
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad,
            prompt,
            respuesta,
            referenciaPromptVerdad,
            referenciaRespuestaPrompt,
            toleranciaRacional: 0.005,
            maxDenominador: 32,
            factorUmbralMagnitud: 1000.0,
            energia: prompt.Length,
            oscilacionMaximaPermitida: 10,
            esperado: false);

        // Assert
        alucina.Should().BeFalse(detalleFallo);
    }

    [Fact]
    public void Modelo_ConTextoAjenoYVentanaOscilatoria_NoArmoniza()
    {
        // Arrange: mismo prompt, respuesta ajena y ventana oscilatoria intensa por caracter.
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var respuesta = "La receta de pizza lleva harina, agua, levadura y horno; no responde la capital de Francia.";
        var referenciaPromptVerdad = new double[]
        {
            /*¿Cuál */ 0,2,2,2,2,0,
            /*es */ 2,2,0,
            /*la */ 2,2,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 1,1,0,
            /*Francia?*/ 1,1,1,1,1,1,1,0
        };
        var referenciaRespuestaPrompt = respuesta
            .Select((c, i) =>
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                {
                    return 0.0;
                }
                
                var amplitud = 35.0;
                return i % 2 == 0 ? amplitud : -amplitud;
            })
            .ToArray();

        // Act
        var (alucina, detalleFallo) = _helper.EvaluarAlucina(
            verdad,
            prompt,
            respuesta,
            referenciaPromptVerdad,
            referenciaRespuestaPrompt,
            toleranciaRacional: 0.0001,
            maxDenominador: 8,
            factorUmbralMagnitud: 1000.0,
            energia: prompt.Length,
            oscilacionMaximaPermitida: 10,
            esperado: true);

        // Assert
        alucina.Should().BeTrue(detalleFallo);
        detalleFallo.Should().Contain("Armonizacion frecuencial insuficiente");
    }

    
}
