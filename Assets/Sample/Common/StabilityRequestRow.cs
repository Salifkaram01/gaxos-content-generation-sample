using ContentGeneration.Models;
using Sample.Base;

namespace Sample.Common
{
    public class StabilityRequestRow : RequestRow
    {
        protected override string GetPrompt(Request request)
        {
            return request.GeneratorParameters["text_prompts"]!.First!["text"]!.ToObject<string>();
        }
    }
}