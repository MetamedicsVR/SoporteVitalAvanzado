using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebriefManager : MonoBehaviour
{
    public Image timelineImage; // Asumiendo que estás usando una imagen para el timeline
    public float lerpDuration = 5f; // Tiempo que tardará el lerp
    private float timelineFill = 0f;

    public GameObject satisfactionPanel;

    public GameObject timelinePanel;

    public int satisfactionLevel;

    public GameObject[] correctTimeStamps;
    public GameObject[] incorrectTimeStamps;

    // Start is called before the first frame update
    void Start()
    {
        satisfactionPanel.SetActive(true);
    }

    public void SetSatisfactionLevel(int incomingSatisfactionLevel) 
    {
        satisfactionLevel = incomingSatisfactionLevel;
        satisfactionPanel.GetComponentInChildren<Animator>().Play("PanelDialogoDesaparece");

    }
 
    public void OpenTimeLine() 
    {
        satisfactionPanel.SetActive(false);
        timelinePanel.SetActive(true);
        for (int i = 0; i < correctTimeStamps.Length; i++)
        {
            correctTimeStamps[i].SetActive(false);
        }
        StartCoroutine(LerpTimelineFill(lerpDuration));
    }
    IEnumerator LerpTimelineFill(float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timelineFill = Mathf.Lerp(0f, 1f, timeElapsed / duration);
            timelineImage.fillAmount = timelineFill; // Si estás modificando el fillAmount de una imagen
            timeElapsed += Time.deltaTime;

            yield return null; // Espera un frame
        }

        // Asegúrate de que la variable termine exactamente en 1
        timelineFill = 1f;
        timelineImage.fillAmount = timelineFill;
    }
}
