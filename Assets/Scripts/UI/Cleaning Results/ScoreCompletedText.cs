﻿using System.Globalization;
using Cleaning;
using Managers;
using RockSystem.Artefacts;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning_Results
{
    public class ScoreCompletedText : DialogueComponent<CleaningCompletedResultsDialogue>
    {
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI artefactsCleanedText;
        [SerializeField] private TextMeshProUGUI artefactsPerfectedText;
        [SerializeField] private TextMeshProUGUI averageHealthText;
        [SerializeField] private TextMeshProUGUI averageExposureText;
        [SerializeField] private TextMeshProUGUI totalTimeText;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private CleaningScoreManager scoreManager;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            scoreManager = M.GetOrThrow<CleaningScoreManager>();

            // artefactHealthText.text =
            //     Mathf.Round(artefactShape.ArtefactHealth * 100).ToString(CultureInfo.InvariantCulture) + "%";
            // artefactExposureText.text =
            //     Mathf.Round(artefactShape.ArtefactExposure * 100).ToString(CultureInfo.InvariantCulture) + "%";
            // //timeRemainingText.text = timerManager.CurrentTime.ToString(CultureInfo.InvariantCulture);
            Debug.Log(artefactsCleanedText);
            artefactsCleanedText.text = scoreManager.ArtefactsCleaned.ToString(CultureInfo.InvariantCulture);
            Debug.Log(artefactsCleanedText.text);
            Debug.Log(scoreManager.ArtefactsCleaned);
            artefactsPerfectedText.text = scoreManager.ArtefactsPerfected.ToString(CultureInfo.InvariantCulture);
            averageHealthText.text = Mathf
                .Round(scoreManager.TotalArtefactsHealth / scoreManager.ArtefactsCleaned * 100)
                .ToString(CultureInfo.InvariantCulture) + "%";
            averageExposureText.text = Mathf
                .Round(scoreManager.TotalArtefactsExposure / scoreManager.ArtefactsCleaned * 100)
                .ToString(CultureInfo.InvariantCulture) + "%";
            totalScoreText.text = scoreManager.Score.ToString(CultureInfo.InvariantCulture);
            totalTimeText.text = timerManager.TotalTime.ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        // TODO
        private string WinStateToString()
        {
            return "Times Up!";

            // switch (cleaningManager.CurrentCleaningState)
            // {
            //     case CleaningManager.CleaningState.Lost:
            //         return "Better luck next time...";
            //     case CleaningManager.CleaningState.Won:
            //         return "Success!";
            //     default:
            //         Debug.Log($"Invalid {nameof(CleaningManager.CleaningState)} {cleaningManager.CurrentCleaningState}.");
            //         return "Invalid State";
            // }
        }
    }
}