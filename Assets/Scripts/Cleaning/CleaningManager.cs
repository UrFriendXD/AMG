﻿using System.Linq;
using Managers;
using RockSystem.Chunks;
using RockSystem.Fossils;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningManager : Manager
    {
        public enum CleaningState
        {
            InProgress,
            Won,
            Lost
        }

        [SerializeField] private GenerationBracket generationBracket;
        [SerializeField] private float RequiredExposureForCompletion;
        [SerializeField] private float RequiredHealthForFailure;
 
        public CleaningState CurrentCleaningState { get; private set; }

        public UnityEvent CleaningStarted = new UnityEvent();
        public UnityEvent CleaningEnded = new UnityEvent();
        public UnityEvent CleaningWon = new UnityEvent();
        public UnityEvent CleaningLost = new UnityEvent();

        private ChunkManager chunkManager;
        private FossilShape fossilShape;

        public ArtefactRock CurrentArtefactRock { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();

            chunkManager = M.GetOrThrow<ChunkManager>();
            fossilShape = M.GetOrThrow<FossilShape>();
        }

        public void StartCleaning()
        {
            CurrentCleaningState = CleaningState.InProgress;
            
            fossilShape.fossilExposed.AddListener(CheckIfCleaningWon);
            fossilShape.fossilDamaged.AddListener(CheckIfCleaningLost);

            CurrentArtefactRock = GenerateArtefactRock(generationBracket);
            chunkManager.Initialise(CurrentArtefactRock.RockShape, CurrentArtefactRock.RockColor, CurrentArtefactRock.ChunkDescription);
            fossilShape.Initialise(CurrentArtefactRock.Antiquity);
            
            CleaningStarted.Invoke();
        }

        public ArtefactRock GenerateArtefactRock(GenerationBracket generationBracket)
        {
            var antiquities = generationBracket.antiquities;
            var rockShapes = generationBracket.rockShapes;
            var chunkDescriptions = generationBracket.chunkDescriptions;
            
            return new ArtefactRock(
                antiquities.ElementAtOrDefault(Random.Range(0, antiquities.Count)),
                rockShapes.ElementAtOrDefault(Random.Range(0, rockShapes.Count)),
                chunkDescriptions.ElementAtOrDefault(Random.Range(0, chunkDescriptions.Count)),
                generationBracket.rockColor
            );
        }
        
        private void EndCleaning()
        {
            fossilShape.fossilExposed.RemoveListener(CheckIfCleaningWon);
            fossilShape.fossilDamaged.RemoveListener(CheckIfCleaningLost);
            
            CleaningEnded.Invoke();
        }

        public void LoseCleaning()
        {
            CurrentCleaningState = CleaningState.Lost;
            
            EndCleaning();
            
            CleaningLost.Invoke();
        }

        public void WinCleaning()
        {
            CurrentCleaningState = CleaningState.Won;
            
            chunkManager.HideRock();
            
            EndCleaning();
            
            CleaningWon.Invoke();
        }

        public void CheckIfCleaningLost()
        {
            if (fossilShape.FossilHealth < RequiredHealthForFailure)
                LoseCleaning();
        }

        public void CheckIfCleaningWon()
        {
            if (fossilShape.FossilExposure > RequiredExposureForCompletion)
                WinCleaning();
        }
    }
}