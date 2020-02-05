using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Pommel.Reversi
{
    public sealed class Field : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_parent;

        [SerializeField]
        private GameObject m_stonePrefabBase;

        private IEnumerable<Stone> m_stones = Enumerable.Empty<Stone>();

        private const int NUMBER_OF_PLACEABLE_STONES = 64;

        private void Awake()
        {
            m_stones = Enumerable.Range(0, NUMBER_OF_PLACEABLE_STONES)
                .Select(_ => Instantiate(m_stonePrefabBase, m_parent.position, Quaternion.identity, m_parent).GetComponent<Stone>())
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
                    foreach (var (stone, stoneState) in m_stones.Zip(
                        state.Stones.SelectMany(stones => stones),
                        (stone, stoneState) => (stone, stoneState)))
                    {
                        switch (stoneState.Color)
                        {
                            case StoneStateElement.State.None when stone.IsNone: continue;
                            case StoneStateElement.State.Black when stone.IsBlack: continue;
                            case StoneStateElement.State.White when stone.IsWhite: continue;

                            case StoneStateElement.State.None:
                                stone.None();
                                continue;

                            case StoneStateElement.State.Black:
                                stone.TurnBlack();
                                continue;

                            case StoneStateElement.State.White:
                                stone.TurnWhite();
                                continue;
                        }

                        throw new ArgumentOutOfRangeException();
                    }
                })
                .AddTo(this);
        }
    }
}
