using System;
using System.Collections.Generic;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class MaskingParameters : VisualElementComponent, IParameters<GaxosMaskingParameters>
    {
        public new class UxmlFactory : UxmlFactory<MaskingParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        GaxosParametersElement gaxosParametersElement => this.Q<GaxosParametersElement>("gaxosParametersElement");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");
        ImageSelection mask => this.Q<ImageSelection>("mask");
        Label maskRequired => this.Q<Label>("maskRequiredLabel");

        public MaskingParameters()
        {
            gaxosParametersElement.OnCodeHasChanged = CodeHasChanged;
            maskRequired.style.visibility = Visibility.Hidden;
            CodeHasChanged();
        }
        
        public Action OnCodeHasChanged { get; set; }
        void CodeHasChanged()
        {
            OnCodeHasChanged?.Invoke();
        }

        public bool Valid()
        {
            maskRequired.style.visibility = Visibility.Hidden;
            if (!gaxosParametersElement.Valid())
            {
                return false;
            }
            if (mask.image == null)
            {
                maskRequired.style.visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        public void ApplyParameters(GaxosMaskingParameters gaxosParameters)
        {
            gaxosParameters.Mask = (Texture2D)mask.image;
            gaxosParametersElement.ApplyParameters(gaxosParameters);
        }

        public string GetCode()
        {
            return gaxosParametersElement.GetCode() +
                   "\t\tMask = <Texture2D object>,\n";
        }
    }
}