using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelSeleccionManual : MonoBehaviour
{

    public TextMeshProUGUI[] optionsTextsInGame;

    public string textoOriginalACorrecto;
    public string textoOriginalBIncorrecto;
    public string textoOriginalCIncorrecto;
    public string textoOriginalDIncorrecto;

    public GameObject buttonA;
    public GameObject buttonB;
    public GameObject buttonC;
    public GameObject buttonD;

    public string [] optionsTexts = new string[4];

    public bool[] disponibilidadNPCs;
    public GameObject panelSeleccionNPC;

    public GameObject panelSeleccionAcciones;

    // Start is called before the first frame update
    void Start()
    {
        //Debug
        bool[] debugTestDisponibilidad = new bool [] {true,true,false,false};
        RecibirOpcionesDePaso("Opcion A", "Opcion B", "Opcion C", "Opcion D");
        RefreshAvailableNPCs(debugTestDisponibilidad);

        //Debug
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RecibirOpcionesDePaso(string OptionACorrect, string OptionBIncorrect, string OptionCIncorrect, string OptionDIncorrect)//El primero que recibes es correcto 
    {
        textoOriginalACorrecto = OptionACorrect;
        textoOriginalBIncorrecto = OptionBIncorrect;
        textoOriginalCIncorrecto = OptionCIncorrect;
        textoOriginalDIncorrecto = OptionDIncorrect;
        AssignRandomTexts();

    }

    void AssignRandomTexts()
    {
        // Creamos una lista con todos los textos originales.
        List<string> textos = new List<string>
        {
            textoOriginalACorrecto,
            textoOriginalBIncorrecto,
            textoOriginalCIncorrecto,
            textoOriginalDIncorrecto
        };

        // Barajamos la lista para que los textos estén en orden aleatorio.
        Shuffle(textos);

        // Asignamos los textos barajados al array.
        for (int i = 0; i < optionsTexts.Length; i++)
        {
            optionsTexts[i] = textos[i];
        }

        for (int i = 0; i < optionsTextsInGame.Length; i++)
        {
            if (i == 0)
            {
                optionsTextsInGame[i].text = "A- ";
            }
            else if (i == 1)
            {
                optionsTextsInGame[i].text = "B- ";
            }
            else if (i == 2)
            {
                optionsTextsInGame[i].text = "C- ";
            }
            else if (i == 3)
            {
                optionsTextsInGame[i].text = "D- ";
            }
            optionsTextsInGame[i].text += optionsTexts[i];
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SeleccionaA() 
    {
        if (optionsTexts[0] == textoOriginalACorrecto)
        {
            print("Correcto");
        }
        else
        {
            print(optionsTexts[0] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionaB()
    {
        if (optionsTexts[1] == textoOriginalACorrecto)
        {
            print("Correcto");
        }
        else
        {
            print(optionsTexts[1] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionaC()
    {
        if (optionsTexts[2] == textoOriginalACorrecto)
        {
            print("Correcto");
        }
        else
        {
            print(optionsTexts[2] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionaD()
    {
        if (optionsTexts[3] == textoOriginalACorrecto)
        {
            print("Correcto");
        }
        else
        {
            print(optionsTexts[3] + " Es incorrecto en este paso");
        }
    }
    public void SeleccionarNPC(int seleccion) //0 Carla // 1 David //2 Ruben // 3 Jesus
    {
        if (disponibilidadNPCs[seleccion])
        {
            print("//0 Carla //1 David //2 Ruben //3 Jesus ");

            print("Selecionado NPC Disponible  " + seleccion);
            panelSeleccionNPC.GetComponent<Animator>().Play("PanelDisappear");
            Invoke(nameof(CerrarPanelNPCAbrirPanelAcciones),1f);
        }
        else
        {
            print("//0 Carla //1 David //2 Ruben //3 Jesus ");

            print("Selecionado NPC NO Disponible  " + seleccion);
        }
    }

    public void CerrarPanelNPCAbrirPanelAcciones() 
    {
        panelSeleccionNPC.SetActive(false);
        panelSeleccionAcciones.SetActive(true);
    }

    public void RefreshAvailableNPCs(bool [] npcsAvailabilityArray) //Tienene que pasarse siempre de  4
    {
        for (int i = 0; i < disponibilidadNPCs.Length; i++)
        {
            disponibilidadNPCs[i] = npcsAvailabilityArray[i];
        }
    }
}
