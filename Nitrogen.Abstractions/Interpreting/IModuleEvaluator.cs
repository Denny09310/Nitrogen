using Nitrogen.Abstractions.Utils;

namespace Nitrogen.Abstractions.Interpreting;

public interface IModuleEvaluator
{
    Module Evaluate(string content);
}