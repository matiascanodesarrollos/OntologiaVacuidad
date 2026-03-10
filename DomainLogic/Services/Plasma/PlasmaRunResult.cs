namespace DomainLogic.Services.Plasma
{
    public readonly record struct PlasmaRunResult(
        RgbColor Color,
        int Saturaciones,
        int Desapariciones,
        double AmplitudPromedioFinal,
        double AmplitudMaximaFinal);
}
