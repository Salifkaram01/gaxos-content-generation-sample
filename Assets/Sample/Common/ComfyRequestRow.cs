using ContentGeneration.Models;
using Sample.Base;

namespace Sample.Common
{
    public class ComfyRequestRow : RequestRow
    {
        protected override string GetPrompt(Request request)
        {
            return request.GeneratorParameters["prompt"]!.ToObject<string>();
        }
    }
}