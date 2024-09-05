using System;
using System.Collections.Generic;
using System.Linq;
using ContentGeneration.Helpers;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
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

        TextField code => this.Q<TextField>("code");
        Button generateButton => this.Q<Button>("generate");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");

        public TextToImage()
        {
            parameters.OnCodeHasChanged += RefreshCode;

            sendingRequest.style.display = DisplayStyle.None;
            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;

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

                var stabilityTextToImageParameters = new StabilityTextToImageParameters();
                parameters.ApplyParameters(stabilityTextToImageParameters);
                ContentGenerationApi.Instance.RequestStabilityTextToImageGeneration(
                    stabilityTextToImageParameters,
                    parameters.generationOptions.GetGenerationOptions(), data: new
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

            code.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            RefreshCode();
        }

        void RefreshCode()
        {
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestTextToImageGeneration\n" +
                "\t(new StabilityTextToImageParameters\n" +
                "\t{\n" +
                parameters.GetCode() +
                "\t},\n" +
                parameters.generationOptions.GetCode() +
                ")";
        }
    }
}