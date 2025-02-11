﻿using Managers;
using RockSystem.Artefacts;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningTimerManager : Manager
    {
        [SerializeField] private float startTime;
        [Tooltip("The amount of time added to the clock based on artefact health.")]
        [SerializeField] private AnimationCurve artefactRockCompletionBonusCurve;

        public UnityEvent timeChanged = new UnityEvent();

        public float CurrentTimeLeft
        {
            get => currentTimeLeft;
            private set
            {
                currentTimeLeft = value;
                
                timeChanged.Invoke();
            }
        }

        public float BonusTime { get; private set; }
        public float TotalTime { get; private set; }
        public float TimeTaken { get; private set; } 

        private CleaningManager cleaningManager;
        private ArtefactShapeManager artefactShapeManager;

        private bool timerActive;
        private float currentTimeLeft;
        private float prevRockTime;

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();

            TotalTime = startTime;
            prevRockTime = startTime;
            TimeTaken = 0f;

            cleaningManager.cleaningStarted.AddListener(ResetAndStartTimer);
            cleaningManager.cleaningEnded.AddListener(StopTimer);
            cleaningManager.artefactRockCompleted.AddListener(OnArtefactRockCompleted);
            cleaningManager.nextArtefactRockStarted.AddListener(OnNextArtefactRockStarted);
            cleaningManager.cleaningPaused.AddListener(StopTimer);
            cleaningManager.cleaningResumed.AddListener(StartTimer);
        }

        private void OnNextArtefactRockStarted()
        {
            TimeTaken = 0f;
        }

        private void OnArtefactRockCompleted()
        {
            BonusTime = artefactRockCompletionBonusCurve.Evaluate(artefactShapeManager.Health);
            TimeTaken = prevRockTime - CurrentTimeLeft;
            CurrentTimeLeft += BonusTime;
            TotalTime += BonusTime;
            prevRockTime = CurrentTimeLeft;
        }

        protected override void Update()
        {
            base.Update();
            
            if (!timerActive) return;
            
            CurrentTimeLeft -= Time.deltaTime;

            if (!(CurrentTimeLeft <= 0)) return;
            
            CurrentTimeLeft = 0;

            timerActive = false;
            
            cleaningManager.EndCleaning();
        }

        public void ResetTimer()
        {
            CurrentTimeLeft = startTime;
        }

        public void StartTimer()
        {
            // Prevents game ending when pausing the game without having started cleaning
            // Can remove later if the cleaning phase doesn't start paused
            if (CurrentTimeLeft != 0)
            {
                timerActive = true;
            }
        }

        public void StopTimer()
        {
            timerActive = false;
        }

        public void ResetAndStartTimer()
        {
            ResetTimer();
            StartTimer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.cleaningStarted.RemoveListener(ResetAndStartTimer);
            cleaningManager.cleaningEnded.RemoveListener(StopTimer);
            cleaningManager.artefactRockCompleted.RemoveListener(OnArtefactRockCompleted);
            cleaningManager.cleaningPaused.RemoveListener(StopTimer);
            cleaningManager.cleaningResumed.RemoveListener(StartTimer);
        }
    }
}