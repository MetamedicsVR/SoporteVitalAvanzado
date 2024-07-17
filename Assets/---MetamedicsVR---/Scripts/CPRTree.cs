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
        //1.El jugador valora la escena en la que se encuentra(Observar donde est� el paciente y que todo es seguro)
        //2.Avisa al resto de los compa�eros(entra B1, B2 y B3)
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


    //3.	Indica a bot1 valore consciencia (se acerca al paciente al o�do le pregunta ��est� usted bien?�, le coge de los hombros y le zarandea)
    //4.	Bot1 dice� est� inconsciente�

    //5.	J1 le indica a B1 abra v�a a�rea (maniobra frente ment�n)
    //6.	B1 Abre v�a a�rea, acercando su oreja a la boca del paciente y visualizando su t�rax.
    //7.	B1 NO RESPIRA

    //8.	J1 a B1 le ponga g�edel y ventile con amb� con reservorio conectado al ox�geno.
    //9.	B1 lo hace.

    //10.	J1 le pide a B2 tome qel pulso carot�deo.
    //11.	B2 indica que no tiene pulso
    
    //      le indica que comience la RCP.

    //12.	J1 indica a B3 que monitorice con el desfibrilador.
    //13.	B3 monitoriza con el desfibrilador.

    //14.	J1 se acerca al monitor y visualiza el ritmo, se observa una fibrilaci�n ventricular.

    //15.	J1 indica a B2 que inicie compresiones y cuente en alto y cuando llegue a 30 compresiones pare
    
    //y B1 hace dos ventilaciones. (deber�a salir un reloj para que J1 contabilice el tiempo de RCP)

    //16.	Mientras J1 indica a B3 que cargue el desfibrilador a 150 julios.
    
    //??? 17.	J1 indica que se retire todo el personal y no toque al paciente.
  
    //18.	J1 indica a B3 que d� al bot�n de descarga.
    
    //19.	J1 indica despu�s de la descarga inmediatamente que B1 y B2 vuela a su posici�n realizando la actividad que ven�an haciendo.
    
    //20.	Mientras B2 y B1 est�n en masaje y ventilando, J1 le dice a B3 que coja una VVP en el brazo.
    //21.	B3 prepara material para coger la v�a.
    //22.	B2 y B1 contin�an 2 minutos de RCP.
    
    //23.	B2 indica que est� cansado y sugiere cambio con otro B. J1 llama a B4.
    //24.	B4 comienza RCP.
    
    //25.	J1 indica que paren el masaje para volver a evaluar.
    //26.	B1 eval�a v�a a�rea (ver arriba) y B4 eval�a circulaci�n (ver arriba).
    //27.	B3 coge VVP.
    //28.	Se realiza otro ciclo de 2 minutos manteniendo los mismos.
    //29.	SE REPITE EL CICLO 3 VECES (3 desfibrilaciones).
    
    //30.	J1 tras la 3 descarga indica a B3 que administre 1 mg de adrenalina Intravenosa.
    //31.	B3 la carga, confirma la medicaci�n, dosis y v�a de administraci�n y se la administra.
    
    //32.	J1 indica a B3 que cargue 300 mg amiodarona en 20 ml de suero fisiol�gico.
    //33.	B3 la carga, confirma la medicaci�n, dosis y v�a de administraci�n y se la administra.
    
    //34.	Tras dos minutos, J1 indica parar la RCP para B1 y B4 reeval�en (ver arriba).
    //35.	Se visualiza en el monitor un electro normal y el paciente tiene pulso.

    //    MODULO 0 BRIEFING
    //    Aparece en la sala P1, J1, B1, B2 y B3 en una sala con sillas colocadas en redondo.
    //   P1 hace una breve sinopsis del caso que se va a realizar, recuerda los objetivos del caso y las normas b�sicas de simulaci�n.
    //J1 tiene que indicar de una bater�a de afirmaciones la/las correctas en cuanto a las normas de simulaci�n cl�nica que trabajamos.
    //J1, B1, B2 y B3, se re�nen antes de pasar para indicar los roles de cada uno. J1 asigna a B su rol. 1. V�a a�rea y respiraci�n, 2. Circulaci�n- masaje y 3. Monitor desfibrilador y VVP.

    //MODULO 2 DEBRIEFING
    //Defusing: P1 recibe a J1, B1, B2 y B3 al salir de la sala. Como en una escala, un term�metro que pueda seleccionar de verde-amarillo-rojo en c�mo se siente (si se marca en rojo, se indique en el informe y nos salga un aviso al profesor, para poder abordarlo)
    //P1 recibe en el aula de debriefing.
    //Se plantear� una l�nea de tiempo en el que se indicar� en naranja lo que NO se ha hecho bien, y en verde lo que SI se ha hecho bien. Deben poder seleccionar lo que NO se ha hecho bien, y ah� dar las opciones de como SI se deber�a haber hecho.
    //�Ahora que ya has visto lo que has hecho bien y en que puedes mejorar, �Qu� incorporar�a si lo realizara en otro momento?�



}
