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
    public VoiceService[] voiceServices;

    public PanelSeleccionManual panelSeleccion;
    public GameObject warningPanel;
    public GameObject errorPanel;

    public bool monitorAttached;

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
        foreach (VoiceService service in voiceServices)
        {
            if (service)
            {
                service.VoiceEvents.OnFullTranscription.AddListener(OnTranscriptionReceived);
            }
        }
        canGiveOrders = false;  
        //StartCoroutine(Experience());
    }


    public void StartExperienceCorutine()
    {
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
                    NPCManager.NPCAction chosenAction = (NPCManager.NPCAction)Enum.Parse(typeof(NPCManager.NPCAction), actionMatches[actionMatches.Count - 1]);
                    string reason;
                    TryGiveInstruction(chosenAction, out reason);
                }
            }
            else
            {
                //Instrucci�n sin NPC seleccionado
            }
        }
    }

    public bool TryGiveInstruction(NPCManager.NPCAction chosenAction, out string reason)
    {
        print(NPCManager.GetInstance().GetSelectedNPC().characterName + " va a " + chosenAction);
        bool stepEnded;
        if (IsCorrectInstruction(chosenAction, out reason, out stepEnded))
        {
            NPCManager.GetInstance().GiveOrder(chosenAction);
            if (stepEnded)
            {
                nextStepIndex++;
                /*
                print("NEXT INDEX: " + nextStepIndex);
                for (int i = 0; i < stepNeededActions[nextStepIndex].Count; i++)
                {
                    print("- " + stepNeededActions[nextStepIndex][i]);
                }
                */
            }
            RefreshPanelOptions();
            return true;
        }
        return false;
    }

    public bool IsCorrectInstruction(NPCManager.NPCAction action, out string reason, out bool stepEnded)
    {
        NPC selectedNPC = NPCManager.GetInstance().GetSelectedNPC();
        if (selectedNPC.CanPerformAction(action))
        {
            if (IsCloserToStep(selectedNPC, action, out stepEnded))
            {
                reason = "";
                return true;
            }
            else
            {
                reason = "No es la acci�n que el NPC debe realizar";
                return false;
            }
        }
        else
        {
            reason = "El NPC no puede realizar la acci�n";
            stepEnded = false;
            return false;
        }
    }

    public IEnumerator Experience()
    {
        //1.El jugador valora la escena en la que se encuentra(Observar donde est� el paciente y que todo es seguro)
        //2.Avisa al resto de los compa�eros(entra B1, B2 y B3)
        yield return new WaitForSeconds(1);
        foreach (NPCManager.NPCName npcName in Enum.GetValues(typeof(NPCManager.NPCName)))
        {
            NPCManager.GetInstance().FindNPC(npcName).gameObject.SetActive(true);
            NPCManager.GetInstance().SelectNPC(npcName);
            NPCManager.GetInstance().GiveOrder(NPCManager.NPCAction.Rest);
            yield return new WaitForSeconds(1);
        }
        yield return null;
        canGiveOrders = true;
        panelSeleccion.gameObject.SetActive(true);
        RefreshPanelOptions();
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
        
        print("Future Actions:");
        {
            for (int i = 0; i < futureCurrentActions.Count; i++)
            {
                print("- " + futureCurrentActions[i]);
            }
        }
        
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
        }
        NPCManager.NPCAction[] allActions = (NPCManager.NPCAction[])Enum.GetValues(typeof(NPCManager.NPCAction));
        for (int i = actions.Count; i < 4; i++)
        {
            List<NPCManager.NPCAction> availableActions = allActions.Except(actions).ToList();
            if (availableActions.Count > 0)
            {
                NPCManager.NPCAction randomAction = availableActions[new System.Random().Next(availableActions.Count)];
                actions.Add(randomAction);
            }
        }
        panelSeleccion.RecibirOpcionesDePaso(actions);
    }

