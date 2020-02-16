﻿using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class BoardRenderer : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_parent;

        [SerializeField]
        private GameObject m_stonePrefabBase;

        private IEnumerable<StoneDispatcher> m_stones = Enumerable.Empty<StoneDispatcher>();

        private readonly (int x, int y) X_Y = (8, 8);

        private void Awake()
        {
            m_stones = Enumerable.Range(0, X_Y.x * X_Y.y)
                .Select(index =>
                {
                    var stone = Instantiate(m_stonePrefabBase, m_parent.position, Quaternion.identity, m_parent).GetComponent<StoneDispatcher>();
                    stone.Constructor(index / X_Y.x, index % X_Y.y);
                    return stone;
                })
                .ToArray();

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
