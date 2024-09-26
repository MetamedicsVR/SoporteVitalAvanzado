using Meta.WitAi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class CPRTree : MonoBehaviour
{
    public VoiceService[] voiceServices;

    public GameObject warningPanel;
    public GameObject errorPanel;

    public bool monitorAttached;

    private bool canGiveOrders;
    private int nextStepIndex;

    private List<List<NPCManager.NPCAction>> stepNeededActions;

    private void Awake()
    {
        stepNeededActions = new List<List<NPCManager.NPCAction>>()
        {
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.CheckConsciousness
            }
        };
    }

    private void Start()
    {
        foreach (VoiceService service in voiceServices)
        {
            if (service)
            {
                service.VoiceEvents.OnFullTranscription.AddListener(OnTranscriptionReceived);
            }
        }
        canGiveOrders = false;
        StartCoroutine(Experience());
    }

    private void OnTranscriptionReceived(string transcription)
    {
        if (canGiveOrders)
        {
            string cleanText = Regex.Replace(transcription.Normalize(NormalizationForm.FormD), @"[^a-zA-Z\s]", "").ToLower();
            string[] words = cleanText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<NPCManager.NPCName> npcNames = NPCManager.GetInstance().CheckNPCNames(words);
            if (npcNames.Count > 0)
            {
                NPCManager.GetInstance().SelectNPC(npcNames[npcNames.Count - 1]);
            }
            NPC selectedNPC = NPCManager.GetInstance().GetSelectedNPC();
            if (selectedNPC)
            {
                List<string> actionMatches = NPCManager.GetInstance().ActionMatches(words);
                if (actionMatches.Count > 0)
                {
                    NPCManager.NPCAction choseAction = (NPCManager.NPCAction)Enum.Parse(typeof(NPCManager.NPCAction), actionMatches[actionMatches.Count - 1]);
                    if (selectedNPC.CanPerformAction(choseAction))
                    {
                        if (IsNextStep(selectedNPC, choseAction))
                        {
                            NPCManager.GetInstance().GiveOrder(choseAction);
                        }
                        else
                        {
                            //Acción errónea
                        }
                    }
                    else
                    {
                        //No puede realizar la acción
                    }
                }
            }
            else
            {
                //Instrucción sin NPC seleccionado
            }
        }
    }

    public IEnumerator Experience()
    {
        //1.El jugador valora la escena en la que se encuentra(Observar donde está el paciente y que todo es seguro)
        //2.Avisa al resto de los compañeros(entra B1, B2 y B3)
        yield return null;
        canGiveOrders = true;
    }

    public bool IsNextStep(NPC npc, NPCManager.NPCAction action)
    {
        List<NPCManager.NPCAction> currentActions = NPCManager.GetInstance().GetCurrentNPCActions();
        currentActions.RemoveAll(action => action == NPCManager.NPCAction.Rest);
        List<NPCManager.NPCAction> futureCurremtActions = new List<NPCManager.NPCAction>(currentActions);
        currentActions.Remove(npc.GetCurrentAction());
        futureCurremtActions.Add(action);
        return ActionsDiference(nextStepIndex, currentActions) < ActionsDiference(nextStepIndex, futureCurremtActions);
    }

    public int ActionsDiference(int stepIndex, List<NPCManager.NPCAction> actionsToCheck)
    {
        List<NPCManager.NPCAction> targetActions = new List<NPCManager.NPCAction>(stepNeededActions[stepIndex]);
        for (int i = actionsToCheck.Count - 1; i >= 0; i--)
        {
            if (targetActions.Contains(actionsToCheck[i]))
            {
                targetActions.Remove(actionsToCheck[i]);
                actionsToCheck.RemoveAt(i);
            }
        }
        return targetActions.Count + actionsToCheck.Count;
    }

