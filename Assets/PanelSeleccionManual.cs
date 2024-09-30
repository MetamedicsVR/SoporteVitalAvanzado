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
            case NPCManager.NPCAction.Lidocaine:
                return "Ordenar que administre amiodarona";
            default:
                return "";
        }

    }

    // Start is called before the first frame update


    

    public void RecibirOpcionesDePaso(List<NPCManager.NPCAction> actions)
    {
        givenActions = actions;
        Shuffle(givenActions);
        textoOriginalA = RecibirAccion(givenActions[0]);
        textoOriginalB = RecibirAccion(givenActions[1]);
        textoOriginalC = RecibirAccion(givenActions[2]);
        textoOriginalD = RecibirAccion(givenActions[3]);
        List<string> textos = new List<string>
        {
            textoOriginalA,
            textoOriginalB,
            textoOriginalC,
            textoOriginalD
        };
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

    public void SeleccionaOpcion(int n) 
    {
        string reason;
        if (CPRTree.GetInstance().TryGiveInstruction(givenActions[n], out reason))
        {
            NPCManager.GetInstance().GiveOrder(givenActions[n]);
        }
        else
        {
            textoPanelIncorrecto.text = (optionsTexts[0] + " es incorrecto en este paso: " + reason);
            textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
            Invoke(nameof(DesaparecePanelIncorrecto), 3);
        }
    }

    public void DesaparecePanelIncorrecto()
    {
        textoPanelIncorrecto.transform.parent.parent.GetComponent<Animator>().Play("PanelDisappear");
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

    public void CerrarPanelAccionesAbrirPanelNPC()
    {
        panelSeleccionNPC.SetActive(true);
        panelSeleccionAcciones.SetActive(false);
    }
}
