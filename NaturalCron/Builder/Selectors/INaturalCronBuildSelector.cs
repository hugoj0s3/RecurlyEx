namespace NaturalCron.Builder.Selectors;

public interface INaturalCronBuildSelector
{
    NaturalCronExpr Build();
    string BuildExpr();
}