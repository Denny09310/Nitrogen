using Nitrogen.Core.Utils;

namespace Nitrogen.Core.Interpreting;

public interface IModuleEvaluator
{
    Module Evaluate(string content);
}