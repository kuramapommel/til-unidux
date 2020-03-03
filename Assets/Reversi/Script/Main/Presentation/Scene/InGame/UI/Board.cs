using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Pommel.Reversi.Presentation.Scene.InGame.Renderer;

namespace Pommel.Reversi.Presentation.Scene.InGame.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Board : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private RectTransform m_parent;

        [SerializeField]
        private Stone m_stonePrefab;

        private IEnumerable<Stone> m_stones;

        private IDisposable m_disposable;

        private readonly (int x, int y) Coordinate = (8, 8);

        public void Initialize()
        {
            m_stones = Enumerable.Range(0, Coordinate.x * Coordinate.y)
                .Select(index =>
                {
                    var stone = Instantiate(m_stonePrefab, m_parent.position, Quaternion.identity, m_parent);
                    stone.Initialize(index / Coordinate.x, index % Coordinate.y);
                    return stone;
                })
                .ToArray();

            m_disposable = this.ApplySwitchStoneColorRendering(m_stones);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
            m_disposable = null;
        }
    }
}
