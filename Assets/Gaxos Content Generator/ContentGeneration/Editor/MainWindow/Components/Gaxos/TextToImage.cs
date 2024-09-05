using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class TextToImage : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<TextToImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        TextToImageParameters parameters => this.Q<TextToImageParameters>("parameters");

        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        TextField code => this.Q<TextField>("code");

        public TextToImage()
        {
            parameters.OnCodeHasChanged = RefreshCode;
            parameters.generationOptionsElement.OnCodeHasChanged = RefreshCode;
            
            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;

            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;

                if (!parameters.Valid())
                {
                    return;
                }

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;


                var gaxosTextToImageParameters = new GaxosTextToImageParameters();
                parameters.ApplyParameters(gaxosTextToImageParameters);
                ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration(
                    gaxosTextToImageParameters,
                    parameters.generationOptionsElement.GetGenerationOptions(), data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }).ContinueInMainThreadWith(
                    t =>
                    {
                        generateButton.SetEnabled(true);
                        sendingRequest.style.display = DisplayStyle.None;
                        if (t.IsFaulted)
                        {
                            requestFailed.style.display = DisplayStyle.Flex;
                            Debug.LogException(t.Exception);
                        }
                        else
                        {
                            requestSent.style.display = DisplayStyle.Flex;
                        }
                        ContentGenerationStore.Instance.RefreshRequestsAsync().Finally(() => ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog());
                    });
            });
            
            RefreshCode();
        }

        void RefreshCode()
        {
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration\n" +
                "\t(new GaxosTextToImageParameters\n" +
                "\t{\n" +
                parameters?.GetCode() +
                "\t},\n" +
                $"{parameters.generationOptionsElement?.GetCode()}" +
                ")";
        }
    }
}