using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelSeleccionManual : MonoBehaviour
{

    public TextMeshProUGUI[] optionsTextsInGame;
    public TextMeshProUGUI textoPanelIncorrecto;

    public string textoOriginalA;
    public string textoOriginalB;
    public string textoOriginalC;
    public string textoOriginalD;

    public GameObject buttonA;
    public GameObject buttonB;
    public GameObject buttonC;
    public GameObject buttonD;

    public string [] optionsTexts = new string[4];

    public GameObject panelSeleccionNPC;

    public GameObject panelSeleccionAcciones;

    public int selectedNPC;

    public NPCManager.NPCAction selectedAction;

    public bool[] valorInternoDeOpciones;

    public List<NPCManager.NPCAction> givenActions;

    public string RecibirAccion(NPCManager.NPCAction thisAction) 
    {
        switch (thisAction)
        {
            case NPCManager.NPCAction.Rest:
                return "Ordenar que descanse";             
            case NPCManager.NPCAction.Walk:
                return "";
            case NPCManager.NPCAction.CheckConsciousness:
                return "Ordenar que compruebe consciencia";
            case NPCManager.NPCAction.CheckAirWay:
                return "Ordenar que compruebe la vía aerea";
            case NPCManager.NPCAction.PutGuedel:
                return "Ordenar que ponga la canula de güedel";
            case NPCManager.NPCAction.CheckPulse:
                return "Ordenar que compruebe el pulso";
            case NPCManager.NPCAction.Compressions:
                return "Ordenar que haga compresiones";
            case NPCManager.NPCAction.Ventilations:
                return "Ordenar que haga ventilaciones";
            case NPCManager.NPCAction.CheckDefibrilator:
                return "Ordenar que encienda desfibrilador";
            case NPCManager.NPCAction.PlacePatches:
                return "Ordenar que coloque parches";
            case NPCManager.NPCAction.ChargeDefibrilator:
                return "Ordenar que cargue defibrilador";
            case NPCManager.NPCAction.DischargeDefibrilator:
                return "Ordenar que de descarga a 150 J";
            case NPCManager.NPCAction.PlaceVVP:
                return "Ordenar que coloque vía en brazo";
            case NPCManager.NPCAction.Epinephrine:
                return "Ordenar que administre epinefrina";
            default:
                return "";
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug
        RecibirOpcionesDePaso(NPCManager.NPCAction.CheckConsciousness, NPCManager.NPCAction.CheckAirWay, NPCManager.NPCAction.PutGuedel, NPCManager.NPCAction.CheckPulse); // EJEMPLO
        bool[] debugTestValores = new bool[] { true, false, false, false }; //Ejemplo
        RefreshAvailableActions(debugTestValores);//Ejemplo
        //Debug
    }

    

    public void RecibirOpcionesDePaso(NPCManager.NPCAction QuizAAction, NPCManager.NPCAction QuizBAction, NPCManager.NPCAction QuizCAction, NPCManager.NPCAction QuizDAction)
    {
        textoOriginalA = RecibirAccion(QuizAAction);
        textoOriginalB = RecibirAccion(QuizBAction);
        textoOriginalC = RecibirAccion(QuizCAction);
        textoOriginalD = RecibirAccion(QuizDAction);
        AssignRandomTexts();
         givenActions = new List<NPCManager.NPCAction>
        {
            QuizAAction,
            QuizBAction,
            QuizCAction,
            QuizDAction
        };
    }

    void AssignRandomTexts()
    {
        // Creamos una lista con todos los textos originales.
        List<string> textos = new List<string>
        {
            textoOriginalA,
            textoOriginalB,
            textoOriginalC,
            textoOriginalD
        };

        

        // Barajamos la lista para que los textos estén en orden aleatorio.
        Shuffle(textos);

        // Asignamos los textos barajados al array.
        for (int i = 0; i < optionsTexts.Length; i++)
        {
            optionsTexts[i] = textos[i];
        }

        for (int i = 0; i < optionsTextsInGame.Length; i++)
        {
            if (i == 0)
            {
                optionsTextsInGame[i].text = "A- ";
            }
            else if (i == 1)
            {
                optionsTextsInGame[i].text = "B- ";
            }
            else if (i == 2)
            {
                optionsTextsInGame[i].text = "C- ";
            }
            else if (i == 3)
            {
                optionsTextsInGame[i].text = "D- ";
            }
            optionsTextsInGame[i].text += optionsTexts[i];
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SeleccionaA() 
    {
        if (valorInternoDeOpciones[0])
        {
            print("Correcto");
            for (int i = 0; i < givenActions.Count; i++)
            {
                print("Comparando -- " + optionsTextsInGame[0].text +" -- con  -- " + RecibirAccion(givenActions[i]));
                if (optionsTextsInGame[0].text.Contains(RecibirAccion(givenActions[i])))
                {
                    selectedAction = givenActions[i];
                    // IsCorrectInstruction(NPCManager.NPCAction selectedAction);
                }
            }
        }
        else
        {
            textoPanelIncorrecto.text = (optionsTexts[0] + " es incorrecto en este paso");
            textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
            print(optionsTexts[0] + " Es incorrecto en este paso");
            Invoke(nameof(DesaparecePanelIncorrecto),3);
        }
    }

    public void SeleccionaB()
    {
        if (valorInternoDeOpciones[1])
        {
            print("Correcto");
            for (int i = 0; i < givenActions.Count; i++)
            {
                print("Comparando -- " + optionsTextsInGame[1].text + " -- con  -- " + RecibirAccion(givenActions[i]));
                if (optionsTextsInGame[1].text.Contains(RecibirAccion(givenActions[i])))
                {
                    selectedAction = givenActions[i];
                    // IsCorrectInstruction(NPCManager.NPCAction selectedAction);
                }
            }
        }
        else
        {
            textoPanelIncorrecto.text = (optionsTexts[1] + " es incorrecto en este paso");
            textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
            print(optionsTexts[1] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionaC()
    {
        if (valorInternoDeOpciones[2])
        {
            print("Correcto");
            for (int i = 0; i < givenActions.Count; i++)
            {
                print("Comparando -- " + optionsTextsInGame[2].text + " -- con  -- " + RecibirAccion(givenActions[i]));
                if (optionsTextsInGame[2].text.Contains(RecibirAccion(givenActions[i])))
                {
                    selectedAction = givenActions[i];
                    // IsCorrectInstruction(NPCManager.NPCAction selectedAction);
                }
            }
        }
        else
        {
            textoPanelIncorrecto.text = (optionsTexts[2] + " es incorrecto en este paso");
            textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
            print(optionsTexts[2] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionaD()
    {
        if (valorInternoDeOpciones[3])
        {
            print("Correcto");
            for (int i = 0; i < givenActions.Count; i++)
            {
                print("Comparando -- " + optionsTextsInGame[3].text + " -- con  -- " + RecibirAccion(givenActions[i]));
                if (optionsTextsInGame[3].text.Contains(RecibirAccion(givenActions[i])))
                {
                    selectedAction = givenActions[i];
                    // IsCorrectInstruction(NPCManager.NPCAction selectedAction);
                }
            }
        }
        else
        {
            textoPanelIncorrecto.text = (optionsTexts[3] + " es incorrecto en este paso");
            textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
            print(optionsTexts[3] + " Es incorrecto en este paso");
        }
    }

    public void DesaparecePanelIncorrecto()
    {
        textoPanelIncorrecto.transform.parent.GetComponent<Animator>().Play("PanelDisappear");
        Invoke(nameof(ApagaPanelIncorrecto), 1f);
    }

    public void ApagaPanelIncorrecto()
    {
        textoPanelIncorrecto.transform.parent.gameObject.SetActive(false);
    }
    public void SeleccionarNPC(int seleccion) //0 Carla // 1 David //2 Ruben // 3 Jesus
    {
        print("Selecionado NPC " + seleccion);
        panelSeleccionNPC.GetComponent<Animator>().Play("PanelDisappear");
        Invoke(nameof(CerrarPanelNPCAbrirPanelAcciones),1f);
        selectedNPC = seleccion;
        NPCManager.GetInstance().SelectNPC((NPCManager.NPCName)selectedNPC);
    }

    public void CerrarPanelNPCAbrirPanelAcciones() 
    {
        panelSeleccionNPC.SetActive(false);
        panelSeleccionAcciones.SetActive(true);
    }

    public void RefreshAvailableActions(bool[] actionsValueArray) //Tienene que pasarse siempre de  4
    {
        for (int i = 0; i < valorInternoDeOpciones.Length; i++)
        {
            valorInternoDeOpciones[i] = actionsValueArray[i];
        }
    }
}
