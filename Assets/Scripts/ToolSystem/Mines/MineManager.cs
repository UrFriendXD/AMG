﻿using System;
using System.Collections.Generic;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ToolSystem.Mines
{
    public class MineManager : ChunkShapeManager<Mine>
    {
        [SerializeField] private Vector2 rectSize;
        [SerializeField] private int minLayer;

        public UnityEvent<Mine> mineDefused = new UnityEvent<Mine>();
        public UnityEvent<Mine> mineDetonated = new UnityEvent<Mine>();

        public void Initialise(int minesToGenerate)
        {
            base.Initialise();
            
            List<Rect> boxes = new List<Rect>();
            List<Rect> bigBoxes = new List<Rect>();

            // Generate first box, centred on the transform
            var position = transform.position;
            
            bigBoxes.Add(new Rect(position.x + rectSize.x / -2, position.y + rectSize.y / -2, rectSize.x, rectSize.y));

            // Divide boxes
            for (int i = 0; i < minesToGenerate - 1; i++)
            {
                // Select a random box from bigBoxes
                var bigBoxIndex = Random.Range(0, bigBoxes.Count);

                var bigBox = bigBoxes[bigBoxIndex];

                bigBoxes.RemoveAt(bigBoxIndex);

                // Cut that box in half and add the halves to boxes
                if (bigBox.size.x > bigBox.size.y)
                {
                    // Cut vertically
                    boxes.Add(new Rect(bigBox.position, new Vector2(bigBox.size.x / 2, bigBox.size.y)));
                    boxes.Add(new Rect(new Vector2(bigBox.center.x, bigBox.position.y),
                        new Vector2(bigBox.size.x / 2, bigBox.size.y)));
                }
                else
                {
                    // Cut horizontally
                    boxes.Add(new Rect(bigBox.position, new Vector2(bigBox.size.x, bigBox.size.y / 2)));
                    boxes.Add(new Rect(new Vector2(bigBox.position.x, bigBox.center.y),
                        new Vector2(bigBox.size.x, bigBox.size.y / 2)));
                }

                // If bigboxes is empty, add all boxes to bigboxes
                if (bigBoxes.Count == 0)
                {
                    bigBoxes.AddRange(boxes);

                    boxes.Clear();
                }
            }

            boxes.AddRange(bigBoxes);

            var spriteRendererBounds = chunkShapePrefab.GetComponent<SpriteRenderer>().bounds;

            var buffer = new Vector2(
                spriteRendererBounds.extents.x,
                spriteRendererBounds.extents.y
            );

            for (int i = 0; i < minesToGenerate; i++)
            {
                Vector2 randomPosInBox = RandomPosInRect(boxes[i], buffer);
                
                var chunkShape = CreateChunkShape(
                    () => Instantiate(chunkShapePrefab, randomPosInBox, Quaternion.identity, transform),
                    chunkShape =>
                    {
                        Mine mine = (Mine) chunkShape;

                        // TODO: Has been moved before initialisation to allow mines defused during initialisation to be removed from the xray
                        mine.defused.AddListener(OnMineDefused);
                        mine.detonated.AddListener(OnMineDetonated);
                    },
                    mine => mine.Initialise(Random.Range(minLayer, ChunkManager.Size.z))
                );

                chunkShape.name = $"Mine {i + 1}";
            }
        }

        protected override Mine CreateChunkShape(Func<GameObject> instantiationFunction, Action<ChunkShape> preinitialisationAction, Action<Mine> initialisationAction)
        {
            var chunkShape = base.CreateChunkShape(instantiationFunction, preinitialisationAction, initialisationAction);

            return chunkShape;
        }
        
        protected override void DestroyChunkShape(Mine mine)
        {
            base.DestroyChunkShape(mine);
            
            mine.defused.RemoveListener(OnMineDefused);
            mine.detonated.RemoveListener(OnMineDetonated);
        }

        private void OnMineDefused(Mine mine)
        {
            mineDefused.Invoke(mine);
        }

        private void OnMineDetonated(Mine mine)
        {
            mineDetonated.Invoke(mine);
        }

        private Vector2 RandomPosInRect(Rect rect, Vector2 buffer)
        {
            var randomX = Random.Range(rect.x + buffer.x, rect.x + rect.size.x - buffer.x);
            var randomY = Random.Range(rect.y + buffer.y, rect.y + rect.size.y - buffer.y);

            return new Vector2(randomX, randomY);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position, new Vector3(rectSize.x, rectSize.y, 1));
        }
    }
}