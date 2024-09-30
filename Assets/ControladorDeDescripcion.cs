using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeDescripcion : MonoBehaviour
{
    public GameObject descripcionDePulsador;


    private void Start()
    {
        descripcionDePulsador = transform.parent.Find("PanelExplicacion").gameObject; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("PulsadorDeDescripcion"))
        {
            descripcionDePulsador.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("PulsadorDeDescripcion"))
        {
            descripcionDePulsador.SetActive(false);
        }
    }
}
