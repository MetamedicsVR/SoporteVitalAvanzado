using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingManager : MonoBehaviour
{

    public GameObject textoBriefing;

    public bool [] briefingQuizesList;
    public bool[] valoresCorrectosBriefingQuiz;
    public GameObject [] quizzesBriefingList;

    public GameObject npcSaludador;
    public GameObject [] checkmarksYes;
    public GameObject [] checkmarksNoes;
    public GameObject  analiticsPanel;

    public bool [] quizesFinalizadosCorrectamenteAnaliticasExternas;
    public int thisPanelNumber;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowPanelBriefing",12);
        Invoke("HidePanelBriefing", 15);// BORRAR CUANDO INTERACCION OK
        Invoke("DebugInvokableQuiz01", 18);// BORRAR CUANDO INTERACCION OK
    }
    public void DebugInvokableQuiz01() 
    {
        quizzesBriefingList[0].SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            CompleteBriefingQuizTrue(0);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            CompleteBriefingQuizTrue(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            CompleteBriefingQuizTrue(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            CompleteBriefingQuizFalse(3);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            CompleteBriefingQuizFalse(4);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            CompleteBriefingQuizFalse(5);
        }
    }

    public void ShowPanelBriefing() 
    {
        textoBriefing.SetActive(true);
    }

    public void HidePanelBriefing()
    {
        npcSaludador.GetComponent<Animator>().CrossFade("Anim_IdleSentado",1);
        textoBriefing.transform.GetComponentInChildren<Animator>().Play("PanelDialogoDesaparece");
        Invoke("SetActiveFalsePanelBriefing", 1);
    }

    public void SetActiveFalsePanelBriefing() 
    {
        textoBriefing.SetActive(false);
    }

    public void CompleteBriefingQuizTrue(int quizNumber) 
    {
        briefingQuizesList[quizNumber] = true;
        quizzesBriefingList[quizNumber].GetComponentInChildren<Animator>().Play("PanelDialogoDesaparece");
        thisPanelNumber = quizNumber;
        Invoke("InvokableSetActiveFalseQuizPanel",1);
    }
    public void CompleteBriefingQuizFalse(int quizNumber)
    {
        briefingQuizesList[quizNumber] = false;
        quizzesBriefingList[quizNumber].GetComponentInChildren<Animator>().Play("PanelDialogoDesaparece");
        thisPanelNumber = quizNumber;
        Invoke("InvokableSetActiveFalseQuizPanel", 1);
    }
    public void InvokableSetActiveFalseQuizPanel() 
    {
        quizzesBriefingList[thisPanelNumber].SetActive(false);
        if (thisPanelNumber + 1  < quizzesBriefingList.Length)
        {
            quizzesBriefingList[thisPanelNumber + 1].SetActive(true);
        }
        else
        {
            Invoke("InvokeAnaliticsPanel", 3);
        }     
    }

    public void InvokeAnaliticsPanel() 
    {
        analiticsPanel.SetActive(true);
        CheckAnaliticsAndSend();
    }

    public void CheckAnaliticsAndSend() 
    {
       
        for (int i = 0; i < briefingQuizesList.Length; i++)
        {
            if (valoresCorrectosBriefingQuiz[i] == briefingQuizesList[i])
            {
                checkmarksYes[i].SetActive(true);
                checkmarksNoes[i].SetActive(false);
                quizesFinalizadosCorrectamenteAnaliticasExternas[i] = true;
            }
            else
            {
                checkmarksYes[i].SetActive(false);
                checkmarksNoes[i].SetActive(true);
                quizesFinalizadosCorrectamenteAnaliticasExternas[i] = false;
            }
        }
    }
}
