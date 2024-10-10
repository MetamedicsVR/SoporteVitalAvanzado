using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelSeleccionManual : MonoBehaviour
{
    public TextMeshProUGUI[] optionsTextsInGame;
    public TextMeshProUGUI textoPanelIncorrecto;

    private string [] optionsTexts = new string[4];

    public GameObject panelSeleccionNPC;

    public GameObject panelSeleccionAcciones;

    private List<NPCManager.NPCAction> givenActions;

    private Coroutine showErrorPanelCoroutine;

    public void AbrirPanelNPC()
    {
        panelSeleccionNPC.SetActive(true);
        panelSeleccionAcciones.SetActive(false);
    }

    public void SeleccionarNPC(int seleccion) //0 Carla // 1 David //2 Ruben // 3 Jesus
    {
        print("Selecionado NPC " + seleccion);
        panelSeleccionNPC.GetComponent<Animator>().Play("PanelDisappear");
        NPCManager.GetInstance().SelectNPC((NPCManager.NPCName)seleccion);
        Invoke(nameof(AbrirPanelAcciones), 1);
    }

    public void SeleccionaOpcion(int n)
    {
        string reason;
        if (CPRTree.GetInstance().TryGiveInstruction(givenActions[n], out reason))
        {
            NPCManager.GetInstance().GiveOrder(givenActions[n]);
            AbrirPanelNPC();
        }
        else
        {
            if (showErrorPanelCoroutine != null)
            {
                StopCoroutine(showErrorPanelCoroutine);
            }
            showErrorPanelCoroutine = StartCoroutine(ShowingErrorPanel(reason));
        }
    }

    private IEnumerator ShowingErrorPanel(string reason)
    {
        textoPanelIncorrecto.text = (optionsTexts[0] + " es incorrecto en este paso: " + reason);
        textoPanelIncorrecto.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        textoPanelIncorrecto.transform.parent.parent.GetComponent<Animator>().Play("PanelDisappear");
        yield return new WaitForSeconds(1);
        textoPanelIncorrecto.transform.parent.gameObject.SetActive(false);
    }

    public void AbrirPanelAcciones()
    {
        panelSeleccionNPC.SetActive(false);
        panelSeleccionAcciones.SetActive(true);
    }

    public void UpdateOptions(List<NPCManager.NPCAction> actions)
    {
        givenActions = actions;
        Shuffle(givenActions);
        List<string> textos = new List<string>
        {
            ActionText(givenActions[0]),
            ActionText(givenActions[1]),
            ActionText(givenActions[2]),
            ActionText(givenActions[3])
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

    public string ActionText(NPCManager.NPCAction thisAction) 
    {
        switch (thisAction)
        {
            case NPCManager.NPCAction.OutNow:
                return "Ordenar que se alejen";
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
}
