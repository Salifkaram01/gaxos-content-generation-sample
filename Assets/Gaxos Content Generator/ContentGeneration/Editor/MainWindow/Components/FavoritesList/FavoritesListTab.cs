using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.FavoritesList
{
    public class FavoritesListTab : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<FavoritesListTab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button refreshButton => this.Q<Button>("refreshButton");
        MultiColumnListView listView => this.Q<MultiColumnListView>();
        FavoritedItem favoritedItem => this.Q<FavoritedItem>("defaultFavoritedItem");

        string _selectedId;

        public FavoritesListTab()
        {
            refreshButton.RegisterCallback<ClickEvent>(_ => { Refresh(); });

            listView.itemsSource = _favorites;
            if (!string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                Refresh();
            }

            Func<VisualElement> CreateCell(int i1)
            {
                return () =>
                {
                    var ret = new Label
                    {
                        name = $"p{i1}"
                    };
                    ret.AddToClassList(ret.name);

                    return ret;
                };
            }

            for (var i = 0; i < listView.columns.Count; i++)
            {
                var listViewColumn = listView.columns[i];
                listViewColumn.makeCell = CreateCell(i);
            }

            listView.columnSortingChanged += Refresh;
            listView.columns["id"].bindCell = (element, index) =>
                (element as Label)!.text = _favorites[index].ID.ToString();
            listView.columns["generator"].bindCell = (element, index) =>
                (element as Label)!.text = _favorites[index].Generator.ToString();
            listView.columns["creditsCost"].bindCell = (element, index) =>
            {
                var label = (element as Label)!;
                label.text = _favorites[index].DeductedCredits
                    .ToString(CultureInfo.InvariantCulture);
            };

            favoritedItem.OnDeleted += () =>
            {
                listView.selectedIndex = -1;
                Refresh();
            };
            favoritedItem.style.display = DisplayStyle.None;

            listView.selectionChanged += objects =>
            {
                _selectedId = null;
                var objectsArray = objects.ToArray();
                favoritedItem.value = null;

                if (objectsArray.Length > 0)
                {
                    var favorite = (objectsArray[0] as Favorite)!;
                    _selectedId = favorite.ID;
                    favoritedItem.value = favorite;
                }

                favoritedItem.style.display =
                    favoritedItem.value == null ? DisplayStyle.None : DisplayStyle.Flex;
            };
        }

        Favorite[] _favorites;
        void Refresh()
        {
            ContentGenerationApi.Instance.GetFavorites().ContinueInMainThreadWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.GetBaseException());
                    return;
                }

                var previousSelectId = _selectedId;
                _favorites = t.Result;
                listView.RefreshItems();
                listView.selectedIndex = -1;
                if (previousSelectId != null)
                {
                    for (var i = 0; i < _favorites.Length; i++)
                    {
                        if (_favorites[i].ID == previousSelectId)
                        {
                            listView.selectedIndex = i;
                            break;
                        }
                    }
                }
            });
        }
    }
}