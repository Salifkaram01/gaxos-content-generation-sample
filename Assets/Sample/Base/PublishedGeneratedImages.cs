using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using Sample.Common;
using UnityEngine;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    public abstract class PublishedGeneratedImages : MonoBehaviour
    {
        protected abstract string subject { get; }

        CancellationTokenSource _cancellationTokenSource;
        void OnEnable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            RefreshAsync(_cancellationTokenSource.Token).CatchAndLog(this);
        }

        [SerializeField] PublishedImageRow _publishedImageRow;
        async Task RefreshAsync(CancellationToken token)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            var publishedImages = await ContentGenerationApi.Instance.GetPublishedAssets(
                /*TODO:
                 new
                {
                    playerId = PlayerId.value,
                    subject = _subject
                }*/);

            if (token.IsCancellationRequested)
            {
                return;
            }
            
            foreach (var publishedImage in publishedImages)
            {
                if (publishedImage.Request.Data?["playerId"] == null || publishedImage.Request.Data["subject"] == null)
                {
                    continue;
                }
                if (publishedImage.Request.Data["playerId"].ToObject<string>() != ProfileSettings.playerId ||
                    publishedImage.Request.Data["subject"].ToObject<string>() != subject) continue;
                
                var publishedImageRow = Instantiate(_publishedImageRow, transform);
                publishedImageRow.SetPublishedImage(publishedImage);
            }
        }
    }
}