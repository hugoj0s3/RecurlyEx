namespace NaturalCron.Builder.Selectors;

/// <summary>
/// Build operations interface for the NaturalCron fluent builder API.
/// Provides methods to finalize and output the constructed expression,
/// either as a validated NaturalCronExpr or as a raw string for debugging.
/// </summary>
public interface INaturalCronBuildSelector
{
    /// <summary>
    /// Builds and validates the constructed expression, returning a NaturalCronExpr instance.
    /// Throws an ArgumentException if the expression syntax is invalid.
    /// </summary>
    /// <returns>A validated NaturalCronExpr instance ready for scheduling</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is invalid</exception>
    NaturalCronExpr Build();
    
    /// <summary>
    /// Returns the raw expression string without parsing or validation.
    /// Useful for debugging, logging
    /// </summary>
    /// <returns>Raw expression string (may be invalid)</returns>
    string ToRawExpression();
}