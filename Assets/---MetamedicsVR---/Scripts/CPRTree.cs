using Meta.WitAi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class CPRTree : MonoBehaviourInstance<CPRTree>
{
    public VoiceController voiceController;
    public PanelSeleccionManual panelSeleccion;

    public GameObject warningPanel;
    public GameObject errorPanel;

    public bool monitorAttached;
    public GameObject pantallaFibrilarVentriculationInDefibrilator;
    public GameObject pantallaSynusRythmInDefibrilator;
    public GameObject pantallaFibrilarVentriculationPlayer;
    public GameObject pantallaSynusRythmPlayer;

    public Timer timer;

    private bool canGiveOrders;
    private int nextStepIndex;

    private List<List<NPCManager.NPCAction>> stepNeededActions;

    protected override void Awake()
    {
        base.Awake();
        stepNeededActions = new List<List<NPCManager.NPCAction>>()
        {
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.CheckConsciousness
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.CheckAirWay
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.CheckPulse
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.PutGuedel
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Ventilations,
                NPCManager.NPCAction.Compressions
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Ventilations,
                NPCManager.NPCAction.Compressions,
                NPCManager.NPCAction.CheckDefibrilator
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Ventilations,
                NPCManager.NPCAction.Compressions,
                NPCManager.NPCAction.ChargeDefibrilator
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.ChargeDefibrilator,
                NPCManager.NPCAction.Rest,
                NPCManager.NPCAction.Rest,
                NPCManager.NPCAction.Rest
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.PlacePatches
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.DischargeDefibrilator
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.PlaceVVP
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Epinephrine
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Lidocaine
            },
            new List<NPCManager.NPCAction>()
            {
                NPCManager.NPCAction.Ventilations,
                NPCManager.NPCAction.Compressions
            },
        };
    }

    private void Start()
    {
        canGiveOrders = false;  
        StartCoroutine(Experience());
    }


    public void StartExperienceCorutine()
    {
        StartCoroutine(Experience());
    }

    public IEnumerator Experience()
    {
        //1.El jugador valora la escena en la que se encuentra(Observar donde está el paciente y que todo es seguro)
        //2.Avisa al resto de los compañeros(entra B1, B2 y B3)
        //if (ExperienceSettings.IsUsingVoice())
        {
            voiceController.StartRecording();
        }
        //else
        {
            //panelSeleccion.AbrirPanelNPC();
        }
        yield return new WaitForSeconds(1);
        foreach (NPCManager.NPCName npcName in Enum.GetValues(typeof(NPCManager.NPCName)))
        {
            NPCManager.GetInstance().FindNPC(npcName).gameObject.SetActive(true);
            NPCManager.GetInstance().SelectNPC(npcName);
            NPCManager.GetInstance().GiveOrder(NPCManager.NPCAction.OutNow);
            yield return new WaitForSeconds(1);
        }
        canGiveOrders = true;
        panelSeleccion.gameObject.SetActive(true);
        RefreshPanelOptions();
    }

    public void NewWords(List<string> words)
    {
        print("TEST WORD:" + words[0]);
        if (canGiveOrders)
        {
            List<NPCManager.NPCName> npcNames = NPCManager.GetInstance().CheckNames(words);
            if (npcNames.Count > 0)
            {
                NPCManager.GetInstance().SelectNPC(npcNames[npcNames.Count - 1]);
                print("TEST NAME: " + npcNames[npcNames.Count - 1]);
            }
            NPC selectedNPC = NPCManager.GetInstance().GetSelectedNPC();
            if (selectedNPC)
            {
                panelSeleccion.AbrirPanelAcciones();
                List<NPCManager.NPCAction> actionMatches = NPCManager.GetInstance().CheckActions(words);
                if (actionMatches.Count > 0)
                {
                    NPCManager.NPCAction chosenAction = actionMatches[actionMatches.Count - 1];
                    print("TEST ACTION: " + actionMatches[actionMatches.Count - 1]);
                    string reason;
                    TryGiveInstruction(chosenAction, out reason);
                }
            }
            else
            {
                //Instrucción sin NPC seleccionado
            }
        }
    }

    public bool TryGiveInstruction(NPCManager.NPCAction chosenAction, out string reason)
    {
        bool stepEnded;
        if (IsCorrectInstruction(chosenAction, out reason, out stepEnded))
        {
            NPCManager.GetInstance().GiveOrder(chosenAction);
            if (stepEnded)
            {
                nextStepIndex++;
                RefreshPanelOptions();
            }
            return true;
        }
        return false;
    }

    public bool IsCorrectInstruction(NPCManager.NPCAction action, out string reason, out bool stepEnded)
    {
        NPC selectedNPC = NPCManager.GetInstance().GetSelectedNPC();
        if (IsCloserToStep(selectedNPC, action, out stepEnded))
        {
            reason = "";
            return true;
        }
        else
        {
            reason = "No es la acción que el NPC debe realizar";
            return false;
        }
    }

    public bool IsCloserToStep(NPC npc, NPCManager.NPCAction action, out bool stepEnded)
    {
        /*
        print("Next index: " + nextStepIndex);
        print("Step needed actions:");
        {
            for (int i = 0; i < stepNeededActions[nextStepIndex].Count; i++)
            {
                print("- " + stepNeededActions[nextStepIndex][i]);
            }
        }
        */
        List<NPCManager.NPCAction> currentActions = NPCManager.GetInstance().GetCurrentNPCActions();
        /*
        print("Current Actions:");
        {
            for (int i = 0; i < currentActions.Count; i++)
            {
                print("- " + currentActions[i]);
            }
        }
        */
        List<NPCManager.NPCAction> futureCurrentActions = new List<NPCManager.NPCAction>(currentActions);
        futureCurrentActions.Remove(npc.GetCurrentAction());
        futureCurrentActions.Add(action);
        /*
        print("Future Actions:");
        {
            for (int i = 0; i < futureCurrentActions.Count; i++)
            {
                print("- " + futureCurrentActions[i]);
            }
        }
        */
        int currentDifference = ActionsDiference(nextStepIndex, currentActions);
        int futureDifference = ActionsDiference(nextStepIndex, futureCurrentActions);

        stepEnded = futureDifference == 0;
        print("Step Ended, difference = " + futureDifference);

        return futureDifference < currentDifference;
    }

    public int ActionsDiference(int stepIndex, List<NPCManager.NPCAction> actionsToCheck)
    {
        List<NPCManager.NPCAction> targetActions = new List<NPCManager.NPCAction>(stepNeededActions[stepIndex]);
        /*
        print("Target Actions:");
        {
            for (int i = 0; i < stepNeededActions[stepIndex].Count; i++)
            {
                print("- " + stepNeededActions[stepIndex][i]);
            }
        }
        */
        for (int i = actionsToCheck.Count - 1; i >= 0; i--)
        {
            if (targetActions.Contains(actionsToCheck[i]))
            {
                targetActions.Remove(actionsToCheck[i]);
                actionsToCheck.RemoveAt(i);
            }
        }
        /*
        print("TARGET ACTIONS:");
        {
            for (int i = 0; i < targetActions.Count; i++)
            {
                print("- " + targetActions[i]);
            }
        }
        print("ACTIONS TO CHECK:");
        {
            for (int i = 0; i < actionsToCheck.Count; i++)
            {
                print("- " + actionsToCheck[i]);
            }
        }
        */
        return targetActions.Count;
    }

    private void RefreshPanelOptions()
    {
        List<NPCManager.NPCAction> actions = new List<NPCManager.NPCAction>();
        for (int i = 0; i <stepNeededActions[nextStepIndex].Count; i++)
        {
            actions.Add(stepNeededActions[nextStepIndex][i]);
            print(stepNeededActions[nextStepIndex][i]);
        }
        NPCManager.NPCAction[] allActions = new NPCManager.NPCAction[] {
            NPCManager.NPCAction.CheckConsciousness,
            NPCManager.NPCAction.CheckAirWay,
            NPCManager.NPCAction.PutGuedel,
            NPCManager.NPCAction.CheckPulse,
            NPCManager.NPCAction.Compressions,
            NPCManager.NPCAction.Ventilations,
            NPCManager.NPCAction.CheckDefibrilator,
            NPCManager.NPCAction.PlacePatches,
            NPCManager.NPCAction.ChargeDefibrilator,
            NPCManager.NPCAction.DischargeDefibrilator,
            NPCManager.NPCAction.PlaceVVP,
            NPCManager.NPCAction.Epinephrine,
            NPCManager.NPCAction.Lidocaine,
        };
        for (int i = actions.Count; i < 4; i++)
        {
            List<NPCManager.NPCAction> availableActions = allActions.Except(actions).ToList();
            if (availableActions.Count > 0)
            {
                NPCManager.NPCAction randomAction = availableActions[new System.Random().Next(availableActions.Count)];
                actions.Add(randomAction);
            }
        }
        panelSeleccion.UpdateOptions(actions);
    }

    private int test = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 5;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            test++;
            Test();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Test();
        }
    }

    private void Test()
    {
        string reason;
        switch (test)
        {
            case 1:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckConsciousness);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.CheckConsciousness, true, "");
                break;
            case 2:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.CheckAirWay, true, "");
                break;
            case 3:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.CheckDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.CheckDefibrilator, true, "");
                break;
            case 4:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.CheckPulse);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.CheckPulse, true, "");
                break;
            case 5:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.PutGuedel);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.PutGuedel, true, "");
                break;
            case 6:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.Ventilations);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.Ventilations, true, "");
                break;
            case 7:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.Compressions, true, "");
                break;
            case 8:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.ChargeDefibrilator, true, "");
                break;
            case 9:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.OutNow, true, "");
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.OutNow, true, "");
                break;
            case 10:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.PlacePatches);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.PlacePatches, true, "");
                break;
            case 11:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.DischargeDefibrilator, true, "");
                break;
            case 12:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.PlaceVVP);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.PlaceVVP, true, "");
                break;
            case 13:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.Ventilations);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.Ventilations, true, "");
                break;
            case 14:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Compressions);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.Compressions, true, "");
                break;
            case 15:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.ChargeDefibrilator, true, "");
                break;
            case 16:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.OutNow, true, "");
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.OutNow, true, "");
                break;
            case 17:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.DischargeDefibrilator, true, "");
                break;
            case 18:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.Ventilations, true, "");
                break;
            case 19:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.Compressions, true, "");
                break;
            case 20:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.ChargeDefibrilator, true, "");
                break;
            case 21:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.OutNow, true, "");
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.OutNow);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.OutNow, true, "");
                break;
            case 22:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.DischargeDefibrilator, true, "");
                break;
            case 23:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.Ventilations, true, "");
                break;
            case 24:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.Compressions);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.Compressions, true, "");
                break;
            case 25:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jesús).GiveOrder(NPCManager.NPCAction.Epinephrine);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Jesús.ToString(), NPCManager.NPCAction.Epinephrine, true, "");
                break;
            case 26:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.Lidocaine);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.David.ToString(), NPCManager.NPCAction.Lidocaine, true, "");
                break;
            case 27:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckPulse);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Carla.ToString(), NPCManager.NPCAction.CheckPulse, true, "");
                pantallaFibrilarVentriculationInDefibrilator.SetActive(false);
                pantallaFibrilarVentriculationPlayer.SetActive(false);
                pantallaSynusRythmInDefibrilator.SetActive(true);
                pantallaSynusRythmPlayer.SetActive(true);
                GameManager.GetInstance().parentPanelSalir.SetActive(true);
                break;
            case 28:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubén).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                Analytics.GetInstance().InsertData(NPCManager.NPCName.Rubén.ToString(), NPCManager.NPCAction.CheckAirWay, true, "");
                break;
        }
        RefreshPanelOptions();
    }
}


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


    
