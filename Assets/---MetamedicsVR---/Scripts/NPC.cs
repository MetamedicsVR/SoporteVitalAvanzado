using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public NPCManager.NPCName characterName;
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
        NPCSpot.SpotType correctSpotType = NPCManager.GetInstance().GetCorrectSpotType(action);
        if (!currentSpot || currentSpot.type != correctSpotType)
        {
            NPCSpot targetSpot = NPCSpotManager.GetInstance().GetNearestSpot(correctSpotType, transform.position);
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
                animator.Play("Anim_GoToComprimir");
                yield return new WaitForSeconds(0.75f);
                Patient.GetInstance().animator.Play("Anim_Comprimido");
                yield return new WaitForSeconds(9999);
                break;
            case NPCManager.NPCAction.Ventilations:
                ambu.SetActive(true);
                animator.Play("Anim_GoToVentilar");

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
                yield return new WaitForSeconds(7.5f);

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
                animator.CrossFade("Anim_idle", 1);
                yield return new WaitForSeconds(3f);
                ambu.SetActive(false);
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

    public bool CanPerformAction(NPCManager.NPCAction action)
    {
        return actionCoroutine == null || currentAction == NPCManager.NPCAction.Compressions || currentAction == NPCManager.NPCAction.Ventilations;
    }
}
