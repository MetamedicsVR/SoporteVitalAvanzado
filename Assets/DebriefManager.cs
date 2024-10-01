using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DebriefManager : MonoBehaviour
{
    public Image timelineImage; // Asumiendo que estás usando una imagen para el timeline
    public float lerpDuration = 6f; // Tiempo que tardará el lerp
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
        LoadDataForDisplay(Analytics.GetInstance().GetData());
        //DebugAnalytics();
    }

#if UNITY_EDITOR
    //DEBUG

    public void DebugAnalytics()
    {
        // Definir los nombres de personajes y órdenes predefinidas
        string[] characterNames = { "Character1", "Character2", "Character3", "Character4", "Character5", "Character6", "Character7", "Character8", "Character9", "Character10", "Character11", "Character12", "Character13", "Character14", "Character15", "Character16", "Character17", "Character18" };
        string[] orders = { "Jump", "Run", "Attack", "Defend", "Jump2", "R2un", "Atta2ck", "Defe2nd", "Jum3p", "R3un", "Atta3ck", "D3efend", "Ju4mp", "R4un", "Atta4ck", "De4end", "Jum17p", "R18un" };

        // Crear una lista de PlayerOrders predefinidos
        List<Analytics.PlayerOrder> simulatedOrders = new List<Analytics.PlayerOrder>();

        System.Random random = new System.Random();
        for (int i = 0; i < characterNames.Length; i++)
        {
            // Generar un timestamp aleatorio
            int minutes = random.Next(0, 60);
            int seconds = random.Next(0, 60);
            string timeStamp = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Simular si la acción es correcta o incorrecta
            bool isCorrect = random.Next(0, 2) == 0; // Genera true o false aleatoriamente

            // Crear una explicación básica para cada acción
            string explanation = isCorrect ? "Action performed correctly --  " + " CUSTOM explanation: " + timeStamp + " ----- " : "Action performed Incorrectly --  " + " CUSTOM explanation: " + timeStamp + " ----- ";

            // Crear un PlayerOrder y asignar valores
            Analytics.PlayerOrder order = new Analytics.PlayerOrder();
            order.timeStamp = timeStamp;
            order.characterName = characterNames[i];
            order.order = orders[i];
            order.isCorrect = isCorrect;
            order.explanation = explanation;

            // Asignar el PlayerOrder a la lista
            simulatedOrders[i] = order;
        }

        // Llamar a LoadDataForDisplay para visualizar los datos simulados
        LoadDataForDisplay(simulatedOrders);
    }
#endif

    public void SetSatisfactionLevel(int incomingSatisfactionLevel) 
    {
        satisfactionLevel = incomingSatisfactionLevel;
        satisfactionPanel.GetComponentInChildren<Animator>().Play("PanelDialogoDesaparece");
        timelinePanel.SetActive(true);
    }
    
    public void LoadDataForDisplay(List<Analytics.PlayerOrder> incommingOrderList) 
    {
        for (int i = 0; i < incommingOrderList.Count; i++)
        {
            if (incommingOrderList[i].isCorrect)
            {
                correctTimeStamps[i].SetActive(true);
                incorrectTimeStamps[i].SetActive(false);

                correctTimeStamps[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = incommingOrderList[i].order;
                correctTimeStamps[i].transform.Find("TimeStep").GetComponent<TextMeshProUGUI>().text = incommingOrderList[i].timeStamp;
            }
            else
            {
                correctTimeStamps[i].SetActive(false);
                incorrectTimeStamps[i].SetActive(true);

                incorrectTimeStamps[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = incommingOrderList[i].order;
                incorrectTimeStamps[i].transform.Find("TimeStep").GetComponent<TextMeshProUGUI>().text = incommingOrderList[i].timeStamp;
                incorrectTimeStamps[i].transform.Find("PanelExplicacion").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = incommingOrderList[i].explanation;
            }
        }
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

    public void GetTimeLineCloserToSelect() 
    {
        timelinePanel.GetComponent<Animator>().Play("TextPanelTimeLineSeAcerca");
    }

}
