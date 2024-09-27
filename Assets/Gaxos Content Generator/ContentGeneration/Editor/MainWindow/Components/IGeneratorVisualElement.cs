using ContentGeneration.Models;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public interface IGeneratorVisualElement
    {
        Generator generator { get; }
        void Show(JObject generatorParameters);
    }
}