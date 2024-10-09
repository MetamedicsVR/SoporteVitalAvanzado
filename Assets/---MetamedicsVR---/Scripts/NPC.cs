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
    public NPCSpot currentSpot;
    public NPCManager.NPCAction currentAction;
    public NPCManager.NPCAction nextAction;
    public NPCManager.NPCAction lastAction;
    private Coroutine actionCoroutine;
    private bool waiting;

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
            if (action == NPCManager.NPCAction.OutNow)
            {
                StopCoroutine(FollowOrder());
                SetCurrentAction(action);
                audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(action));
                audioSource.Play();
                nextAction = NPCManager.NPCAction.Rest;
                actionCoroutine = StartCoroutine(FollowOrder());
            }
            else
            {
                nextAction = action;
            }
        }
        else
        {
            SetCurrentAction(action);
            if (currentAction == NPCManager.NPCAction.Compressions || currentAction == NPCManager.NPCAction.Ventilations)
            {
                nextAction = currentAction;
            }
            else
            {
                audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(action));
                audioSource.Play();
                nextAction = NPCManager.NPCAction.Rest;
            }
            actionCoroutine = StartCoroutine(FollowOrder());
        }
    }

    private IEnumerator FollowOrder()
    {
        yield return EndLastAction();
        yield return WalkToSpot();
        yield return StartNextAction();
    }

    private IEnumerator EndLastAction()
    {
        yield return null;
        switch (lastAction)
        {
            case NPCManager.NPCAction.Rest:
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
                Patient.GetInstance().animator.Play("Anim_IdlePaciente");
                break;
            case NPCManager.NPCAction.Ventilations:
                ambu.SetActive(false);
                break;
            case NPCManager.NPCAction.CheckDefibrilator:
                break;
            case NPCManager.NPCAction.PlacePatches:
                break;
            case NPCManager.NPCAction.ChargeDefibrilator:
                break;
            case NPCManager.NPCAction.OutNow:
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
            case NPCManager.NPCAction.Lidocaine2:
                break;
            case NPCManager.NPCAction.OutAfterEnd:
                break;
            default:
                break;
        }
    }

    private IEnumerator WalkToSpot()
    {
        NPCSpot.SpotType correctSpotType = NPCManager.GetInstance().GetCorrectSpotType(currentAction);
        if (!currentSpot || currentSpot.type != correctSpotType)
        {
            if (currentAction == NPCManager.NPCAction.Compressions || currentAction == NPCManager.NPCAction.Ventilations)
            {
                audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(currentAction));
                audioSource.Play();
            }
            NPCSpot targetSpot = NPCSpotManager.GetInstance().GetNearestFreeSpot(correctSpotType, transform.position);
            if (targetSpot.npcInSpot)
            {
                yield return new WaitUntil(() => !targetSpot.npcInSpot || targetSpot.npcInSpot.currentAction == NPCManager.NPCAction.Rest);
                targetSpot.npcInSpot.GiveOrder(NPCManager.NPCAction.OutAfterEnd);
            }
            SetCurrentSpot(targetSpot);
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
    }

    private IEnumerator StartNextAction()
    {
        NPCSpot otherSpot;
        switch (currentAction)
        {
            case NPCManager.NPCAction.CheckConsciousness:
                GameManager.GetInstance().askThemToComePanel.SetActive(false);
                animator.Play("Anim_ComprobarConciencia");
                Patient.GetInstance().animator.Play("Anim_ComprobarConcienciaPaciente");
                yield return new WaitForSeconds(3f);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_usted_bien_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_usted_bien_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_usted_bien_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_usted_bien_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(4.5f);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_inconsciente_Carla_);
                        GetComponent<AudioSource>().Play();
                  
                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_inconsciente_David_);
                        GetComponent<AudioSource>().Play();
       
                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_inconsciente_Rub�n_);
                        GetComponent<AudioSource>().Play();
                      
                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Est�_inconsciente_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCAction.CheckAirWay:

                animator.Play("Anim_ComprobarRespiracion");
                Patient.GetInstance().animator.Play("Anim_ComprobarRespiracionPaciente");
                yield return new WaitForSeconds(3);
              

                yield return new WaitForSeconds(10);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_respira_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_respira_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_respira_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_respira_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
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
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_tiene_pulso_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_tiene_pulso_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_tiene_pulso_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._No_tiene_pulso_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCAction.Compressions:
                otherSpot = NPCSpotManager.GetInstance().GetNearestSpot(NPCSpot.SpotType.Ventilations, Vector3.zero);
                if (otherSpot.npcInSpot && otherSpot.npcInSpot.currentAction != NPCManager.NPCAction.Rest)
                {
                    SetCurrentAction(NPCManager.NPCAction.Rest);
                    yield return new WaitUntil(() => !otherSpot.npcInSpot || otherSpot.npcInSpot.currentAction == NPCManager.NPCAction.Rest);
                    SetCurrentAction(NPCManager.NPCAction.Compressions);
                    audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(currentAction));
                    audioSource.Play();
                }
                animator.Play("Anim_GoToComprimir");
                if (!CPRTree.GetInstance().timer.running)
                {
                    CPRTree.GetInstance().timer.StartTimer();
                    CPRTree.GetInstance().timer.ShowTimer();
                }
                yield return new WaitForSeconds(1);
                Patient.GetInstance().animator.Play("Anim_Comprimido");
                yield return new WaitForSeconds(30);
                animator.Play("Anim_idle");
                SetCurrentAction(NPCManager.NPCAction.Rest);
                yield return new WaitForSeconds(1);
                SetCurrentAction(NPCManager.NPCAction.Compressions);
                break;
            case NPCManager.NPCAction.Ventilations:
                otherSpot = NPCSpotManager.GetInstance().GetNearestSpot(NPCSpot.SpotType.Compressions, Vector3.zero);
                if (otherSpot.npcInSpot && otherSpot.npcInSpot.currentAction != NPCManager.NPCAction.Rest)
                {
                    SetCurrentAction(NPCManager.NPCAction.Rest);
                    yield return new WaitUntil(() => !otherSpot.npcInSpot || otherSpot.npcInSpot.currentAction == NPCManager.NPCAction.Rest);
                    SetCurrentAction(NPCManager.NPCAction.Ventilations);
                    audioSource.clip = AudioManager.GetInstance().GetAudioClip(ActionAudio(currentAction));
                    audioSource.Play();
                }
                ambu.SetActive(true);
                animator.Play("Anim_GoToVentilar");
                yield return new WaitForSeconds(11.5f);
                Patient.GetInstance().animator.Play("Anim_IdlePaciente");
                SetCurrentAction(NPCManager.NPCAction.Rest);
                yield return new WaitForSeconds(1);
                SetCurrentAction(NPCManager.NPCAction.Ventilations);
                break;
            case NPCManager.NPCAction.CheckDefibrilator:
                CPRTree.GetInstance().pantallaFibrilarVentriculationInDefibrilator.SetActive(true);
                CPRTree.GetInstance().pantallaFibrilarVentriculationPlayer.SetActive(true);
                animator.Play("Anim_TocarBotonesDea");
                yield return new WaitForSeconds(2);
                Patient.GetInstance().vitalSignsMonitor.enabled = true;
                yield return new WaitForSeconds(4);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Paciente_monitorizado_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Paciente_monitorizado_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Paciente_monitorizado_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Paciente_monitorizado_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(2);
                break;
            case NPCManager.NPCAction.PlacePatches:
                animator.Play("Anim_ColocarParchesDea");
                yield return new WaitForSeconds(1.5f);
                Patient.GetInstance().patches[0].SetActive(true);
                yield return new WaitForSeconds(0.5f);
                Patient.GetInstance().patches[1].SetActive(true);
                yield return new WaitForSeconds(5);
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
                yield return new WaitForSeconds(0.2f);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Descarga_completada_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Descarga_completada_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Descarga_completada_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Descarga_completada_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(1f);
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
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._V�a_colocada_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._V�a_colocada_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._V�a_colocada_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._V�a_colocada_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(4);
                break;
            case NPCManager.NPCAction.Epinephrine:
                ambu.SetActive(false);
                animator.Play("Anim_ExtraerMedicamento");
                yield return new WaitForSeconds(1);
                syringe.SetActive(true);
                medication.SetActive(true);
                yield return new WaitForSeconds(25f);
                syringe.SetActive(false);
                medication.SetActive(false);
                animator.CrossFade("Anim_idle", 0.5f);
                yield return new WaitForSeconds(4);
                SetCurrentAction(NPCManager.NPCAction.Epinephrine2);
                yield return FollowOrder();
                break;
            case NPCManager.NPCAction.Epinephrine2:
                ambu.SetActive(false);
                animator.Play("Anim_PincharSuero");
                yield return new WaitForSeconds(6);
                syringe.SetActive(false);
                medication.SetActive(false);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Adrenalina_administrada_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Adrenalina_administrada_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Adrenalina_administrada_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Adrenalina_administrada_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCAction.Lidocaine:
                ambu.SetActive(false);
                animator.Play("Anim_ExtraerMedicamento");
                yield return new WaitForSeconds(1);
                syringe.SetActive(true);
                medication.SetActive(true);
                yield return new WaitForSeconds(25f);
                syringe.SetActive(false);
                medication.SetActive(false);
                //animator.CrossFade("Anim_idle", 0.5f);
                yield return new WaitForSeconds(4);
                SetCurrentAction(NPCManager.NPCAction.Lidocaine2);
                yield return FollowOrder();
                break;
            case NPCManager.NPCAction.Lidocaine2:
                ambu.SetActive(false);
                animator.Play("Anim_PincharSuero");
                yield return new WaitForSeconds(6);
                syringe.SetActive(false);
                medication.SetActive(false);
                switch (characterName)
                {
                    case NPCManager.NPCName.Carla:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Amiodarona_administrada_Carla_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.David:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Amiodarona_administrada_David_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Rub�n:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Amiodarona_administrada_Rub�n_);
                        GetComponent<AudioSource>().Play();

                        break;
                    case NPCManager.NPCName.Jes�s:
                        GetComponent<AudioSource>().clip = AudioManager.GetInstance().LoadAudioClip(AudioManager.AudioName._Adrenalina_administrada_Jes�s_);
                        GetComponent<AudioSource>().Play();
                        break;
                    default:
                        break;
                }
                CPRTree.GetInstance().timer.StopTimer();
                break;      
        }
        actionCoroutine = null;
        if (nextAction != NPCManager.NPCAction.Rest)
        {
            GiveOrder(nextAction);
        }
        else
        {
            SetCurrentAction(NPCManager.NPCAction.Rest);
            animator.Play("Anim_idle");
        }

    }

    public void SetCurrentAction(NPCManager.NPCAction action)
    {
        if (currentAction != NPCManager.NPCAction.Rest)
        {
            lastAction = currentAction;
        }
        currentAction = action;
    }

    private void SetCurrentSpot(NPCSpot npcSpot)
    {
        if (currentSpot && currentSpot.npcInSpot == this)
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

    public NPCManager.NPCAction GetNextAction()
    {
        return nextAction;
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
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Carla_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Carla_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_g�edel_Carla_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carot�deo_Carla_;
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
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Carla_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_v�a_en_el_brazo_Carla_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Carla_;
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
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_David_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_David_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_g�edel_David_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carot�deo_David_;
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
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_David_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_v�a_en_el_brazo_David_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_David_;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_David_;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCName.Rub�n:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Rub�n_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Rub�n_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_g�edel_Rub�n_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carot�deo_Rub�n_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_Rub�n_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_Rub�n_1;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_Rub�n_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_Rub�n_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_Rub�n_;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Rub�n_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_v�a_en_el_brazo_Rub�n_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Rub�n_;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Rub�n_;
                    default:
                        break;
                }
                break;
            case NPCManager.NPCName.Jes�s:
                switch (action)
                {
                    case NPCManager.NPCAction.Rest:
                        return AudioManager.AudioName.Null;
                    case NPCManager.NPCAction.CheckConsciousness:
                        return AudioManager.AudioName._Comprobando_consciencia_Jes�s_;
                    case NPCManager.NPCAction.CheckAirWay:
                        return AudioManager.AudioName._Abriendo_via_aerea_Jes�s_;
                    case NPCManager.NPCAction.PutGuedel:
                        return AudioManager.AudioName._Colocando_canula_de_g�edel_Jes�s_;
                    case NPCManager.NPCAction.CheckPulse:
                        return AudioManager.AudioName._Tomando_pulso_carot�deo_Jes�s_;
                    case NPCManager.NPCAction.Compressions:
                        return AudioManager.AudioName._Empezando_ciclo_de_compresiones_Jes�s_;
                    case NPCManager.NPCAction.Ventilations:
                        return AudioManager.AudioName._Ventilando_con_ambu_Jes�s_;
                    case NPCManager.NPCAction.CheckDefibrilator:
                        return AudioManager.AudioName._Monitorizando_con_defibrilador_Jes�s_;
                    case NPCManager.NPCAction.PlacePatches:
                        return AudioManager.AudioName._Colocando_parches_Jes�s_;
                    case NPCManager.NPCAction.ChargeDefibrilator:
                        return AudioManager.AudioName._Cargando_desfibrilador_a_150_julios_Jes�s_;
                    case NPCManager.NPCAction.DischargeDefibrilator:
                        return AudioManager.AudioName._Dando_descarga_Jes�s_;
                    case NPCManager.NPCAction.PlaceVVP:
                        return AudioManager.AudioName._Cogiendo_v�a_en_el_brazo_Jes�s_;
                    case NPCManager.NPCAction.Epinephrine:
                        return AudioManager.AudioName._Inyectando_1_miligramo_de_adrenalina_Intravenosa_Jes�s_;
                    case NPCManager.NPCAction.Lidocaine:
                        return AudioManager.AudioName._Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Jes�s_;
                    default:
                        break;
                }
                break;
        }
        return AudioManager.AudioName.Null;
    }
}
