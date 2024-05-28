using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Stability;
using Sample.Base;
using Sample.Common;

namespace Sample.Flag
{
    public class NewFlagForm : NewGeneratedImageForm
    {
        public const string FlagSubject = "Flag";

        protected override Task RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestStabilityTextToImageGeneration
            (new StabilityTextToImageParameters
            {
                TextPrompts = new[]
                {
                    new Prompt
                    {
                        Text = prompt,
                        Weight = 1,
                    },
                    new Prompt
                    {
                        Text = "Flat flag",
                        Weight = 0.5f,
                    },
                    new Prompt
                    {
                        Text = "High quality",
                        Weight = 0.5f,
                    }
                },
                Samples = 4,
                StylePreset = "digital-art"
            }, data: new
            {
                ProfileSettings.playerId,
                subject = FlagSubject
            });
        }
    }
}