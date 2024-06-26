using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Comfy;
using Sample.Base;
using Sample.Common;

namespace Sample.Flag
{
    public class NewFlagForm : NewGeneratedImageForm
    {
        public const string FlagSubject = "Flag";

        protected override Task RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestComfyTextToImageGeneration
            (new ComfyTextToImageParameters
            {
                Prompt = prompt,
                NegativePrompt = "Background, 3D, low quality, blurry",
                NSamples = 4,
            }, data: new
            {
                ProfileSettings.playerId,
                subject = FlagSubject
            });
        }
    }
}