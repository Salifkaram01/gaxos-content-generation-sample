using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using Newtonsoft.Json.Linq;
using Sample.Common;
using UnityEngine;

namespace Sample.Body
{
    [RequireComponent(typeof(Renderer))]
    public class BodyMaterialTextureGroup : MonoBehaviour
    {
        Renderer _renderer;
        Texture _defaultBaseColor;
        Texture _defaultNormal;
        Texture _defaultSpecular;

        static readonly int MainTex = Shader.PropertyToID("_MainTex");
        static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        static readonly int SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _defaultBaseColor = _renderer.sharedMaterial.GetTexture(MainTex);
            _defaultNormal = _renderer.sharedMaterial.GetTexture(BumpMap);
            _defaultSpecular = _renderer.sharedMaterial.GetTexture(SpecGlossMap);
            ProfileSettings.OnBodyChanged += RefreshImage;
            RefreshImage(ProfileSettings.body);
        }

        CancellationTokenSource _cancellationTokenSource;

        [SerializeField] int _textureIndexInGenerationResult;

        void RefreshImage(PublishedAsset asset)
        {
            _renderer.material.SetTexture(MainTex, _defaultBaseColor);
            _renderer.material.SetTexture(BumpMap, _defaultNormal);
            _renderer.material.SetTexture(SpecGlossMap, _defaultSpecular);
            _cancellationTokenSource?.Cancel();
            var thisCancellationToken = _cancellationTokenSource = new CancellationTokenSource();
            var textureGroup =
                asset?.Request.GeneratorResult?["texture_urls"]?[_textureIndexInGenerationResult];
            if (textureGroup != null)
            {
                RefreshImageAsync(textureGroup, thisCancellationToken.Token).CatchAndLog(this);
            }
        }

        async Task RefreshImageAsync(JToken textureGroup, CancellationToken token)
        {
            Texture2D baseColor = null;
            if (textureGroup["base_color"] != null)
            {
                baseColor = await TextureHelper.DownloadImage(textureGroup["base_color"].ToObject<string>(), token);
            }

            if (token.IsCancellationRequested)
                return;
            Texture2D normalMap = null;
            if (textureGroup["normal"] != null)
            {
                normalMap = await TextureHelper.DownloadImage(textureGroup["normal"].ToObject<string>(), token);
            }

            if (token.IsCancellationRequested)
                return;
            Texture2D specularMap = null;
            if (textureGroup["roughness"] != null)
            {
                specularMap = await TextureHelper.DownloadImage(textureGroup["roughness"].ToObject<string>(), token);
            }

            if (token.IsCancellationRequested)
                return;

            if (baseColor != null)
            {
                _renderer.material.SetTexture(MainTex, baseColor);
            }

            if (normalMap != null)
            {
                _renderer.material.SetTexture(BumpMap, normalMap);
            }

            if (specularMap != null)
            {
                _renderer.material.SetTexture(SpecGlossMap, specularMap);
            }
        }
    }
}