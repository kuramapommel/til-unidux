using System.Collections.Generic;
using Unidux.SceneTransition;
using static Pommel.Reversi.Domain.Scene.ValueObjects;

namespace Pommel
{
    public sealed class SceneConfig : ISceneConfig<Scene, Page>
    {
        public IDictionary<Scene, int> CategoryMap { get; } = new Dictionary<Scene, int>()
        {
            { Scene.Base, SceneCategory.Permanent },
            { Scene.Title, SceneCategory.Page },
            { Scene.InGame, SceneCategory.Page },
        };

        public IDictionary<Page, Scene[]> PageMap { get; } = new Dictionary<Page, Scene[]>()
        {
            {
                Page.Title, new Scene[]
                {
                    Scene.Title
                }
            },
            {
                Page.InGame, new Scene[]
                {
                    Scene.InGame
                }
            }
        };
    }
}