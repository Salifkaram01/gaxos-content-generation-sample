using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models.DallE;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
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


                var dallETextToImageParameters = new DallETextToImageParameters();
                parameters.ApplyParameters(dallETextToImageParameters);
                ContentGenerationApi.Instance.RequestDallETextToImageGeneration(
                    dallETextToImageParameters,
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
                "var requestId = await ContentGenerationApi.Instance.RequestDallETextToImageGeneration\n" +
                "\t(new DallETextToImageParameters\n" +
                "\t{\n" +
                parameters?.GetCode() +
                "\t},\n" +
                $"{parameters.generationOptionsElement?.GetCode()}" +
                ")";
        }
    }
}