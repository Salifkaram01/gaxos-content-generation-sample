using System.Linq;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using Sample.Base;
using Sample.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Shield
{
    public class NewShieldForm : NewGeneratedImageForm
    {
        public const string ShieldSubject = "Shield";
        [SerializeField] Texture2D _backgroundImage;
        [SerializeField] Toggle[] _maskToggles;

        protected override Task RequestGeneration(string prompt)
        {
            var selectedMaskToggle = _maskToggles.First(t => t.isOn);
            var selectedMask = selectedMaskToggle.GetComponentInChildren<RawImage>();
            return ContentGenerationApi.Instance.RequestStabilityMaskedImageGeneration
            (new StabilityMaskedImageParameters
                {
                    TextPrompts = new[]
                    {
                        new Prompt
                        {
                            Text = prompt,
                            Weight = 1f,
                        }
                    },
                    Samples = 4,
                    InitImage = _backgroundImage,
                    MaskImage = (Texture2D)selectedMask.texture,
                    MaskSource = MaskSource.MaskImageWhite,
                    StylePreset = "digital-art",
                },
                new GenerationOptions()
                {
                    TransparentColor = Color.magenta
                },
                new
                {
                    ProfileSettings.playerId,
                    subject = ShieldSubject
                });
        }
    }
}