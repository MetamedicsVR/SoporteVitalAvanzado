using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public NPCManager.NPCName characterName;
    public AudioSource audioSource;
    public NPCSpot startingSpot;

    [Header("Tools")]
    public GameObject ambu;
    public GameObject guedel;
    public GameObject patches;
    public GameObject vvp;
    public GameObject syringe;
    public GameObject medication;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private NPCSpot currentSpot;
    private NPCManager.NPCAction currentAction;
    private NPCManager.NPCAction nextAction;
    private Coroutine actionCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetCurrentSpot(startingSpot);
    }

    public void GiveOrder(NPCManager.NPCAction action)
    {
        if (actionCoroutine != null)
        {
            StopCoroutine(actionCoroutine);
        }
        actionCoroutine = StartCoroutine(FollowOrder(action));
    }

    private IEnumerator FollowOrder(NPCManager.NPCAction action)
    {
        switch (currentAction)
        {
            case NPCManager.NPCAction.Rest:
                break;
            case NPCManager.NPCAction.Walk:
                break;
            case NPCManager.NPCAction.CheckConsciousness:
                break;
            case NPCManager.NPCAction.CheckAirWay:
                break;
            case NPCManager.NPCAction.PutGuedel:
                break;
            case NPCManager.NPCAction.CheckPulse:
                break;
            case NPCManager.NPCAction.Compressions:
                Patient.GetInstance().animator.Play("");
                break;
            case NPCManager.NPCAction.Ventilations:
                break;
            case NPCManager.NPCAction.CheckDefibrilator:
                break;
            case NPCManager.NPCAction.PlacePatches:
                break;
            case NPCManager.NPCAction.ChargeDefibrilator:
                break;
            case NPCManager.NPCAction.AllOut:
                break;
            case NPCManager.NPCAction.DischargeDefibrilator:
                break;
            case NPCManager.NPCAction.PlaceVVP:
                break;
            case NPCManager.NPCAction.Epinephrine:
                break;
            case NPCManager.NPCAction.Epinephrine2:
                break;
            case NPCManager.NPCAction.Lidocaine:
                break;
            default:
                break;
        }
        audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(action));
        audioSource.Play();
        NPCSpot.SpotType correctSpotType = NPCManager.GetInstance().GetCorrectSpotType(action);
        if (!currentSpot || currentSpot.type != correctSpotType)
        {
            NPCSpot targetSpot = NPCSpotManager.GetInstance().GetNearestFreeSpot(correctSpotType, transform.position);
            if (targetSpot.npcInSpot)
            {
                targetSpot.npcInSpot.GiveOrder(NPCManager.NPCAction.Rest);
            }
            SetCurrentSpot(targetSpot);
            currentAction = NPCManager.NPCAction.Walk;
            animator.Play("Anim_Andar");

            navMeshAgent.SetDestination(currentSpot.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(currentSpot.transform.position, transform.position) < 0.5f);

            float lerpDuration = 0.5f;
            Vector3 startPosition = transform.position;
            Quaternion startingRotation = transform.rotation;
            float elapsedTime = 0f;
            animator.CrossFade("Anim_idle", lerpDuration);
            while (elapsedTime < lerpDuration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, currentSpot.transform.position, Mathf.Clamp01(elapsedTime / lerpDuration));
                transform.rotation = Quaternion.Lerp(startingRotation, currentSpot.transform.rotation, Mathf.Clamp01(elapsedTime / lerpDuration));
                yield return null;
            }
            transform.position = currentSpot.transform.position;
            transform.rotation = currentSpot.transform.rotation;
        }
        currentAction = action;
        NPCSpot otherSpot;
        switch (action)
        {
            case NPCManager.NPCAction.Rest:
                print("Rest");

                break;
            case NPCManager.NPCAction.CheckConsciousness:

                animator.Play("Anim_ComprobarConciencia");
                Patient.GetInstance().animator.Play("Anim_ComprobarConcienciaPaciente");
                yield return new WaitForSeconds(7.5f);
                break;
            case NPCManager.NPCAction.CheckAirWay:

                animator.Play("Anim_ComprobarRespiracion");
                Patient.GetInstance().animator.Play("Anim_ComprobarRespiracionPaciente");
                yield return new WaitForSeconds(15);
                break;
            case NPCManager.NPCAction.PutGuedel:
                guedel.SetActive(true);
                animator.Play("Anim_IntroducirGuedel");
                Patient.GetInstance().animator.Play("Anim_IntroducirGuedelPaciente");
                yield return new WaitForSeconds(3f);
                guedel.SetActive(false);
                yield return new WaitForSeconds(7);
                break;
            case NPCManager.NPCAction.CheckPulse:
                animator.Play("Anim_ComprobarPulso");
                yield return new WaitForSeconds(11.2f);
                break;
            case NPCManager.NPCAction.Compressions:
                otherSpot = NPCSpotManager.GetInstance().GetNearestSpot(NPCSpot.SpotType.Ventilations, Vector3.zero);
                if (otherSpot.npcInSpot && otherSpot.npcInSpot.GetCurrentAction() == NPCManager.NPCAction.Ventilations)
                {
                    otherSpot.npcInSpot.ambu.SetActive(true);
                    otherSpot.npcInSpot.animator.Play("Anim_GoToVentilar");
                    yield return new WaitForSeconds(7.5f);
                    otherSpot.npcInSpot.ambu.SetActive(false);
                    animator.Play("Anim_GoToComprimir");
                    Patient.GetInstance().animator.Play("Anim_Comprimido");
                }
                break;
            case NPCManager.NPCAction.Ventilations:
                otherSpot = NPCSpotManager.GetInstance().GetNearestSpot(NPCSpot.SpotType.Compressions, Vector3.zero);
                if (otherSpot.npcInSpot && otherSpot.npcInSpot.GetCurrentAction() == NPCManager.NPCAction.Compressions && otherSpot.npcInSpot.CurrentActionEnded())
                {
                    ambu.SetActive(true);
                    animator.Play("Anim_GoToVentilar");
                    yield return new WaitForSeconds(7.5f);
                    ambu.SetActive(false);
                    otherSpot.npcInSpot.animator.Play("Anim_GoToComprimir");
                    Patient.GetInstance().animator.Play("Anim_Comprimido");
                }
                break;
            case NPCManager.NPCAction.CheckDefibrilator:
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                Patient.GetInstance().vitalSignsMonitor.enabled = true;
                yield return new WaitForSeconds(6);
                break;
            case NPCManager.NPCAction.PlacePatches:
                animator.Play("Anim_ColocarParchesDea");
                yield return new WaitForSeconds(1.5f);
                Patient.GetInstance().patches[0].SetActive(true);
                yield return new WaitForSeconds(0.5f);
                Patient.GetInstance().patches[1].SetActive(true);
                yield return new WaitForSeconds(5);
                break;
            case NPCManager.NPCAction.AllOut:
             
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions || NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot)
                        {
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.StartCoroutine(FollowOrder(NPCManager.NPCAction.Rest));
                        }
                    }
                }
                // QUE LOS QUE ESTAN EN LOS SPOTS DE VENTILACIONES Y COMPRESIONES SE DESCANSEN
                yield return new WaitForSeconds(1f);
                break;
            case NPCManager.NPCAction.ChargeDefibrilator:
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                Patient.GetInstance().vitalSignsMonitor.GetComponent<VitalLine>().enabled = true;
                yield return new WaitForSeconds(6);
                break;
            case NPCManager.NPCAction.DischargeDefibrilator:
                animator.Play("Anim_TocarBotonesDea");

                yield return new WaitForSeconds(2);
                Patient.GetInstance().vitalSignsMonitor.GetComponent<VitalLine>().enabled = true;
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            Patient.GetInstance().animator.CrossFade("Anim_IdlePaciente", 0.7f);
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().CrossFade("Anim_idle", 0.7f);
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
                Patient.GetInstance().animator.Play("Anim_RecibeShock");
                yield return new WaitForSeconds(1.2f);
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            Patient.GetInstance().animator.Play("Anim_Comprimido");
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().Play("Anim_Comprimir");
                        }
                    }
                }
                yield return new WaitForSeconds(2f);
                animator.CrossFade("Anim_idle", 0.6f);
                yield return new WaitForSeconds(2f);
                break;
            case NPCManager.NPCAction.PlaceVVP:
                animator.Play("Anim_Colocaraguja");
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            Patient.GetInstance().animator.CrossFade("Anim_IdlePaciente", 0.7f);
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().CrossFade("Anim_idle", 0.7f);
                        }
                    }
                }
                //yield return new WaitForSeconds(0.1f);
                Patient.GetInstance().animator.Play("Anim_PacienteColocarAguja");
                yield return new WaitForSeconds(7.5f);
                Patient.GetInstance().cableSuero.SetActive(true);
                yield return new WaitForSeconds(1.2f);
                for (int i = 0; i < NPCSpotManager.GetInstance().spots.Length; i++)
                {
                    if (NPCSpotManager.GetInstance().spots[i].type == NPCSpot.SpotType.Compressions)
                    {
                        if (NPCSpotManager.GetInstance().spots[i].npcInSpot != null)
                        {
                            Patient.GetInstance().animator.Play("Anim_Comprimido");
                            NPCSpotManager.GetInstance().spots[i].npcInSpot.GetComponent<Animator>().Play("Anim_Comprimir");
                        }
                    }
                }
                yield return new WaitForSeconds(4);
                break;
            case NPCManager.NPCAction.Epinephrine:
                animator.Play("Anim_ExtraerMedicamento");
                yield return new WaitForSeconds(1);
                syringe.SetActive(true);
                medication.SetActive(true);
                yield return new WaitForSeconds(25f);
                syringe.SetActive(false);
                medication.SetActive(false);
                animator.CrossFade("Anim_idle", 0.5f);
                yield return new WaitForSeconds(4);
                StartCoroutine(FollowOrder(NPCManager.NPCAction.Epinephrine2));
                break;
            case NPCManager.NPCAction.Epinephrine2:
                animator.Play("Anim_PincharSuero");
                yield return new WaitForSeconds(6);
                syringe.SetActive(false);
                medication.SetActive(false);
                break;
            case NPCManager.NPCAction.Lidocaine:
                syringe.SetActive(true);
                medication.SetActive(true);
                animator.Play("");
                yield return new WaitForSeconds(9999);
                syringe.SetActive(false);
                medication.SetActive(false);
                break;

        }
        actionCoroutine = null;
        if (nextAction != NPCManager.NPCAction.Rest)
        {
            GiveOrder(nextAction);
        }
        else
        {
            animator.Play("Anim_idle");
        }
    }

    private void SetCurrentSpot(NPCSpot npcSpot)
    {
        if (currentSpot)
        {
            currentSpot.npcInSpot = null;
        }
        currentSpot = npcSpot;
        if (currentSpot)
        {
          currentSpot.npcInSpot = this;
        }
    }

    public NPCManager.NPCAction GetCurrentAction()
    {
        return currentAction;
    }

    public bool CurrentActionEnded()
    {
        return actionCoroutine == null;
    }

    public bool CanPerformAction(NPCManager.NPCAction action)
    {
        return actionCoroutine == null || currentAction == NPCManager.NPCAction.Compressions || currentAction == NPCManager.NPCAction.Ventilations;
    }

    private AudioManager.AudioName ActionAudio(NPCManager.NPCAction action)
    {
        switch (characterName)
        {
            case NPCManager.NPCName.Carla:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Walk:
                        return AudioManager.AudioName.Null; 
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Carla_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Carla_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_güedel_Carla_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carotídeo_Carla_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_Carla_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_Carla_;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_Carla_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_Carla_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_Carla_;
                    case NPCManager.NPCAction.AllOut:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Carla_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_vía_en_el_brazo_Carla_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Carla_;
                    case NPCManager.NPCAction.Epinephrine2:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Carla_;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCName.David:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Walk:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_David_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_David_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_güedel_David_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carotídeo_David_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_David_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_David_;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_David_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_David_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_David_;
                    case NPCManager.NPCAction.AllOut:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_David_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_vía_en_el_brazo_David_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_David_;
                    case NPCManager.NPCAction.Epinephrine2:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_David_;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCName.Rubén:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Walk:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Rubén_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Rubén_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_güedel_Rubén_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carotídeo_Rubén_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_Rubén_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_Rubén_1;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_Rubén_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_Rubén_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_Rubén_;
                    case NPCManager.NPCAction.AllOut:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Rubén_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_vía_en_el_brazo_Rubén_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Rubén_;
                    case NPCManager.NPCAction.Epinephrine2:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Rubén_;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCName.Jesús:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Walk:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Jesús_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Jesús_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_güedel_Jesús_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carotídeo_Jesús_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_Jesús_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_Jesús_;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_Jesús_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_Jesús_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_Jesús_;
                    case NPCManager.NPCAction.AllOut:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Jesús_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_vía_en_el_brazo_Jesús_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Jesús_;
                    case NPCManager.NPCAction.Epinephrine2:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Jesús_;
                    default:
                        break;
                }
                break;
        }
        return AudioManager.AudioName.Null;
    }
}
