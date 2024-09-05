using System;
using System.Collections.Generic;
using ContentGeneration.Models.Gaxos;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class TextToImageParameters : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<TextToImageParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        SliderInt width => this.Q<SliderInt>("width");
        SliderInt height => this.Q<SliderInt>("height");

        GaxosParametersElement gaxosParametersElement => this.Q<GaxosParametersElement>("gaxosParametersElement");
        public GenerationOptionsElement generationOptionsElement => this.Q<GenerationOptionsElement>("generationOptions");

        public TextToImageParameters()
        {
            gaxosParametersElement.OnCodeHasChanged = CodeHasChanged;
            width.RegisterValueChangedCallback(_ => CodeHasChanged());
            height.RegisterValueChangedCallback(_ => CodeHasChanged());
            CodeHasChanged();
        }

        public Action OnCodeHasChanged;
        void CodeHasChanged()
        {
            OnCodeHasChanged?.Invoke();
        }

        public bool Valid()
        {
            if (!gaxosParametersElement.Valid())
            {
                return false;
            }

            return true;
        }

        public void ApplyParameters(GaxosTextToImageParameters gaxosParameters)
        {
            gaxosParameters.Width = (uint)width.value;
            gaxosParameters.Height = (uint)height.value;
            gaxosParametersElement.ApplyParameters(gaxosParameters);
        }

        public string GetCode()
        {
            return gaxosParametersElement.GetCode() +
                   $"\t\tWidth = {width.value}\n" +
                   $"\t\tHeight = {height.value}\n";
        }
    }
}
