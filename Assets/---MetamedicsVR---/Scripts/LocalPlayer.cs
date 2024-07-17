using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalPlayer : MonoBehaviourInstance<LocalPlayer>
{
    [Header("Head")]
    public Transform head;
    public Camera headCamera;
    public HeadPanel headPanel;

    [Header("Body")]
    public CharacterController characterController;

    [Header("Left Hand")]
    public Transform leftHandAnchor;
    public SkinnedMeshRenderer leftHandRenderer;
    public Transform leftIndexFinger;

    [Header("Right Hand")]
    public Transform rightHandAnchor;
    public SkinnedMeshRenderer rightHandRenderer;
    public Transform rightIndexFinger;

    public enum Side
    {
        Any,
        Left,
        Right
    }

    #region TELEPORT

    public void Teleport(Vector3 position)
    {
        StartCoroutine(Teleporting(position));
    }

    public void Teleport(Vector3 position, Vector3 lookDirection)
    {
        StartCoroutine(Teleporting(position, lookDirection));
    }

    private IEnumerator Teleporting(Vector3 position)
    {
        headPanel.FadeToBlack();
        yield return new WaitForSeconds(HeadPanel.defaultFadeTime);
        transform.position = position;
        headPanel.FadeToTransparent();
    }

    private IEnumerator Teleporting(Vector3 position, Vector3 lookDirection)
    {
        headPanel.FadeToBlack();
        yield return new WaitForSeconds(HeadPanel.defaultFadeTime);
        transform.position = position;
        LookAt(lookDirection);
        //transform.position += new Vector3(transform.position.x - head.position.x, 0, transform.position.z - head.position.z);
        headPanel.FadeToTransparent();
    }

    public void LookAt(Transform target)
    {
        LookAt(target.position - head.position);
    }

    public void LookAt(Vector3 targetDirection)
    {
        transform.localEulerAngles += new Vector3(0, Vector3.SignedAngle(head.forward, targetDirection, Vector3.up), 0);
    }

    #endregion

    #region HANDS

    public void ActivateHandLeft()
    {
        leftHandAnchor.gameObject.SetActive(true);
    }

    public void ActivateHandRight()
    {
        rightHandAnchor.gameObject.SetActive(true);
    }

    public void ActivateHands()
    {
        ActivateHandLeft();
        ActivateHandRight();
    }

    public void DeactivateHandLeft()
    {
        leftHandAnchor.gameObject.SetActive(false);
    }

    public void DeactivateHandRight()
    {
        rightHandAnchor.gameObject.SetActive(false);
    }

    public void DeactivateHands()
    {
        DeactivateHandLeft();
        DeactivateHandRight();
    }

    public void ShowHandLeft()
    {
        leftHandRenderer.enabled = true;
    }

    public void ShowHandRight()
    {
        rightHandRenderer.enabled = true;
    }

    public void ShowHands()
    {
        ShowHandLeft();
        ShowHandRight();
    }

    public void HideHandLeft()
    {
        leftHandRenderer.enabled = false;
    }

    public void HideHandRight()
    {
        rightHandRenderer.enabled = false;
    }

    public void HideHands()
    {
        HideHandLeft();
        HideHandRight();
    }

    #endregion
}