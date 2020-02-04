using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

namespace Pommel.Reversi
{
    public sealed class Field : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_parent;

        [SerializeField]
        private GameObject m_stonePrefabBase;

        private GameObject[] m_stones = new GameObject[] { };

        private const int NUMBER_OF_PLACEABLE_STONES = 64;

        private void Awake()
        {
            m_stones = Enumerable.Range(0, NUMBER_OF_PLACEABLE_STONES)
                .Select(_ => Instantiate(m_stonePrefabBase, m_parent.position, Quaternion.identity, m_parent))
                .ToArray();
        }

        private void OnEnable()
        {
            _ = Unidux
                .Subject
                .TakeUntilDisable(this)
                .StartWith(Unidux.State)
                .Subscribe(state =>
                {
                    foreach (var (stoneGameObject, stoneState) in m_stones.Zip(
                        state.Stones.SelectMany(stones => stones),
                        (stoneGameObject, stoneState) => (stoneGameObject, stoneState)))
                    {
                        var image = stoneGameObject.GetComponent<Image>();

                        switch (stoneState.Color)
                        {
                            case StoneStateElement.State.None when image.color == Define.NONE_COLOR: continue;
                            case StoneStateElement.State.Black when image.color == Color.black: continue;
                            case StoneStateElement.State.White when image.color == Color.white: continue;

                            case StoneStateElement.State.None:
                                image.color = Define.NONE_COLOR;
                                continue;

                            case StoneStateElement.State.Black:
                                image.color = Color.black;
                                continue;

                            case StoneStateElement.State.White:
                                image.color = Color.white;
                                continue;
                        }

                        throw new ArgumentOutOfRangeException();
                    }
                })
                .AddTo(this);
        }
    }

    public static class StringExtension
    {
        public static Color ToColor(this string self) =>
            ColorUtility.TryParseHtmlString(self, out var color)
                ? color
                : default;
    }

    public static class Define
    {
        public static readonly Color NONE_COLOR = "#13E70E".ToColor();
    }
}
