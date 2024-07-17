using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CPRTree : MonoBehaviour
{
    public GameObject warningPanel;
    public GameObject errorPanel;

    public bool monitorAttached;

    private bool canGiveOrders;
    private int nextStepIndex;

    private List<NPC.NPCAction> currentActions = new List<NPC.NPCAction>();
    private List<List<NPC.NPCAction>> stepNeededActions;

    private void Awake()
    {
        stepNeededActions = new List<List<NPC.NPCAction>>()
        {
            new List<NPC.NPCAction>() {
                NPC.NPCAction.CheckConsciousness
            }
        };
    }

    private void Start()
    {
        canGiveOrders = false;
        StartCoroutine(Experience());
    }

    public IEnumerator Experience()
    {
        //1.El jugador valora la escena en la que se encuentra(Observar donde está el paciente y que todo es seguro)
        //2.Avisa al resto de los compañeros(entra B1, B2 y B3)
        yield return null;
        canGiveOrders = true;
    }

    public void SelectNPC(NPC npc)
    {

    }

    public void GiveOrder(NPC npc, NPC.NPCAction action)
    {
        if (canGiveOrders && npc.CanPerformAction(action))
        {
            bool isCorrect = IsNextStep(npc, action);
            //Analytics.GetInstance().InsertData(npc.characterName, action, isCorrect);
            if (isCorrect)
            {
                //Proceder
            }
            else
            {
                //Cartel de error
            }
        }
    }

    public bool IsNextStep(NPC npc, NPC.NPCAction action)
    {
        if (npc.CanPerformAction(action))
        {
            return ActionsDiference(nextStepIndex, new List<NPC.NPCAction>(currentActions)) < ActionsDiference(nextStepIndex, new List<NPC.NPCAction>(currentActions));
        }
        return false;
    }

    public int ActionsDiference(int stepIndex, List<NPC.NPCAction> actionsToCheck)
    {
        List<NPC.NPCAction> targetActions = new List<NPC.NPCAction>(stepNeededActions[stepIndex]);
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


    //3.	Indica a bot1 valore consciencia (se acerca al paciente al oído le pregunta ”¿está usted bien?”, le coge de los hombros y le zarandea)
    //4.	Bot1 dice” está inconsciente”

    //5.	J1 le indica a B1 abra vía aérea (maniobra frente mentón)
    //6.	B1 Abre vía aérea, acercando su oreja a la boca del paciente y visualizando su tórax.
    //7.	B1 NO RESPIRA

    //8.	J1 a B1 le ponga güedel y ventile con ambú con reservorio conectado al oxígeno.
    //9.	B1 lo hace.

    //10.	J1 le pide a B2 tome qel pulso carotídeo.
    //11.	B2 indica que no tiene pulso
    
    //      le indica que comience la RCP.

    //12.	J1 indica a B3 que monitorice con el desfibrilador.
    //13.	B3 monitoriza con el desfibrilador.

    //14.	J1 se acerca al monitor y visualiza el ritmo, se observa una fibrilación ventricular.

    //15.	J1 indica a B2 que inicie compresiones y cuente en alto y cuando llegue a 30 compresiones pare
    
    //y B1 hace dos ventilaciones. (debería salir un reloj para que J1 contabilice el tiempo de RCP)

    //16.	Mientras J1 indica a B3 que cargue el desfibrilador a 150 julios.
    
    //??? 17.	J1 indica que se retire todo el personal y no toque al paciente.
  
    //18.	J1 indica a B3 que dé al botón de descarga.
    
    //19.	J1 indica después de la descarga inmediatamente que B1 y B2 vuela a su posición realizando la actividad que venían haciendo.
    
    //20.	Mientras B2 y B1 están en masaje y ventilando, J1 le dice a B3 que coja una VVP en el brazo.
    //21.	B3 prepara material para coger la vía.
    //22.	B2 y B1 continúan 2 minutos de RCP.
    
    //23.	B2 indica que está cansado y sugiere cambio con otro B. J1 llama a B4.
    //24.	B4 comienza RCP.
    
    //25.	J1 indica que paren el masaje para volver a evaluar.
    //26.	B1 evalúa vía aérea (ver arriba) y B4 evalúa circulación (ver arriba).
    //27.	B3 coge VVP.
    //28.	Se realiza otro ciclo de 2 minutos manteniendo los mismos.
    //29.	SE REPITE EL CICLO 3 VECES (3 desfibrilaciones).
    
    //30.	J1 tras la 3 descarga indica a B3 que administre 1 mg de adrenalina Intravenosa.
    //31.	B3 la carga, confirma la medicación, dosis y vía de administración y se la administra.
    
    //32.	J1 indica a B3 que cargue 300 mg amiodarona en 20 ml de suero fisiológico.
    //33.	B3 la carga, confirma la medicación, dosis y vía de administración y se la administra.
    
    //34.	Tras dos minutos, J1 indica parar la RCP para B1 y B4 reevalúen (ver arriba).
    //35.	Se visualiza en el monitor un electro normal y el paciente tiene pulso.

    //    MODULO 0 BRIEFING
    //    Aparece en la sala P1, J1, B1, B2 y B3 en una sala con sillas colocadas en redondo.
    //   P1 hace una breve sinopsis del caso que se va a realizar, recuerda los objetivos del caso y las normas básicas de simulación.
    //J1 tiene que indicar de una batería de afirmaciones la/las correctas en cuanto a las normas de simulación clínica que trabajamos.
    //J1, B1, B2 y B3, se reúnen antes de pasar para indicar los roles de cada uno. J1 asigna a B su rol. 1. Vía aérea y respiración, 2. Circulación- masaje y 3. Monitor desfibrilador y VVP.

    //MODULO 2 DEBRIEFING
    //Defusing: P1 recibe a J1, B1, B2 y B3 al salir de la sala. Como en una escala, un termómetro que pueda seleccionar de verde-amarillo-rojo en cómo se siente (si se marca en rojo, se indique en el informe y nos salga un aviso al profesor, para poder abordarlo)
    //P1 recibe en el aula de debriefing.
    //Se planteará una línea de tiempo en el que se indicará en naranja lo que NO se ha hecho bien, y en verde lo que SI se ha hecho bien. Deben poder seleccionar lo que NO se ha hecho bien, y ahí dar las opciones de como SI se debería haber hecho.
    //“Ahora que ya has visto lo que has hecho bien y en que puedes mejorar, ¿Qué incorporaría si lo realizara en otro momento?”



}
