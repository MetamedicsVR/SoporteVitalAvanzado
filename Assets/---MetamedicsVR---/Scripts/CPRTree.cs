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
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Calvo).GiveOrder(NPC.NPCAction.Rest);
                    break;
                case 1:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.CheckConsciousness);
                    break;
                case 2:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.CheckAirWay); //Open if closed
                    break;
                case 3:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.CheckPulse);
                    break;
                case 4:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.PutGuedel);
                    break;
                case 5:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Ventilations);
                    break;
                case 6:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Compressions);
                    //NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.CheckDefibrilator);
                    break;
                case 7:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.ChargeDefibrilator);
                    break;
                case 8:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Rest);
                    break;
                case 9:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.DischargeDefibrilator);
                    break;
                case 10:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Ventilations);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Compressions);
                    break;
                case 11:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Calvo).GiveOrder(NPC.NPCAction.Compressions);
                    break;
                case 12:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Rest);
                    break;
                case 13:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.CheckAirWay);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Calvo).GiveOrder(NPC.NPCAction.CheckPulse);
                    break;
                case 14:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Rest);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Rest);
                    break;
                case 15:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.Ventilations);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Gafas).GiveOrder(NPC.NPCAction.Compressions);
                    break;
                case 16:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.Epinephrine);
                    break;
                case 17:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Rubio).GiveOrder(NPC.NPCAction.Lidocaine);
                    break;
                case 18:
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Chica).GiveOrder(NPC.NPCAction.CheckAirWay);
                    NPCManager.GetInstance().FindNPC(NPCManager.NPCName.Calvo).GiveOrder(NPC.NPCAction.CheckPulse);
                    break;
            }
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


                                                       MODULO 0 BRIEFING
Aparece en la sala P1, J1, B1, B2 y B3 en una sala con sillas colocadas en redondo.
P1 hace una breve sinopsis del caso que se va a realizar, recuerda los objetivos del caso y las normas básicas de simulación.
J1 tiene que indicar de una batería de afirmaciones la/las correctas en cuanto a las normas de simulación clínica que trabajamos.

-	Si cometes errores durante la simulación, te penalizará en tu nota de la asignatura. (falso)
-	El error es nuestro amigo.Si cometes errores durante la simulación, te servirán para reflexionar durante el debriefing y no cometerlos en la vida real. (verdadero)
-	Tus actos durante el escenario no serán juzgados. Aprovecha para aprender al máximo. (verdadero)
-	Es muy importante leer la documentación previa al caso (guías, protocolos, documentación aportada en el curso, etc.). (verdadero)
-	Puedes venir a hacer la simulación sin haber leído nada antes.Es un juego y el objetivo es pasárselo bien(falso).
-	Nos hemos esforzado en hacer el escenario lo más real posible.No obstante, la fidelidad al 100% es imposible, por lo que te pedimos que trabajes en la simulación como si fuera la realidad con el fin aprovechar al máximo la experiencia. (verdadero).


J1, B1, B2 y B3, se reúnen antes de pasar para indicar los roles de cada uno.J1 asigna a B su rol. 1. Vía aérea y respiración, 2. Circulación- masaje y 3. Monitor desfibrilador y VVP.

MODULO 2 DEBRIEFING
Defusing: P1 recibe a J1, B1, B2 y B3 al salir de la sala. Como en una escala, un termómetro que pueda seleccionar de verde-amarillo-rojo en cómo se siente (si se marca en rojo, se indique en el informe y nos salga un aviso al profesor, para poder abordarlo)
P1 recibe en el aula de debriefing.
Se planteará una línea de tiempo en el que se indicará en naranja lo que NO se ha hecho bien, y en verde lo que SI se ha hecho bien.Deben poder seleccionar lo que NO se ha hecho bien, y ahí dar las opciones de como SI se debería haber hecho.
“Ahora que ya has visto lo que has hecho bien y en que puedes mejorar, ¿Qué incorporaría si lo realizara en otro momento?”

    */


}