#if UNITY_EDITOR
    private int test = -1;

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
            case 0:
                StartExperienceCorutine();
                break;
            case 1:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckConsciousness);
                break;
            case 2:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                break;
            case 3:
                 NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.CheckDefibrilator);
                break;
            case 4:
                 NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.CheckPulse);
                break;
            case 5:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.PutGuedel);
          
                break;
            case 6:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.CheckPulse); 
                break;
            case 7:
               NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Ventilations);
                break;
            case 8:
               NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.Compressions);
                break;
            case 9:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
         
                break;
            case 10:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.Rest);
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Rest);
               
                break;
            case 11:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.PlacePatches);
                break;
            case 12:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                break;
            case 13:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.PlaceVVP);
                break;
            case 14:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Ventilations);
                break;
            case 15:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Compressions);
                break;
            case 16:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                break;
            case 17:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.AllOut);
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Rest);
                break;
            case 18:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                break;
            case 19:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.Ventilations);
                break;
            case 20:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.Compressions);
                break;
            case 21:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Epinephrine);
                break;
            case 22:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.ChargeDefibrilator);
                break;
            case 23:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.AllOut);
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Rest);
                break;
            case 24:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.David).GiveOrder(NPCManager.NPCAction.DischargeDefibrilator);
                break;
            case 25:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.Lidocaine);
                break;
            case 26:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.Compressions);
                break;
            case 27:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Jes�s).GiveOrder(NPCManager.NPCAction.Ventilations);
                break;
            case 28:
                 NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Carla).GiveOrder(NPCManager.NPCAction.CheckPulse);
                break;
            case 29:
                NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rub�n).GiveOrder(NPCManager.NPCAction.CheckAirWay);
                break;

        }

    }
}
#endif

    /*
1.	El jugador valora la escena en la que se encuentra(Observar donde est� el paciente y que todo es seguro). Se coloca los guantes.Error inocuo
2.	Avisa al resto de los compa�eros (entra B1, B2 y B3). Error cr�tico, parar.
3.	Indica a bot1 valore consciencia(se acerca al paciente al o�do le pregunta ��est� usted bien?�, le coge de los hombros y le zarandea). Error inocuo.
4.	Bot1 dice� est� inconsciente�
5.	J1 le indica a B1 abra v�a a�rea (maniobra frente ment�n). Error cr�tico, parar.
    - Aqu� realmente no se est� indicando que se abra la v�a a�rea, sino que se compruebe si respira, y el NPC la tiene que abrir antes porque est� cerrada
6.	B1 Abre v�a a�rea, acercando su oreja a la boca del paciente y visualizando su t�rax.
7.	B1 NO RESPIRA
8.	J1 a B1 que le ponga g�edel y ventile con amb� con reservorio conectado al ox�geno.Error inocuo.
    - Aqu� realmente se est� indicando que ponga guedel, no que ventile
9.	B1 lo hace.
10.	J1 le pide a B2 tome el pulso carot�deo.Error cr�tico, parar.
11.	B2 indica que no tiene pulso
12.	J1 le indica que comience la RCP: que inicie compresiones y cuente en alto y cuando llegue a 30 compresiones pare y B1 hace dos ventilaciones. (deber�a salir un reloj para que J1 contabilice el tiempo de RCP). Error cr�tico, parar.
13.	J1 indica a B3 que monitorice con el desfibrilador.Error cr�tico, parar.
14.	B3 monitoriza con el desfibrilador.
15.	J1 se acerca al monitor y visualiza el ritmo, se observa una fibrilaci�n ventricular.
16.	Mientras J1 indica a B3 que cargue el desfibrilador a 150 julios.Error cr�tico, parar.
17.	J1 indica que se retire todo el personal y no toque al paciente.Error inocuo.
18.	J1 indica a B3 que d� al bot�n de descarga. Error cr�tico, parar.
19.	J1 indica despu�s de la descarga inmediatamente que B1 y B2 vuela a su posici�n realizando la actividad que ven�an haciendo.Error cr�tico, parar.
20.	Mientras B2 y B1 est�n en masaje y ventilando, J1 le dice a B3 que coja una VVP en el brazo. Error cr�tico, parar.
21.	B3 prepara material para coger la v�a.
22.	B2 y B1 contin�an 2 minutos de RCP.
23.	B2 indica que est� cansado y sugiere cambio con otro B.J1 llama a B4. Error inocuo.
24.	B4 comienza RCP.
25.	J1 indica que paren el masaje para volver a evaluar a los 2 minutos.Error cr�tico, parar.
26.	B1 eval�a v�a a�rea (ver arriba) y B4 eval�a circulaci�n (ver arriba).
27.	B3 coge VVP.
28.	B1 indica que no hay respiraci�n y B4 que no hay pulso. 
29.	J1 indica que se realice otro ciclo de 2 minutos manteniendo los mismos bots.Error cr�tico, parar.
30.	SE REPITE EL CICLO 3 VECES (3 desfibrilaciones).
31.	J1 tras la 3 descarga indica a B3 que administre 1 mg de adrenalina Intravenosa. .Error cr�tico, parar.
32.	B3 la carga, confirma la medicaci�n, dosis y v�a de administraci�n y se la administra.
33.	J1 indica a B3 que cargue 300 mg amiodarona en 20 ml de suero fisiol�gico. . Error cr�tico, parar.
34.	B3 la carga, confirma la medicaci�n, dosis y v�a de administraci�n y se la administra.
35.	Tras dos minutos, J1 indica parar la RCP para B1 y B4 reeval�en (ver arriba). . Error cr�tico, parar.
36.	Se visualiza en el monitor un electro normal y el paciente tiene pulso.

    */


    