#if UNITY_EDITOR
    private int test = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            test++;
            print("Test Next");
            switch (test)
            {
                case 0:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.Rest);
                    break;
                case 1:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckConsciousness);
                    break;
                case 2:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.CheckAirWay); //Open if closed
                    break;
                case 3:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.CheckPulse);
                    break;
                case 4:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.PutGuedel);
                    break;
                case 5:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                    break;
                case 6:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                    //NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.CheckDefibrilator);
                    break;
                case 7:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                    break;
                case 8:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Rest);
                    break;
                case 9:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                    break;
                case 10:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                    break;
                case 11:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.Compressions);
                    break;
                case 12:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Rest);
                    break;
                case 13:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.CheckPulse);
                    break;
                case 14:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Rest);
                    break;
                case 15:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                    break;
                case 16:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.Epinephrine);
                    break;
                case 17:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.Lidocaine);
                    break;
                case 18:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.CheckPulse);
                    break;
            }
        }
    }
#endif

    /*
1.	El jugador valora la escena en la que se encuentra(Observar donde está el paciente y que todo es seguro). Se coloca los guantes.Error inocuo
2.	Avisa al resto de los compañeros (entra B1, B2 y B3). Error crítico, parar.
3.	Indica a bot1 valore consciencia(se acerca al paciente al oído le pregunta ”¿está usted bien?”, le coge de los hombros y le zarandea). Error inocuo.
4.	Bot1 dice” está inconsciente”
5.	J1 le indica a B1 abra vía aérea (maniobra frente mentón). Error crítico, parar.
    - Aquí realmente no se está indicando que se abra la vía aérea, sino que se compruebe si respira, y el NPC la tiene que abrir antes porque está cerrada
6.	B1 Abre vía aérea, acercando su oreja a la boca del paciente y visualizando su tórax.
7.	B1 NO RESPIRA
8.	J1 a B1 que le ponga güedel y ventile con ambú con reservorio conectado al oxígeno.Error inocuo.
    - Aquí realmente se está indicando que ponga guedel, no que ventile
9.	B1 lo hace.
10.	J1 le pide a B2 tome el pulso carotídeo.Error crítico, parar.
11.	B2 indica que no tiene pulso
12.	J1 le indica que comience la RCP: que inicie compresiones y cuente en alto y cuando llegue a 30 compresiones pare y B1 hace dos ventilaciones. (debería salir un reloj para que J1 contabilice el tiempo de RCP). Error crítico, parar.
13.	J1 indica a B3 que monitorice con el desfibrilador.Error crítico, parar.
14.	B3 monitoriza con el desfibrilador.
15.	J1 se acerca al monitor y visualiza el ritmo, se observa una fibrilación ventricular.
16.	Mientras J1 indica a B3 que cargue el desfibrilador a 150 julios.Error crítico, parar.
17.	J1 indica que se retire todo el personal y no toque al paciente.Error inocuo.
18.	J1 indica a B3 que dé al botón de descarga. Error crítico, parar.
19.	J1 indica después de la descarga inmediatamente que B1 y B2 vuela a su posición realizando la actividad que venían haciendo.Error crítico, parar.
20.	Mientras B2 y B1 están en masaje y ventilando, J1 le dice a B3 que coja una VVP en el brazo. Error crítico, parar.
21.	B3 prepara material para coger la vía.
22.	B2 y B1 continúan 2 minutos de RCP.
23.	B2 indica que está cansado y sugiere cambio con otro B.J1 llama a B4. Error inocuo.
24.	B4 comienza RCP.
25.	J1 indica que paren el masaje para volver a evaluar a los 2 minutos.Error crítico, parar.
26.	B1 evalúa vía aérea (ver arriba) y B4 evalúa circulación (ver arriba).
27.	B3 coge VVP.
28.	B1 indica que no hay respiración y B4 que no hay pulso. 
29.	J1 indica que se realice otro ciclo de 2 minutos manteniendo los mismos bots.Error crítico, parar.
30.	SE REPITE EL CICLO 3 VECES (3 desfibrilaciones).
31.	J1 tras la 3 descarga indica a B3 que administre 1 mg de adrenalina Intravenosa. .Error crítico, parar.
32.	B3 la carga, confirma la medicación, dosis y vía de administración y se la administra.
33.	J1 indica a B3 que cargue 300 mg amiodarona en 20 ml de suero fisiológico. . Error crítico, parar.
34.	B3 la carga, confirma la medicación, dosis y vía de administración y se la administra.
35.	Tras dos minutos, J1 indica parar la RCP para B1 y B4 reevalúen (ver arriba). . Error crítico, parar.
36.	Se visualiza en el monitor un electro normal y el paciente tiene pulso.

    */


}
