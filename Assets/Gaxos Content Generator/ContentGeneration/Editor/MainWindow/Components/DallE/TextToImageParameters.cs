using System;
using System.Collections.Generic;
using ContentGeneration.Models.DallE;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
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

        DallEParametersElement dallEParametersElement => this.Q<DallEParametersElement>("dallEParametersElement");
        public GenerationOptionsElement generationOptionsElement => this.Q<GenerationOptionsElement>("generationOptions");

        public TextToImageParameters()
        {
            dallEParametersElement.OnCodeChanged += CodeHasChanged;
            generationOptionsElement.OnCodeHasChanged = CodeHasChanged;
            CodeHasChanged();
        }

        public Action OnCodeHasChanged;
        void CodeHasChanged()
        {
            OnCodeHasChanged?.Invoke();
        }

        public bool Valid()
        {
            if (!dallEParametersElement.Valid())
            {
                return false;
            }

            return true;
        }

        public void ApplyParameters(DallETextToImageParameters dallEParameters)
        {
            dallEParametersElement.ApplyParameters(dallEParameters);
        }

        public string GetCode()
        {
            return dallEParametersElement?.GetCode();
        }
    }
}