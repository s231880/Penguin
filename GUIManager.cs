using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace PenguinRun
{
    public class GUIManager : MonoBehaviour
    {
        private TextMeshProUGUI m_ScoreText;
        public void Initialise()
        {
            m_ScoreText = this.transform.GetComponentInChildren<TextMeshProUGUI>();
        }
        public void SetScore(string score)
        {
            m_ScoreText.text = score;
        }

        public void ShowEndGameScreen() { }
    }
}
