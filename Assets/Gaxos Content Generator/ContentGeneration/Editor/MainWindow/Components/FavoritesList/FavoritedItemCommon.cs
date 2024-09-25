using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.FavoritesList
{
    public class FavoritedItemCommon : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<FavoritedItemCommon, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(VisualElement)); }
            }
        }

        SubWindow favoritedItem => this.Q<SubWindow>("favoritedItem");
        Label generator => this.Q<Label>("generator");
        TextField generatorParameters => this.Q<TextField>("generatorParameters");
        Button deleteButton => this.Q<Button>("deleteButton");

        public override VisualElement contentContainer => this.Q<VisualElement>("childrenContainer");

        public FavoritedItemCommon()
        {
            deleteButton.clicked += () =>
            {
                if (deleteButton.enabledSelf)
                {
                    deleteButton.SetEnabled(false);
                    // TODO:
                    // ContentGenerationApi.Instance.DeleteFavorite(value.ID).ContinueInMainThreadWith(t =>
                    // {
                    //     if (t.IsFaulted)
                    //     {
                    //         Debug.LogException(t.Exception!.InnerException);
                    //     }
                    //     else
                    //     {
                    //         OnDeleted?.Invoke();
                    //     }
                    //
                    //     deleteButton.SetEnabled(true);
                    // });
                }
            };
            generatorParameters.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
        }

        public event Action OnDeleted;
        Favorite _value;

        public Favorite value
        {
            get => _value;
            set
            {
                _value = value;
                if (value == null)
                    return;

                switch (value.Generator)
                {
                    case Generator.StabilityTextToImage:
                        favoritedItem.subWindowName = "Stability AI Text To Image";
                        favoritedItem.subWindowIcon = "Stability AI";
                        break;
                    case Generator.StabilityImageToImage:
                        favoritedItem.subWindowName = "Stability AI Image To Image";
                        favoritedItem.subWindowIcon = "Stability AI";
                        break;
                    case Generator.StabilityMasking:
                        favoritedItem.subWindowName = "Stability AI Masking";
                        favoritedItem.subWindowIcon = "Stability AI";
                        break;
                    case Generator.DallETextToImage:
                        favoritedItem.subWindowName = "Dall-E Text To Image";
                        favoritedItem.subWindowIcon = "Dall-E";
                        break;
                    case Generator.DallEInpainting:
                        favoritedItem.subWindowName = "Dall-E Inpainting";
                        favoritedItem.subWindowIcon = "Dall-E";
                        break;
                    case Generator.MeshyTextToMesh:
                    default:
                        favoritedItem.subWindowName = value.Generator.ToString();
                        favoritedItem.subWindowIcon = null;
                        break;
                }

                generator.text = value.Generator.ToString();

                generatorParameters.value = value.GeneratorParameters?.ToString();
            }
        }
    }
}