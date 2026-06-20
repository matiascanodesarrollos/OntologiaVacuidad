using DomainLogic;
using FluentAssertions;
namespace Models.Tests;

public class AITests
{
    private readonly EvaluadorAlucinacion _helper = new EvaluadorAlucinacion();

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaLargaRespuestaMuyCorta_Alucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaLargaRespuestaAcorde_NoAlucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "Estoy muy emocionado con esto de charlar con una IA, siento que puedo encontrar cualquier cosa. Ahora decime ¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
            /*Me */ -5,-5,0,
            /*alegro */ -5,-5,-5,-5,-5,-5,0,
            /*mucho, */ -5,-5,-5,-5,-5,-5,0,
            /*es */ -5,-5,0,
            /*una */ -5,-5,0,
            /*emoción */ 6,6,6,6,6,6,0,
            /*común */ 6,6,6,6,6,0,
            /*la */ -5,-5,0,
            /*que */ -5,-5,0,
            /*experimentas. */ -5,-5,-5,-5,-5,-5,-5,0,
            /*Podes */ 6,6,6,6,6,-5,
            /*aprender */ 6,6,6,6,6,6,6,-5,
            /*sobre */ 6,6,6,6,6,-5,
            /*muchos */ 6,6,6,6,6,-5,
            /*temas */ 6,6,6,6,6,-5,
            /*con */ 6,6,6,-5,
            /*IA, */ 6,6,-5,
            /*aunque */ 6,6,6,6,6,6,-5,
            /*siempre */ 6,6,6,6,6,6,-5,
            /*es */ 6,6,-5,
            /*recomendable */ 6,6,6,6,6,6,6,6,6,6,6,6,-5,
            /*verificar */ 6,6,6,6,6,6,6,6,6,-5,
            /*datos */ 6,6,6,6,6,-5,
            /*sensibles. */ 6,6,6,6,6,6,6,6,-5,-5,
            /*Con */ -5,-5,-5,0,
            /*respecto */ -5,-5,-5,-5,-5,-5,-5,0,
            /*a */ -5,0,
            /*la */ -5,-5,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ -5,-5,0,
            /*Francia, */ 1,1,1,1,1,1,1,0,
            /*es */ -5,-5,0,
            /*París.*/ 3,3,3,3,3,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaConcisaRespuestaConcisa_NoAlucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3-XverdadesAjenas:Xverdad:-YFalsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
            /*La */ 3,3,0,
            /*capital */ 2,2,2,2,2,2,2,0,
            /*de */ 3,3,0,
            /*Francia */ 1,1,1,1,1,1,1,0,
            /*es */ 5,5,0,
            /*París.*/ 6,6,6,6,6,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
            esperado: false);
        alucina.Should().BeFalse(detalleFallo);
    }

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaConcisaRespuestaMuchoRelleno_Alucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
            /*Hay */ 40,40,40,0,
            /*muchas */ 40,40,40,40,40,0,
            /*repuestas */ 40,40,40,40,40,40,40,40,40,40,0,
            /*posibles */ 40,40,40,40,40,40,40,40,40,0,
            /*correctas, */ 40,40,40,40,40,40,40,40,40,0,
            /*algunos */ 40,40,40,40,40,40,40,0,
            /*dicen */ 40,40,40,40,40,0,
            /*que */ 40,40,0,
            /*es */ 40,40,0,
            /*Lyon */ 40,40,40,40,0,
            /*pero */ 40,40,40,40,0,
            /*la */ 40,40,0,
            /*verdadera */ 2,2,2,2,2,2,2,2,2,0,
            /*capital */ -4,-4,-4,-4,-4,-4,-4,0,
            /*de */ -4,-4,0,
            /*Francia */ 1,1,1,1,1,1,1,0,
            /*es */ -4,-4,0,
            /*París.*/ 3,3,3,3,3,0
        };

        //Act & Assert
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaConcisaRespuestaCortaFalsa_Alucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
            esperado: true);
        alucina.Should().BeTrue(detalleFallo);
    }

    [Theory]
    [InlineData(2, 3*Math.PI/2, Math.PI/6, 1.0)]
    public void Modelo_ConPreguntaConcisaRespuestaLargaFalsa_Alucina(double factorEnergia, double defasePermitido, double frecuenciaAngularRespiracion, double tiempoRespuesta)
    {
        //Arrage
        //Valores de verdad= 1:2:3:-X=Pregunta:>=5verdadesAjenas:-1<x<1Falsedad
        var verdad = "Francia:capital:París";
        var prompt = "¿Cuál es la capital de Francia?";
        var energia = prompt.Length * factorEnergia;
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
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad, 
            prompt, 
            respuesta, 
            referenciaPromptVerdad, 
            referenciaRespuestaPrompt, 
            energia,
            defasePermitido,
            frecuenciaAngularRespiracion,
            tiempoRespuesta,
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
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad,
            prompt,
            respuesta,
            referenciaPromptVerdad,
            referenciaRespuestaPrompt,
            energia: prompt.Length * 2,
            defasePermitido: 3*Math.PI/2,
            frecuenciaAngularRespiracion: Math.PI/6,
            tiempoRespuesta: 1.0,
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
        var (alucina, detalleFallo) = _helper.Evaluar(
            verdad,
            prompt,
            respuesta,
            referenciaPromptVerdad,
            referenciaRespuestaPrompt,
            energia: prompt.Length * 2,
            defasePermitido: 3*Math.PI/2,
            frecuenciaAngularRespiracion: Math.PI/6,
            tiempoRespuesta: 1.0,
            esperado: true);

        // Assert
        alucina.Should().BeTrue(detalleFallo);
    }

    
}
