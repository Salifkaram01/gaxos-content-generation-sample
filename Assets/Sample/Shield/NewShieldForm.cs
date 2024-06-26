using System.Linq;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Comfy;
using Sample.Base;
using Sample.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Shield
{
    public class NewShieldForm : NewGeneratedImageForm
    {
        public const string ShieldSubject = "Shield";
        [SerializeField] Toggle[] _maskToggles;

        protected override Task RequestGeneration(string prompt)
        {
            var selectedMaskToggle = _maskToggles.First(t => t.isOn);
            var selectedMask = selectedMaskToggle.GetComponentInChildren<RawImage>();
            return ContentGenerationApi.Instance.RequestComfyMaskingGeneration
            (new ComfyMaskingParameters
                {
                    Prompt = prompt,
                    NSamples = 4,
                    Mask = (Texture2D)selectedMask.texture,
                },
                data: new
                {
                    ProfileSettings.playerId,
                    subject = ShieldSubject
                });
        }
    }
}