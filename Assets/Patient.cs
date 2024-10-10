using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient : MonoBehaviourInstance<Patient>
{
    public Animator animator;

    public GameObject[] patches;
    public VitalLine vitalSignsMonitor;
    public GameObject cableSuero;

    public bool isBreathing;
    public bool hasPulse;

    private int compressions;
    private int ventilations;
    private int discharges;
    private bool epinephrine;
    private bool lidocaine;

    public void VentilationCycle()
    {
        if (ventilations <= compressions)
        {
            ventilations++;
        }
    }

    public void CompressionCycle()
    {
        if (compressions <= ventilations)
        {
            compressions++;
        }
    }

    public void Discharge()
    {
        discharges++;
    }

    public void Epinephrine()
    {
        epinephrine = true;
    }

    public void Lidocaine()
    {
        lidocaine = true;
    }
}
